import 'package:flutter/material.dart';
import 'package:qr_flutter/qr_flutter.dart';

import '../config/constants.dart';
import '../theme/app_design_tokens.dart';
import 'arah_card.dart';

/// Banner PIX alinhado ao kit PixPay: QR scannable + código copia-e-cola.
class ArahPixPay extends StatelessWidget {
  const ArahPixPay({
    super.key,
    required this.bannerLabel,
    required this.pixCode,
    required this.unavailableLabel,
    this.amountLabel,
  });

  final String bannerLabel;
  final String? pixCode;
  final String unavailableLabel;
  final String? amountLabel;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = context.appColors;
    final code = pixCode?.trim() ?? '';
    final hasCode = code.isNotEmpty;

    return ArahCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(bannerLabel, style: theme.textTheme.titleSmall),
          if (amountLabel != null && amountLabel!.isNotEmpty) ...[
            const SizedBox(height: AppConstants.spacingXs),
            Text(
              amountLabel!,
              style: theme.textTheme.titleMedium?.copyWith(color: colors.primary),
            ),
          ],
          const SizedBox(height: AppConstants.spacingMd),
          Center(
            child: hasCode
                ? Container(
                    padding: const EdgeInsets.all(AppConstants.spacingMd),
                    decoration: BoxDecoration(
                      color: colors.surface,
                      borderRadius:
                          BorderRadius.circular(AppConstants.radiusMd),
                      border: Border.all(color: colors.outlineSubtle),
                    ),
                    child: QrImageView(
                      data: code,
                      version: QrVersions.auto,
                      size: 200,
                      backgroundColor: colors.surface,
                      eyeStyle: QrEyeStyle(
                        eyeShape: QrEyeShape.square,
                        color: colors.onSurface,
                      ),
                      dataModuleStyle: QrDataModuleStyle(
                        dataModuleShape: QrDataModuleShape.square,
                        color: colors.onSurface,
                      ),
                    ),
                  )
                : Icon(Icons.qr_code_2, size: 88, color: colors.onSurfaceSubtle),
          ),
          const SizedBox(height: AppConstants.spacingMd),
          SelectableText(
            hasCode ? code : unavailableLabel,
            style: theme.textTheme.bodySmall?.copyWith(
              fontFamily: AppDesignTokens.fontFamilyBody,
            ),
          ),
        ],
      ),
    );
  }
}
