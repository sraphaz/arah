import 'package:flutter/widgets.dart';

import '../../../../l10n/app_localizations.dart';

class LoginChoiceWebView extends StatelessWidget {
  const LoginChoiceWebView({
    super.key,
    required this.l10n,
    required this.onGoogleTap,
    required this.onEmailTap,
  });

  static const bool isSupported = false;

  final AppLocalizations l10n;
  final VoidCallback? onGoogleTap;
  final VoidCallback? onEmailTap;

  @override
  Widget build(BuildContext context) => const SizedBox.shrink();
}
