import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/auth_state_provider.dart';

/// Tela de login. API usa auth/social; em dev usamos provider "dev".
class LoginScreen extends ConsumerStatefulWidget {
  const LoginScreen({super.key, required this.bffBaseUrl});

  final String bffBaseUrl;

  @override
  ConsumerState<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends ConsumerState<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _nameController = TextEditingController();

  @override
  void dispose() {
    _emailController.dispose();
    _nameController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    await ref.read(authStateProvider.notifier).login(
          email: _emailController.text.trim(),
          displayName: _nameController.text.trim().isEmpty ? null : _nameController.text.trim(),
        );
    if (!mounted) return;
    final auth = ref.read(authStateProvider);
    if (auth.hasError && mounted) {
      final msg = auth.error is ApiException
          ? (auth.error! as ApiException).userMessage
          : auth.error.toString();
      showErrorSnackBar(context, msg);
    } else if (auth.valueOrNull != null) {
      context.go('/home');
    }
  }

  @override
  Widget build(BuildContext context) {
    final auth = ref.watch(authStateProvider);

    return Scaffold(
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingLg),
            child: Form(
              key: _formKey,
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    AppLocalizations.of(context)!.appTitle,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                          color: Theme.of(context).colorScheme.primary,
                          fontWeight: FontWeight.bold,
                        ),
                  ),
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    AppLocalizations.of(context)!.loginSubtitle,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
                        ),
                  ),
                  const SizedBox(height: AppConstants.spacingXl + AppConstants.spacingSm),
                  TextFormField(
                    controller: _emailController,
                    keyboardType: TextInputType.emailAddress,
                    autocorrect: false,
                    decoration: InputDecoration(
                      labelText: AppLocalizations.of(context)!.email,
                      hintText: AppLocalizations.of(context)!.emailHint,
                      border: const OutlineInputBorder(),
                    ),
                    validator: (v) =>
                        (v == null || v.isEmpty) ? AppLocalizations.of(context)!.informEmail : null,
                  ),
                  const SizedBox(height: AppConstants.spacingMd),
                  TextFormField(
                    controller: _nameController,
                    textCapitalization: TextCapitalization.words,
                    decoration: InputDecoration(
                      labelText: AppLocalizations.of(context)!.nameOptional,
                      border: const OutlineInputBorder(),
                    ),
                  ),
                  const SizedBox(height: AppConstants.spacingLg),
                  FilledButton(
                    onPressed: auth.isLoading ? null : _submit,
                    child:                     auth.isLoading
                        ? SizedBox(
                            height: AppConstants.loadingIndicatorSize,
                            width: AppConstants.loadingIndicatorSize,
                            child: const CircularProgressIndicator(strokeWidth: 2),
                          )
                        : Text(AppLocalizations.of(context)!.login),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
