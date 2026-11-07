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
    required this.latitude,
    required this.longitude,
    required this.requestingLocation,
    required this.completing,
    required this.onRequestLocation,
  });

  final bool hasGeo;
  final double? latitude;
  final double? longitude;
  final bool requestingLocation;
  final bool completing;
  final Future<void> Function() onRequestLocation;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return DecoratedBox(
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: [
            Color(0xFF1C201A),
            Color(0xFF161814),
          ],
        ),
        borderRadius: BorderRadius.circular(AppDesignTokens.radiusCard),
        border: Border.all(color: Colors.white.withValues(alpha: 0.07)),
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
        child: hasGeo
            ? Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Row(
                    children: [
                      const Icon(
                        Icons.my_location,
                        color: AppDesignTokens.primary,
                        size: 20,
                      ),
                      const SizedBox(width: 11),
                      Expanded(
                        child: Text(
                          l10n.onboardingLocationEnabled,
                          style: Theme.of(context)
                              .textTheme
                              .titleSmall
                              ?.copyWith(
                                color: Theme.of(context).colorScheme.primary,
                                fontSize: 13.5,
                                fontWeight: FontWeight.w600,
                              ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 1),
                  Text(
                    latitude != null && longitude != null
                        ? '${latitude!.toStringAsFixed(3)}, ${longitude!.toStringAsFixed(3)} · privada'
                        : l10n.onboardingLocationPrivacy,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
                          fontSize: 11.5,
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
                      requestingLocation
                          ? l10n.onboardingGettingLocation
                          : l10n.useMyLocation,
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
