import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_card.dart';
import '../../../../core/widgets/arah_journey_shell.dart';
import '../../../../core/widgets/arah_loading_indicator.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../feed/presentation/providers/feed_provider.dart';
import '../../data/models/store_product.dart';
import '../providers/marketplace_provider.dart';

/// Jornada adicionar/editar produto (APP-DS-15/16): detalhes → foto+descrição → revisão.
class AddProductJourneyScreen extends ConsumerStatefulWidget {
  const AddProductJourneyScreen({super.key, this.itemId});

  final String? itemId;

  @override
  ConsumerState<AddProductJourneyScreen> createState() =>
      _AddProductJourneyScreenState();
}

class _AddProductJourneyScreenState
    extends ConsumerState<AddProductJourneyScreen> {
  static const int _totalSteps = 3;

  int _step = 0;
  bool _submitting = false;
  bool _loadingEdit = false;
  final _titleController = TextEditingController();
  final _categoryController = TextEditingController();
  final _priceController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _imagePicker = ImagePicker();
  String _pricingType = 'Fixed';
  Uint8List? _photoBytes;
  String? _photoFileName;
  String _photoMimeType = 'image/jpeg';
  /// Media id já enviado — evita re-upload em retry.
  String? _uploadedMediaId;
  String? _existingImageUrl;
  bool _startedWithPhoto = false;

  bool get _isEdit => widget.itemId != null && widget.itemId!.isNotEmpty;

  bool get _hasLocalPhoto => _photoBytes != null && _photoBytes!.isNotEmpty;

  bool get _hasPhoto =>
      _hasLocalPhoto ||
      (_existingImageUrl != null && _existingImageUrl!.isNotEmpty) ||
      (_uploadedMediaId != null && _uploadedMediaId!.isNotEmpty);

  @override
  void initState() {
    super.initState();
    if (_isEdit) {
      _loadingEdit = true;
      WidgetsBinding.instance.addPostFrameCallback((_) => _loadExisting());
    }
  }

  @override
  void dispose() {
    _titleController.dispose();
    _categoryController.dispose();
    _priceController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _loadExisting() async {
    final l10n = AppLocalizations.of(context)!;
    try {
      final product =
          await ref.read(marketplaceProvider.notifier).getProduct(widget.itemId!);
      if (!mounted) return;
      _applyProduct(product);
      setState(() => _loadingEdit = false);
    } catch (e) {
      if (!mounted) return;
      setState(() => _loadingEdit = false);
      showErrorSnackBar(
        context,
        e is ApiException ? e.userMessage : l10n.errorSaveProduct,
      );
    }
  }

  void _applyProduct(StoreProduct product) {
    _titleController.text = product.title;
    _categoryController.text = product.category ?? '';
    _descriptionController.text = product.description ?? '';
    _pricingType = _normalizePricing(product.pricingType);
    _existingImageUrl = product.primaryImageUrl;
    _startedWithPhoto = product.primaryImageUrl != null &&
        product.primaryImageUrl!.trim().isNotEmpty;
    if (product.priceAmount != null) {
      _priceController.text = product.priceAmount!.toStringAsFixed(2);
    }
  }

  String _normalizePricing(String raw) {
    final lower = raw.toLowerCase();
    if (lower.contains('negot')) return 'Negotiable';
    if (lower.contains('free')) return 'Free';
    return 'Fixed';
  }

  void _close() {
    if (context.canPop()) {
      context.pop(true);
    } else {
      context.go('/marketplace');
    }
  }

  Future<void> _pickPhoto() async {
    final picked = await _imagePicker.pickImage(
      source: ImageSource.gallery,
      imageQuality: 85,
    );
    if (picked == null || !mounted) return;
    final bytes = await picked.readAsBytes();
    if (!mounted) return;
    setState(() {
      _photoBytes = bytes;
      _photoFileName = picked.name;
      _photoMimeType =
          (picked.mimeType == null || picked.mimeType!.trim().isEmpty)
              ? 'image/jpeg'
              : picked.mimeType!.trim();
      _uploadedMediaId = null;
      _existingImageUrl = null;
    });
  }

  void _clearPhoto() {
    setState(() {
      _photoBytes = null;
      _photoFileName = null;
      _photoMimeType = 'image/jpeg';
      _uploadedMediaId = null;
      _existingImageUrl = null;
    });
  }

  bool get _detailsValid {
    if (_titleController.text.trim().isEmpty) return false;
    if (_pricingType == 'Fixed') {
      final amount = double.tryParse(_priceController.text.replaceAll(',', '.'));
      if (amount == null || amount <= 0) return false;
    }
    return true;
  }

  /// Resolve `mediaIds` + se devem ir no PATCH.
  /// - Nova foto local → upload (com cache de retry).
  /// - Remoção da foto existente → lista vazia + include.
  /// - Edição sem mudança de foto → null / não incluir (preserva).
  Future<({List<String>? mediaIds, bool includeMediaIds})> _resolveMedia() async {
    if (_hasLocalPhoto) {
      final bytesSnapshot = _photoBytes!;
      final mimeSnapshot = _photoMimeType;
      final fileName = (_photoFileName == null || _photoFileName!.isEmpty)
          ? 'produto.jpg'
          : _photoFileName!;
      final mediaId = (_uploadedMediaId != null && _uploadedMediaId!.isNotEmpty)
          ? _uploadedMediaId!
          : await ref.read(mediaRepositoryProvider).uploadImage(
                fileName: fileName,
                mimeType: mimeSnapshot,
                bytes: bytesSnapshot,
              );

      // Seleção pode ter mudado enquanto o upload corria (voltar/trocar/remover).
      if (!identical(_photoBytes, bytesSnapshot)) {
        _uploadedMediaId = null;
        return _resolveMedia();
      }

      _uploadedMediaId = mediaId;
      return (mediaIds: <String>[mediaId], includeMediaIds: true);
    }

    // Sem foto local: limpa cache de upload para não reanexar id antigo.
    _uploadedMediaId = null;

    final clearedExisting = _isEdit &&
        _startedWithPhoto &&
        (_existingImageUrl == null || _existingImageUrl!.trim().isEmpty);
    if (clearedExisting) {
      return (mediaIds: <String>[], includeMediaIds: true);
    }

    return (mediaIds: null, includeMediaIds: false);
  }

  Future<void> _onPrimary() async {
    final l10n = AppLocalizations.of(context)!;
    if (_step < 2) {
      setState(() => _step += 1);
      return;
    }

    setState(() => _submitting = true);
    late final List<String>? mediaIds;
    late final bool includeMediaIds;
    try {
      final resolved = await _resolveMedia();
      if (!mounted) return;
      mediaIds = resolved.mediaIds;
      includeMediaIds = resolved.includeMediaIds;
    } catch (e) {
      if (!mounted) return;
      setState(() => _submitting = false);
      showErrorSnackBar(
        context,
        e is ApiException ? e.userMessage : l10n.errorUploadProductPhoto,
      );
      return;
    }

    try {
      final notifier = ref.read(marketplaceProvider.notifier);
      final title = _titleController.text.trim();
      final category = _categoryController.text.trim();
      final description = _descriptionController.text.trim();
      final price = _pricingType == 'Fixed'
          ? double.tryParse(_priceController.text.replaceAll(',', '.'))
          : null;

      if (_isEdit) {
        await notifier.updateProduct(
          itemId: widget.itemId!,
          title: title,
          description: description.isEmpty ? null : description,
          category: category.isEmpty ? null : category,
          pricingType: _pricingType,
          priceAmount: price,
          mediaIds: mediaIds,
          includeMediaIds: includeMediaIds,
        );
      } else {
        await notifier.createProduct(
          title: title,
          description: description.isEmpty ? null : description,
          category: category.isEmpty ? null : category,
          pricingType: _pricingType,
          priceAmount: price,
          mediaIds: mediaIds,
        );
      }
      if (!mounted) return;
      setState(() => _submitting = false);
      showSuccessSnackBar(
        context,
        _isEdit ? l10n.productUpdated : l10n.productCreated,
      );
      _close();
    } catch (e) {
      if (!mounted) return;
      setState(() => _submitting = false);
      showErrorSnackBar(
        context,
        e is ApiException ? e.userMessage : l10n.errorSaveProduct,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final store = ref.watch(marketplaceProvider).myStore;

    if (_loadingEdit) {
      return const Scaffold(body: Center(child: ArahLoadingIndicator()));
    }

    if (store == null && !_isEdit) {
      return ArahJourneyShell(
        title: l10n.addProductTitle,
        currentStep: 0,
        totalSteps: 1,
        onClose: _close,
        primaryActionLabel: l10n.checkoutDone,
        onPrimaryAction: _close,
        child: Text(l10n.openStoreEmptyDescription),
      );
    }

    final primaryLabel = _step < 2
        ? l10n.continueButton
        : (_isEdit ? l10n.saveChanges : l10n.publishProduct);

    return ArahJourneyShell(
      title: _isEdit ? l10n.editProductTitle : l10n.addProductTitle,
      currentStep: _step,
      totalSteps: _totalSteps,
      onClose: _close,
      onBack: (_step > 0 && !_submitting)
          ? () => setState(() => _step -= 1)
          : null,
      primaryActionLabel: primaryLabel,
      onPrimaryAction: _submitting
          ? null
          : (_step == 0 && !_detailsValid ? null : _onPrimary),
      primaryEnabled: !_submitting && (_step > 0 || _detailsValid),
      primaryLoading: _submitting,
      child: _buildStep(l10n),
    );
  }

  Widget _buildStep(AppLocalizations l10n) {
    switch (_step) {
      case 0:
        return _DetailsStep(
          l10n: l10n,
          titleController: _titleController,
          categoryController: _categoryController,
          priceController: _priceController,
          pricingType: _pricingType,
          onPricingChanged: (v) => setState(() => _pricingType = v),
          onChanged: () => setState(() {}),
        );
      case 1:
        return _PhotoDescriptionStep(
          l10n: l10n,
          descriptionController: _descriptionController,
          photoBytes: _photoBytes,
          existingImageUrl: _existingImageUrl,
          interactionsEnabled: !_submitting,
          onPickPhoto: _pickPhoto,
          onClearPhoto: _clearPhoto,
        );
      default:
        return _ReviewStep(
          l10n: l10n,
          title: _titleController.text.trim(),
          category: _categoryController.text.trim(),
          description: _descriptionController.text.trim(),
          pricingType: _pricingType,
          priceText: _priceController.text.trim(),
          hasPhoto: _hasPhoto,
          photoBytes: _photoBytes,
          existingImageUrl: _existingImageUrl,
        );
    }
  }
}

class _DetailsStep extends StatelessWidget {
  const _DetailsStep({
    required this.l10n,
    required this.titleController,
    required this.categoryController,
    required this.priceController,
    required this.pricingType,
    required this.onPricingChanged,
    required this.onChanged,
  });

  final AppLocalizations l10n;
  final TextEditingController titleController;
  final TextEditingController categoryController;
  final TextEditingController priceController;
  final String pricingType;
  final ValueChanged<String> onPricingChanged;
  final VoidCallback onChanged;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.addProductDetailsTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        TextField(
          controller: titleController,
          decoration: InputDecoration(
            labelText: l10n.productTitleLabel,
            border: const OutlineInputBorder(),
          ),
          onChanged: (_) => onChanged(),
        ),
        const SizedBox(height: AppConstants.spacingMd),
        TextField(
          controller: categoryController,
          decoration: InputDecoration(
            labelText: l10n.productCategoryLabel,
            border: const OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: AppConstants.spacingMd),
        Text(l10n.productPricingType, style: theme.textTheme.titleSmall),
        const SizedBox(height: AppConstants.spacingSm),
        Wrap(
          spacing: AppConstants.spacingSm,
          children: [
            for (final option in const ['Fixed', 'Negotiable', 'Free'])
              ChoiceChip(
                label: Text(_pricingLabel(l10n, option)),
                selected: pricingType == option,
                onSelected: (_) {
                  onPricingChanged(option);
                  onChanged();
                },
              ),
          ],
        ),
        if (pricingType == 'Fixed') ...[
          const SizedBox(height: AppConstants.spacingMd),
          TextField(
            controller: priceController,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(
              labelText: l10n.productPriceLabel,
              border: const OutlineInputBorder(),
              prefixText: 'BRL ',
            ),
            onChanged: (_) => onChanged(),
          ),
        ],
      ],
    );
  }

  String _pricingLabel(AppLocalizations l10n, String value) {
    switch (value) {
      case 'Negotiable':
        return l10n.pricingNegotiable;
      case 'Free':
        return l10n.pricingFree;
      default:
        return l10n.pricingFixed;
    }
  }
}

class _PhotoDescriptionStep extends StatelessWidget {
  const _PhotoDescriptionStep({
    required this.l10n,
    required this.descriptionController,
    required this.photoBytes,
    required this.existingImageUrl,
    required this.interactionsEnabled,
    required this.onPickPhoto,
    required this.onClearPhoto,
  });

  final AppLocalizations l10n;
  final TextEditingController descriptionController;
  final Uint8List? photoBytes;
  final String? existingImageUrl;
  final bool interactionsEnabled;
  final VoidCallback onPickPhoto;
  final VoidCallback onClearPhoto;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = context.appColors;
    final hasLocal = photoBytes != null && photoBytes!.isNotEmpty;
    final hasRemote =
        existingImageUrl != null && existingImageUrl!.isNotEmpty;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.addProductPhotoDescriptionTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        Text(
          l10n.addProductDescriptionHint,
          style: theme.textTheme.bodyMedium?.copyWith(
            color: colors.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        Material(
          color: colors.surfaceContainer.withValues(alpha: 0.55),
          borderRadius: BorderRadius.circular(AppConstants.radiusMd),
          child: InkWell(
            onTap: interactionsEnabled ? onPickPhoto : null,
            borderRadius: BorderRadius.circular(AppConstants.radiusMd),
            child: Container(
              width: double.infinity,
              constraints: const BoxConstraints(minHeight: 160),
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(AppConstants.radiusMd),
                border: Border.all(
                  color: colors.outlineSubtle,
                  style: BorderStyle.solid,
                ),
              ),
              clipBehavior: Clip.antiAlias,
              child: hasLocal
                  ? Image.memory(
                      photoBytes!,
                      height: 180,
                      width: double.infinity,
                      fit: BoxFit.cover,
                    )
                  : hasRemote
                      ? Image.network(
                          existingImageUrl!,
                          height: 180,
                          width: double.infinity,
                          fit: BoxFit.cover,
                          errorBuilder: (_, __, ___) => _PhotoPlaceholder(
                            l10n: l10n,
                            colors: colors,
                          ),
                        )
                      : _PhotoPlaceholder(l10n: l10n, colors: colors),
            ),
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        Row(
          children: [
            TextButton.icon(
              onPressed: interactionsEnabled ? onPickPhoto : null,
              icon: const Icon(Icons.add_a_photo_outlined, size: 18),
              label: Text(
                hasLocal || hasRemote
                    ? l10n.changeProductPhoto
                    : l10n.addProductPhoto,
              ),
            ),
            if (hasLocal || hasRemote) ...[
              const SizedBox(width: AppConstants.spacingSm),
              TextButton(
                onPressed: interactionsEnabled ? onClearPhoto : null,
                child: Text(l10n.removeProductPhoto),
              ),
            ],
          ],
        ),
        const SizedBox(height: AppConstants.spacingLg),
        TextField(
          controller: descriptionController,
          maxLines: 5,
          decoration: InputDecoration(
            labelText: l10n.descriptionLabel,
            border: const OutlineInputBorder(),
            alignLabelWithHint: true,
          ),
        ),
      ],
    );
  }
}

class _PhotoPlaceholder extends StatelessWidget {
  const _PhotoPlaceholder({required this.l10n, required this.colors});

  final AppLocalizations l10n;
  final AppColors colors;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(AppConstants.spacingLg),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.add_a_photo_outlined, size: 40, color: colors.primary),
          const SizedBox(height: AppConstants.spacingSm),
          Text(
            l10n.addProductPhotoHint,
            textAlign: TextAlign.center,
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: colors.onSurfaceVariant,
                ),
          ),
        ],
      ),
    );
  }
}

class _ReviewStep extends StatelessWidget {
  const _ReviewStep({
    required this.l10n,
    required this.title,
    required this.category,
    required this.description,
    required this.pricingType,
    required this.priceText,
    required this.hasPhoto,
    required this.photoBytes,
    required this.existingImageUrl,
  });

  final AppLocalizations l10n;
  final String title;
  final String category;
  final String description;
  final String pricingType;
  final String priceText;
  final bool hasPhoto;
  final Uint8List? photoBytes;
  final String? existingImageUrl;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = context.appColors;
    final priceDisplay = switch (pricingType) {
      'Free' => l10n.pricingFree,
      'Negotiable' => l10n.pricingNegotiable,
      _ => priceText.isEmpty ? '—' : 'BRL $priceText',
    };
    final hasLocal = photoBytes != null && photoBytes!.isNotEmpty;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.addProductReviewTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingMd),
        if (hasPhoto) ...[
          ClipRRect(
            borderRadius: BorderRadius.circular(AppConstants.radiusMd),
            child: hasLocal
                ? Image.memory(
                    photoBytes!,
                    height: 120,
                    width: double.infinity,
                    fit: BoxFit.cover,
                  )
                : Image.network(
                    existingImageUrl!,
                    height: 120,
                    width: double.infinity,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => const SizedBox.shrink(),
                  ),
          ),
          const SizedBox(height: AppConstants.spacingMd),
        ],
        ArahCard(
          child: Column(
            children: [
              _row(context, l10n.productTitleLabel, title),
              _row(
                context,
                l10n.productCategoryLabel,
                category.isEmpty ? '—' : category,
              ),
              _row(context, l10n.productPricingType, priceDisplay),
              _row(
                context,
                l10n.productPhotoLabel,
                hasPhoto
                    ? l10n.productPhotoAttached
                    : l10n.productPhotoNone,
              ),
              _row(
                context,
                l10n.descriptionLabel,
                description.isEmpty ? '—' : description,
                showDivider: false,
              ),
            ],
          ),
        ),
        if (!hasPhoto) ...[
          const SizedBox(height: AppConstants.spacingSm),
          Text(
            l10n.addProductPhotoOptionalHint,
            style: theme.textTheme.bodySmall?.copyWith(
              color: colors.onSurfaceSubtle,
            ),
          ),
        ],
      ],
    );
  }

  Widget _row(
    BuildContext context,
    String label,
    String value, {
    bool showDivider = true,
  }) {
    final colors = context.appColors;
    return DecoratedBox(
      decoration: BoxDecoration(
        border: showDivider
            ? Border(bottom: BorderSide(color: colors.outlineSubtle))
            : null,
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: AppConstants.spacingMd),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Text(
                label,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: colors.onSurfaceVariant,
                    ),
              ),
            ),
            Expanded(
              child: Text(
                value,
                textAlign: TextAlign.end,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
