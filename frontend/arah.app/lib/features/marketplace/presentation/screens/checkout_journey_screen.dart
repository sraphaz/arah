import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/theme/arah_motion.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_card.dart';
import '../../../../core/widgets/arah_journey_shell.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/marketplace_provider.dart';

/// Jornada Mercado → PIX (APP-DS-14): sacola → recebimento → pagamento → sucesso.
class CheckoutJourneyScreen extends ConsumerStatefulWidget {
  const CheckoutJourneyScreen({super.key});

  @override
  ConsumerState<CheckoutJourneyScreen> createState() =>
      _CheckoutJourneyScreenState();
}

enum _Fulfillment { pickup, delivery }

class _CheckoutJourneyScreenState extends ConsumerState<CheckoutJourneyScreen> {
  static const int _totalSteps = 4;

  int _step = 0;
  bool _submitting = false;
  _Fulfillment _fulfillment = _Fulfillment.pickup;
  /// Pedidos já criados no checkout — retry de PIX não recria pedido.
  List<_PendingPixOrder> _pendingOrders = const [];
  String? _pixCode;
  String? _orderTotalLabel;

  void _close() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/marketplace');
    }
  }

  List<Map<String, dynamic>> _cartItems(Map<String, dynamic>? cart) {
    final items = cart?['items'];
    if (items is! List) return const [];
    return items.whereType<Map<String, dynamic>>().toList();
  }

  String _formatMoney(Object? total) {
    if (total is Map) {
      final amount = total['total'] ??
          total['amount'] ??
          total['grossAmount'] ??
          total['Total'];
      final currency =
          total['currency']?.toString() ?? total['Currency']?.toString() ?? 'BRL';
      if (amount is num) {
        return '$currency ${amount.toStringAsFixed(2)}';
      }
      if (amount != null) return '$currency $amount';
    }
    if (total is num) return 'BRL ${total.toStringAsFixed(2)}';
    return '—';
  }

  Future<void> _onPrimary() async {
    final l10n = AppLocalizations.of(context)!;
    final notifier = ref.read(marketplaceProvider.notifier);

    if (_step == 0 || _step == 1) {
      setState(() => _step += 1);
      return;
    }

    if (_step == 2) {
      setState(() => _submitting = true);
      try {
        var pending = _pendingOrders;
        if (pending.isEmpty) {
          final checkout = await notifier.checkout(
            message: _fulfillment == _Fulfillment.pickup
                ? l10n.checkoutFulfillmentPickup
                : l10n.checkoutFulfillmentDelivery,
          );
          pending = _parseCheckoutOrders(checkout, l10n);
          if (!mounted) return;
          setState(() => _pendingOrders = pending);
        }

        var working = List<_PendingPixOrder>.from(pending);
        for (var i = 0; i < working.length; i++) {
          final order = working[i];
          if (order.pixCode != null &&
              order.pixCode!.isNotEmpty &&
              order.gatewayPaymentId != null &&
              order.gatewayPaymentId!.isNotEmpty) {
            continue;
          }
          final pay = await notifier.payWithPix(order.orderId);
          final pixCode = pay['pixCopyPasteCode']?.toString();
          final gatewayId = pay['gatewayPaymentId']?.toString();
          if (pixCode == null ||
              pixCode.isEmpty ||
              gatewayId == null ||
              gatewayId.isEmpty) {
            throw ApiException(l10n.pixCodeUnavailable);
          }
          working[i] = order.copyWith(
            pixCode: pixCode,
            gatewayPaymentId: gatewayId,
          );
          if (!mounted) return;
          setState(() => _pendingOrders = List.unmodifiable(working));
        }

        if (!mounted) return;
        final codes = working
            .map((o) => o.pixCode)
            .whereType<String>()
            .where((c) => c.isNotEmpty)
            .toList();
        setState(() {
          _pendingOrders = List.unmodifiable(working);
          _pixCode = codes.join('\n\n');
          _orderTotalLabel = working.length == 1
              ? working.first.totalLabel
              : '${working.length} ${l10n.checkoutItemsCount}';
          _submitting = false;
          _step = 3;
        });
      } catch (e) {
        if (!mounted) return;
        setState(() => _submitting = false);
        showErrorSnackBar(
          context,
          e is ApiException ? e.userMessage : l10n.errorCheckout,
        );
      }
      return;
    }

    _close();
  }

  List<_PendingPixOrder> _parseCheckoutOrders(
    Map<String, dynamic> checkout,
    AppLocalizations l10n,
  ) {
    final orders = checkout['orders'];
    if (orders is! List || orders.isEmpty) {
      throw ApiException(l10n.errorCheckout);
    }
    final parsed = <_PendingPixOrder>[];
    for (final raw in orders) {
      if (raw is! Map) throw ApiException(l10n.errorCheckout);
      final orderId = raw['id']?.toString();
      if (orderId == null || orderId.isEmpty) {
        throw ApiException(l10n.errorCheckout);
      }
      parsed.add(
        _PendingPixOrder(
          orderId: orderId,
          totalLabel: _formatMoney(raw['total']),
        ),
      );
    }
    return parsed;
  }

  Future<void> _copyPix() async {
    final code = _pixCode;
    if (code == null || code.isEmpty) return;
    await Clipboard.setData(ClipboardData(text: code));
    ArahMotion.selectionTap();
    if (!mounted) return;
    showSuccessSnackBar(context, AppLocalizations.of(context)!.pixCodeCopied);
  }

  Future<void> _confirmPaid() async {
    final l10n = AppLocalizations.of(context)!;
    final pending = _pendingOrders;
    final incomplete = pending
        .where(
          (o) =>
              o.gatewayPaymentId == null || o.gatewayPaymentId!.isEmpty,
        )
        .toList();
    if (pending.isEmpty || incomplete.isNotEmpty) {
      showErrorSnackBar(context, l10n.pixPaymentPendingHint);
      return;
    }
    setState(() => _submitting = true);
    try {
      final notifier = ref.read(marketplaceProvider.notifier);
      for (final order in pending) {
        await notifier.confirmPayment(
          transactionId: order.orderId,
          gatewayPaymentId: order.gatewayPaymentId!,
        );
      }
      if (!mounted) return;
      setState(() => _submitting = false);
      showSuccessSnackBar(context, l10n.pixPaymentConfirmed);
    } catch (e) {
      if (!mounted) return;
      setState(() => _submitting = false);
      showErrorSnackBar(
        context,
        e is ApiException ? e.userMessage : l10n.pixPaymentPendingHint,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final cart = ref.watch(marketplaceProvider).cart;
    final items = _cartItems(cart);

    final String primaryLabel;
    switch (_step) {
      case 2:
        primaryLabel = l10n.checkoutPayPix;
      case 3:
        primaryLabel = l10n.checkoutDone;
      default:
        primaryLabel = l10n.continueButton;
    }

    return ArahJourneyShell(
      title: l10n.checkoutJourneyTitle,
      currentStep: _step,
      totalSteps: _totalSteps,
      onClose: _close,
      onBack: _step > 0 && _step < 3 ? () => setState(() => _step -= 1) : null,
      primaryActionLabel: primaryLabel,
      onPrimaryAction: _submitting
          ? null
          : (_step == 0 && items.isEmpty ? null : _onPrimary),
      primaryEnabled: !_submitting && (_step > 0 || items.isNotEmpty),
      primaryLoading: _submitting,
      secondaryActionLabel: _step == 3 ? l10n.pixCopyCode : null,
      onSecondaryAction: _step == 3 ? _copyPix : null,
      child: _buildStep(context, l10n, items),
    );
  }

  Widget _buildStep(
    BuildContext context,
    AppLocalizations l10n,
    List<Map<String, dynamic>> items,
  ) {
    switch (_step) {
      case 0:
        return _BagStep(l10n: l10n, items: items);
      case 1:
        return _FulfillmentStep(
          l10n: l10n,
          value: _fulfillment,
          onChanged: (v) => setState(() => _fulfillment = v),
        );
      case 2:
        return _ReviewPayStep(
          l10n: l10n,
          items: items,
          fulfillment: _fulfillment,
        );
      default:
        return _PixSuccessStep(
          l10n: l10n,
          pixCode: _pixCode,
          totalLabel: _orderTotalLabel,
          onConfirmPaid: _submitting ? null : _confirmPaid,
        );
    }
  }
}

class _PendingPixOrder {
  const _PendingPixOrder({
    required this.orderId,
    required this.totalLabel,
    this.pixCode,
    this.gatewayPaymentId,
  });

  final String orderId;
  final String totalLabel;
  final String? pixCode;
  final String? gatewayPaymentId;

  _PendingPixOrder copyWith({
    String? pixCode,
    String? gatewayPaymentId,
  }) {
    return _PendingPixOrder(
      orderId: orderId,
      totalLabel: totalLabel,
      pixCode: pixCode ?? this.pixCode,
      gatewayPaymentId: gatewayPaymentId ?? this.gatewayPaymentId,
    );
  }
}

class _BagStep extends StatelessWidget {
  const _BagStep({required this.l10n, required this.items});

  final AppLocalizations l10n;
  final List<Map<String, dynamic>> items;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    if (items.isEmpty) {
      return Text(
        l10n.checkoutEmptyCart,
        style: theme.textTheme.bodyMedium?.copyWith(color: colors.onSurfaceVariant),
      );
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.checkoutBagTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingMd),
        for (final item in items) ...[
          ArahCard(
            margin: const EdgeInsets.only(bottom: AppConstants.spacingSm),
            child: Row(
              children: [
                Icon(Icons.shopping_bag_outlined, color: colors.primary),
                const SizedBox(width: AppConstants.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        item['item']?['title']?.toString() ??
                            item['title']?.toString() ??
                            l10n.marketplace,
                        style: theme.textTheme.titleSmall,
                      ),
                      Text(
                        '${l10n.checkoutQuantity}: ${item['quantity'] ?? 1}',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: colors.onSurfaceVariant,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ],
      ],
    );
  }
}

class _FulfillmentStep extends StatelessWidget {
  const _FulfillmentStep({
    required this.l10n,
    required this.value,
    required this.onChanged,
  });

  final AppLocalizations l10n;
  final _Fulfillment value;
  final ValueChanged<_Fulfillment> onChanged;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.checkoutFulfillmentTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        Text(
          l10n.checkoutFulfillmentSubtitle,
          style: theme.textTheme.bodyMedium?.copyWith(
            color: context.appColors.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        RadioListTile<_Fulfillment>(
          value: _Fulfillment.pickup,
          groupValue: value,
          onChanged: (v) {
            if (v != null) onChanged(v);
          },
          title: Text(l10n.checkoutFulfillmentPickup),
          subtitle: Text(l10n.checkoutFulfillmentPickupHint),
        ),
        RadioListTile<_Fulfillment>(
          value: _Fulfillment.delivery,
          groupValue: value,
          onChanged: (v) {
            if (v != null) onChanged(v);
          },
          title: Text(l10n.checkoutFulfillmentDelivery),
          subtitle: Text(l10n.checkoutFulfillmentDeliveryHint),
        ),
      ],
    );
  }
}

class _ReviewPayStep extends StatelessWidget {
  const _ReviewPayStep({
    required this.l10n,
    required this.items,
    required this.fulfillment,
  });

  final AppLocalizations l10n;
  final List<Map<String, dynamic>> items;
  final _Fulfillment fulfillment;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = context.appColors;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.checkoutReviewTitle,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingMd),
        ArahCard(
          child: Column(
            children: [
              _ReviewRow(
                label: l10n.checkoutItemsCount,
                value: '${items.length}',
              ),
              _ReviewRow(
                label: l10n.checkoutFulfillmentTitle,
                value: fulfillment == _Fulfillment.pickup
                    ? l10n.checkoutFulfillmentPickup
                    : l10n.checkoutFulfillmentDelivery,
              ),
              _ReviewRow(
                label: l10n.checkoutPaymentMethod,
                value: l10n.checkoutPayPix,
                showDivider: false,
              ),
            ],
          ),
        ),
        const SizedBox(height: AppConstants.spacingMd),
        Text(
          l10n.checkoutPixHint,
          style: theme.textTheme.bodySmall?.copyWith(color: colors.onSurfaceVariant),
        ),
      ],
    );
  }
}

class _ReviewRow extends StatelessWidget {
  const _ReviewRow({
    required this.label,
    required this.value,
    this.showDivider = true,
  });

  final String label;
  final String value;
  final bool showDivider;

  @override
  Widget build(BuildContext context) {
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
          children: [
            Expanded(
              child: Text(
                label,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: colors.onSurfaceVariant,
                    ),
              ),
            ),
            Text(
              value,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
            ),
          ],
        ),
      ),
    );
  }
}

class _PixSuccessStep extends StatelessWidget {
  const _PixSuccessStep({
    required this.l10n,
    required this.pixCode,
    required this.totalLabel,
    required this.onConfirmPaid,
  });

  final AppLocalizations l10n;
  final String? pixCode;
  final String? totalLabel;
  final VoidCallback? onConfirmPaid;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final theme = Theme.of(context);
    return Column(
      children: [
        Container(
          width: 88,
          height: 88,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: colors.success.withValues(alpha: 0.12),
          ),
          child: Icon(Icons.qr_code_2, size: 48, color: colors.success),
        ),
        const SizedBox(height: AppConstants.spacingLg),
        Text(
          l10n.checkoutSuccessTitle,
          textAlign: TextAlign.center,
          style: theme.textTheme.headlineSmall?.copyWith(
            fontFamily: AppDesignTokens.fontFamilyDisplay,
            letterSpacing: AppDesignTokens.letterSpacingTight,
          ),
        ),
        const SizedBox(height: AppConstants.spacingSm),
        Text(
          l10n.checkoutSuccessMessage,
          textAlign: TextAlign.center,
          style: theme.textTheme.bodyMedium?.copyWith(
            color: colors.onSurfaceVariant,
          ),
        ),
        if (totalLabel != null) ...[
          const SizedBox(height: AppConstants.spacingMd),
          Text(
            totalLabel!,
            style: theme.textTheme.titleMedium?.copyWith(color: colors.primary),
          ),
        ],
        const SizedBox(height: AppConstants.spacingLg),
        ArahCard(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                l10n.pixPayBanner,
                style: theme.textTheme.titleSmall,
              ),
              const SizedBox(height: AppConstants.spacingSm),
              SelectableText(
                pixCode?.isNotEmpty == true ? pixCode! : l10n.pixCodeUnavailable,
                style: theme.textTheme.bodySmall?.copyWith(
                  fontFamily: AppDesignTokens.fontFamilyBody,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: AppConstants.spacingMd),
        TextButton(
          onPressed: onConfirmPaid,
          child: Text(l10n.pixAlreadyPaid),
        ),
      ],
    );
  }
}
