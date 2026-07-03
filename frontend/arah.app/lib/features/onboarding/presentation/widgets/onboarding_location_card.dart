import 'package:flutter/material.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../l10n/app_localizations.dart';

/// Bloco de localização do onboarding: mostra o estado habilitado (com aviso de
/// privacidade) ou o botão para solicitar acesso à localização.
class OnboardingLocationCard extends StatelessWidget {
  const OnboardingLocationCard({
    super.key,
    required this.hasGeo,
    required this.requestingLocation,
    required this.completing,
    required this.onRequestLocation,
  });

  final bool hasGeo;
  final bool requestingLocation;
  final bool completing;
  final Future<void> Function() onRequestLocation;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(AppConstants.spacingMd),
        child: hasGeo
            ? Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Row(
                    children: [
                      const Icon(
                        Icons.location_on,
                        color: AppDesignTokens.locationPin,
                        size: AppConstants.iconSizeLg,
                      ),
                      const SizedBox(width: AppConstants.spacingSm),
                      Expanded(
                        child: Text(
                          l10n.onboardingLocationEnabled,
                          style: Theme.of(context).textTheme.titleSmall?.copyWith(
                                color: Theme.of(context).colorScheme.primary,
                                fontWeight: FontWeight.w600,
                              ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    l10n.onboardingLocationPrivacy,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
                        ),
                  ),
                ],
              )
            : Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  OutlinedButton.icon(
                    onPressed: completing || requestingLocation
                        ? null
                        : () async {
                            await onRequestLocation();
                          },
                    icon: requestingLocation
                        ? SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Theme.of(context).colorScheme.primary,
                            ),
                          )
                        : const Icon(Icons.location_on_outlined),
                    label: Text(
                      requestingLocation ? l10n.onboardingGettingLocation : l10n.useMyLocation,
                    ),
                  ),
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    l10n.onboardingAllowLocationToCenter,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
                        ),
                  ),
                ],
              ),
      ),
    );
  }
}
