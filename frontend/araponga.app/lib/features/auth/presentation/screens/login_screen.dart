import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/auth_state_provider.dart';

/// Passos do fluxo de login/cadastro (email-first).
enum LoginStep {
  /// Só e-mail; ao enviar chama check-email.
  email,
  /// E-mail existe: campo senha + Entrar (login dev por enquanto).
  password,
  /// E-mail não existe: formulário criar conta (email + nome).
  signup,
}

/// Tela de login: 1) e-mail → check-email → 2) senha (se existe) ou 3) criar conta (se não existe).
class LoginScreen extends ConsumerStatefulWidget {
  const LoginScreen({super.key, required this.bffBaseUrl});

  final String bffBaseUrl;

  @override
  ConsumerState<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends ConsumerState<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _displayNameController = TextEditingController();
  final _signupPasswordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();
  final _emailFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();
  final _displayNameFocusNode = FocusNode();

  LoginStep _step = LoginStep.email;
  String _email = '';
  bool _checkEmailLoading = false;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _displayNameController.dispose();
    _signupPasswordController.dispose();
    _confirmPasswordController.dispose();
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    _displayNameFocusNode.dispose();
    super.dispose();
  }

  void _goBack() {
    setState(() {
      if (_step == LoginStep.password || _step == LoginStep.signup) {
        _step = LoginStep.email;
        _email = '';
        _passwordController.clear();
        _displayNameController.clear();
        _signupPasswordController.clear();
        _confirmPasswordController.clear();
      }
    });
  }

  Future<void> _submitEmail() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final email = _emailController.text.trim();
    setState(() => _checkEmailLoading = true);
    bool exists = false;
    try {
      exists = await ref.read(authStateProvider.notifier).checkEmailExists(email);
    } catch (e) {
      if (!mounted) return;
      setState(() => _checkEmailLoading = false);
      showErrorSnackBar(
        context,
        e is ApiException ? e.userMessage : e.toString(),
      );
      return;
    }
    if (!mounted) return;
    setState(() {
      _checkEmailLoading = false;
      _email = email;
      _step = exists ? LoginStep.password : LoginStep.signup;
    });
  }

  Future<void> _submitPassword() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    await ref.read(authStateProvider.notifier).login(email: _email, password: _passwordController.text);
    if (!mounted) return;
    final auth = ref.read(authStateProvider);
    if (auth.hasError && mounted) {
      final msg = auth.error is ApiException
          ? (auth.error! as ApiException).userMessage
          : auth.error.toString();
      showErrorSnackBar(context, msg);
    } else if (auth.valueOrNull != null) {
      context.go('/onboarding');
    }
  }

  Future<void> _submitSignUp() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final displayName = _displayNameController.text.trim();
    final password = _signupPasswordController.text;
    await ref.read(authStateProvider.notifier).signUp(email: _email, displayName: displayName, password: password);
    if (!mounted) return;
    final auth = ref.read(authStateProvider);
    if (auth.hasError && mounted) {
      final msg = auth.error is ApiException
          ? (auth.error! as ApiException).userMessage
          : auth.error.toString();
      showErrorSnackBar(context, msg);
    } else if (auth.valueOrNull != null) {
      showSuccessSnackBar(context, AppLocalizations.of(context)!.accountCreated);
      context.go('/onboarding');
    }
  }

  @override
  Widget build(BuildContext context) {
    final auth = ref.watch(authStateProvider);
    final l10n = AppLocalizations.of(context)!;

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
                  if (_step != LoginStep.email)
                    Align(
                      alignment: Alignment.centerLeft,
                      child: TextButton.icon(
                        onPressed: _checkEmailLoading || auth.isLoading ? null : _goBack,
                        icon: const Icon(Icons.arrow_back, size: 20),
                        label: Text(l10n.back),
                      ),
                    ),
                  Text(
                    l10n.appTitle,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                          color: Theme.of(context).colorScheme.primary,
                          fontWeight: FontWeight.bold,
                        ),
                  ),
                  const SizedBox(height: AppConstants.spacingSm),
                  Text(
                    _step == LoginStep.email
                        ? l10n.loginSubtitle
                        : _step == LoginStep.password
                            ? l10n.enterPassword
                            : l10n.signUpSubtitle,
                    textAlign: TextAlign.center,
                    style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          color: Theme.of(context).colorScheme.onSurfaceVariant,
                        ),
                  ),
                  const SizedBox(height: AppConstants.spacingXl + AppConstants.spacingSm),
                  if (_step == LoginStep.email) ..._buildEmailStep(l10n),
                  if (_step == LoginStep.password) ..._buildPasswordStep(l10n, auth.isLoading),
                  if (_step == LoginStep.signup) ..._buildSignUpStep(l10n, auth.isLoading),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  List<Widget> _buildEmailStep(AppLocalizations l10n) {
    return [
      TextFormField(
        controller: _emailController,
        focusNode: _emailFocusNode,
        keyboardType: TextInputType.emailAddress,
        autocorrect: false,
        textInputAction: TextInputAction.done,
        onFieldSubmitted: (_) => _submitEmail(),
        decoration: InputDecoration(
          labelText: l10n.email,
          hintText: l10n.emailHint,
          border: const OutlineInputBorder(),
        ),
        validator: (v) => (v == null || v.isEmpty) ? l10n.informEmail : null,
      ),
      const SizedBox(height: AppConstants.spacingLg),
      FilledButton(
        onPressed: _checkEmailLoading ? null : _submitEmail,
        child: _checkEmailLoading
            ? const SizedBox(
                height: AppConstants.loadingIndicatorSize,
                width: AppConstants.loadingIndicatorSize,
                child: CircularProgressIndicator(strokeWidth: 2),
              )
            : Text(l10n.continueButton),
      ),
    ];
  }

  List<Widget> _buildPasswordStep(AppLocalizations l10n, bool authLoading) {
    return [
      TextFormField(
        controller: _passwordController,
        focusNode: _passwordFocusNode,
        obscureText: true,
        textInputAction: TextInputAction.done,
        onFieldSubmitted: (_) => _submitPassword(),
        decoration: InputDecoration(
          labelText: l10n.password,
          hintText: l10n.passwordHint,
          border: const OutlineInputBorder(),
        ),
        validator: (v) => (v == null || v.isEmpty) ? l10n.passwordHint : null,
      ),
      const SizedBox(height: AppConstants.spacingLg),
      FilledButton(
        onPressed: authLoading ? null : _submitPassword,
        child: authLoading
            ? const SizedBox(
                height: AppConstants.loadingIndicatorSize,
                width: AppConstants.loadingIndicatorSize,
                child: CircularProgressIndicator(strokeWidth: 2),
              )
            : Text(l10n.login),
      ),
    ];
  }

  List<Widget> _buildSignUpStep(AppLocalizations l10n, bool authLoading) {
    return [
      TextFormField(
        initialValue: _email,
        readOnly: true,
        decoration: InputDecoration(
          labelText: l10n.email,
          border: const OutlineInputBorder(),
        ),
      ),
      const SizedBox(height: AppConstants.spacingMd),
      TextFormField(
        controller: _displayNameController,
        focusNode: _displayNameFocusNode,
        textCapitalization: TextCapitalization.words,
        decoration: InputDecoration(
          labelText: l10n.displayName,
          hintText: l10n.displayNameHint,
          border: const OutlineInputBorder(),
        ),
        validator: (v) => (v == null || v.trim().isEmpty) ? l10n.nameRequired : null,
      ),
      const SizedBox(height: AppConstants.spacingMd),
      TextFormField(
        controller: _signupPasswordController,
        obscureText: true,
        decoration: InputDecoration(
          labelText: l10n.password,
          hintText: l10n.passwordHint,
          border: const OutlineInputBorder(),
        ),
        validator: (v) {
          if (v == null || v.isEmpty) return l10n.passwordHint;
          if (v.length < 6) return l10n.passwordMinLength;
          return null;
        },
      ),
      const SizedBox(height: AppConstants.spacingMd),
      TextFormField(
        controller: _confirmPasswordController,
        obscureText: true,
        textInputAction: TextInputAction.done,
        onFieldSubmitted: (_) => _submitSignUp(),
        decoration: InputDecoration(
          labelText: l10n.confirmPassword,
          hintText: l10n.confirmPasswordHint,
          border: const OutlineInputBorder(),
        ),
        validator: (v) {
          if (v == null || v.isEmpty) return l10n.confirmPasswordHint;
          if (v != _signupPasswordController.text) return l10n.passwordsDontMatch;
          return null;
        },
      ),
      const SizedBox(height: AppConstants.spacingLg),
      FilledButton(
        onPressed: authLoading ? null : _submitSignUp,
        child: authLoading
            ? const SizedBox(
                height: AppConstants.loadingIndicatorSize,
                width: AppConstants.loadingIndicatorSize,
                child: CircularProgressIndicator(strokeWidth: 2),
              )
            : Text(l10n.signUp),
      ),
    ];
  }
}
