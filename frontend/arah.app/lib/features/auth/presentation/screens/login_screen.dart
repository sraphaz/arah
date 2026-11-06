import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/config/constants.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/theme/app_design_tokens.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../core/widgets/arah_brand_header.dart';
import '../../../../core/widgets/arah_glass_card.dart';
import '../../../../core/widgets/arah_button.dart';
import '../../../../core/widgets/arah_scaffold.dart';
import '../../../../l10n/app_localizations.dart';
import '../providers/auth_state_provider.dart';

/// Passos do fluxo de login/cadastro (email-first).
enum LoginStep {
  /// Estado inicial: escolha entre Google e e-mail, alinhado ao UI kit.
  choice,

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

  LoginStep _step = LoginStep.choice;
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
      if (_step == LoginStep.email) {
        _step = LoginStep.choice;
        _email = '';
        _emailController.clear();
      } else if (_step == LoginStep.password || _step == LoginStep.signup) {
        _step = LoginStep.email;
        _passwordController.clear();
        _displayNameController.clear();
        _signupPasswordController.clear();
        _confirmPasswordController.clear();
      }
    });
  }

  void _openEmailStep() {
    setState(() => _step = LoginStep.email);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        _emailFocusNode.requestFocus();
      }
    });
  }

  Future<void> _submitEmail() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final email = _emailController.text.trim();
    setState(() => _checkEmailLoading = true);
    bool exists = false;
    try {
      exists =
          await ref.read(authStateProvider.notifier).checkEmailExists(email);
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
    await ref
        .read(authStateProvider.notifier)
        .login(email: _email, password: _passwordController.text);
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

  Future<void> _loginWithGoogle() async {
    await ref.read(authStateProvider.notifier).loginWithGoogle();
    if (!mounted) return;
    final auth = ref.read(authStateProvider);
    if (auth.hasError) {
      final msg = auth.error is ApiException
          ? (auth.error! as ApiException).userMessage
          : auth.error.toString();
      showErrorSnackBar(context, msg);
    } else if (auth.valueOrNull?.accessToken.isNotEmpty ?? false) {
      context.go('/onboarding');
    }
    // valueOrNull == null → usuário cancelou o seletor do Google: nenhuma ação.
  }

  Future<void> _submitSignUp() async {
    if (!(_formKey.currentState?.validate() ?? false)) return;
    final displayName = _displayNameController.text.trim();
    final password = _signupPasswordController.text;
    await ref
        .read(authStateProvider.notifier)
        .signUp(email: _email, displayName: displayName, password: password);
    if (!mounted) return;
    final auth = ref.read(authStateProvider);
    if (auth.hasError && mounted) {
      final msg = auth.error is ApiException
          ? (auth.error! as ApiException).userMessage
          : auth.error.toString();
      showErrorSnackBar(context, msg);
    } else if (auth.valueOrNull != null) {
      showSuccessSnackBar(
          context, AppLocalizations.of(context)!.accountCreated);
      context.go('/onboarding');
    }
  }

  @override
  Widget build(BuildContext context) {
    final auth = ref.watch(authStateProvider);
    final l10n = AppLocalizations.of(context)!;

    if (_step == LoginStep.choice) {
      return _buildChoiceScreen(context, l10n, auth.isLoading);
    }

    return _buildFormScreen(context, l10n, auth.isLoading);
  }

  Widget _buildChoiceScreen(
    BuildContext context,
    AppLocalizations l10n,
    bool authLoading,
  ) {
    final colors = context.appColors;

    return DecoratedBox(
      decoration: const BoxDecoration(color: AppDesignTokens.surface),
      child: Scaffold(
        backgroundColor: Colors.transparent,
        body: SizedBox.expand(
          child: Stack(
            fit: StackFit.expand,
            children: [
              const ColoredBox(color: AppDesignTokens.surface),
              Positioned(
                top: -405,
                left: -58,
                width: 507,
                height: 675,
                child: IgnorePointer(
                  child: DecoratedBox(
                    decoration: const BoxDecoration(
                      gradient: RadialGradient(
                        colors: [
                          AppDesignTokens.loginHeroGlow,
                          AppDesignTokens.surface,
                        ],
                        stops: [0.0, 0.58],
                      ),
                    ),
                  ),
                ),
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 26),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Image.asset(
                            'assets/images/arah-logo.png',
                            height: 92,
                            fit: BoxFit.fitHeight,
                            filterQuality: FilterQuality.high,
                            semanticLabel: l10n.appTitle,
                            alignment: Alignment.centerLeft,
                          ),
                          const SizedBox(height: 22),
                          Text(
                            l10n.loginEyebrow,
                            style: Theme.of(context).textTheme.labelLarge?.copyWith(
                                  color: colors.primary,
                                  fontSize: 11,
                                  fontWeight: FontWeight.w600,
                                  fontVariations: const [FontVariation('wght', 600)],
                                  letterSpacing: 1.4,
                                ),
                          ),
                          const SizedBox(height: 10),
                          Text(
                            l10n.appTitle,
                            style: Theme.of(context).textTheme.displaySmall?.copyWith(
                                  color: colors.onSurface,
                                  fontSize: 44,
                                  height: 1.02,
                                  fontWeight: FontWeight.w700,
                                  fontVariations: const [FontVariation('wght', 700)],
                                  letterSpacing: -1.2,
                                ),
                          ),
                          const SizedBox(height: 14),
                          ConstrainedBox(
                            constraints: const BoxConstraints(maxWidth: 300),
                            child: Text(
                              l10n.loginLead,
                              style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                                    color: colors.onSurfaceVariant,
                                    fontSize: 16,
                                    height: 1.5,
                                    fontVariations: const [FontVariation('wght', 400)],
                                  ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.only(bottom: 40),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          _LoginChoiceButton(
                            label: l10n.loginWithGoogle,
                          iconName: 'login',
                          iconFill: 1,
                            onPressed: authLoading ? null : _loginWithGoogle,
                            variant: _LoginChoiceButtonVariant.primary,
                          ),
                          const SizedBox(height: 11),
                          _LoginChoiceButton(
                            label: l10n.loginWithEmail,
                          iconName: 'mail',
                            onPressed: authLoading ? null : _openEmailStep,
                          ),
                          const SizedBox(height: 8),
                          GestureDetector(
                            onTap: authLoading ? null : _openEmailStep,
                            behavior: HitTestBehavior.opaque,
                            child: Center(
                              child: Text.rich(
                                TextSpan(
                                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                        fontSize: 13,
                                        color: colors.onSurfaceSubtle,
                                      fontVariations: const [FontVariation('wght', 400)],
                                      ),
                                  children: [
                                    TextSpan(text: l10n.noAccountYet),
                                    const TextSpan(text: ' '),
                                    TextSpan(
                                      text: l10n.createAccount,
                                      style: TextStyle(
                                        color: colors.primary,
                                        fontWeight: FontWeight.w600,
                                        fontVariations: const [FontVariation('wght', 600)],
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildFormScreen(
    BuildContext context,
    AppLocalizations l10n,
    bool authLoading,
  ) {
    return ArahScaffold(
      showWatermark: false,
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: AppConstants.spacingLg),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 420),
              child: Form(
                key: _formKey,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Align(
                      alignment: Alignment.centerLeft,
                      child: TextButton.icon(
                        onPressed:
                            _checkEmailLoading || authLoading ? null : _goBack,
                        icon: const Icon(Icons.arrow_back, size: 20),
                        label: Text(l10n.back),
                      ),
                    ),
                    ArahBrandHeader(
                      subtitle: _step == LoginStep.email
                          ? l10n.loginSubtitle
                          : _step == LoginStep.password
                              ? l10n.enterPassword
                              : l10n.signUpSubtitle,
                    ),
                    const SizedBox(height: AppConstants.spacing2xl),
                    ArahGlassCard(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          if (_step == LoginStep.email) ..._buildEmailStep(l10n),
                          if (_step == LoginStep.password)
                            ..._buildPasswordStep(l10n, authLoading),
                          if (_step == LoginStep.signup)
                            ..._buildSignUpStep(l10n, authLoading),
                        ],
                      ),
                    ),
                  ],
                ),
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
      ArahButton(
        label: l10n.continueButton,
        onPressed: _checkEmailLoading ? null : _submitEmail,
        loading: _checkEmailLoading,
        expand: true,
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
        validator: (v) =>
            (v == null || v.trim().isEmpty) ? l10n.nameRequired : null,
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
          if (v == null || v.isEmpty) {
            return l10n.confirmPasswordHint;
          }
          if (v != _signupPasswordController.text) {
            return l10n.passwordsDontMatch;
          }
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

enum _LoginChoiceButtonVariant { primary, secondary }

class _LoginChoiceButton extends StatelessWidget {
  const _LoginChoiceButton({
    required this.label,
    required this.iconName,
    required this.onPressed,
    this.iconFill = 0,
    this.variant = _LoginChoiceButtonVariant.secondary,
  });

  final String label;
  final String iconName;
  final int iconFill;
  final VoidCallback? onPressed;
  final _LoginChoiceButtonVariant variant;

  @override
  Widget build(BuildContext context) {
    final colors = context.appColors;
    final primary = variant == _LoginChoiceButtonVariant.primary;
    final borderColor =
        primary ? Colors.transparent : AppDesignTokens.outlineStrong;
    final shadowColor =
        primary ? AppDesignTokens.loginButtonGlow : Colors.transparent;

    return Opacity(
      opacity: onPressed == null ? 0.65 : 1,
      child: DecoratedBox(
        decoration: BoxDecoration(
          gradient: primary
              ? const LinearGradient(
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                  colors: [
                    AppDesignTokens.loginButtonGradientStart,
                    AppDesignTokens.loginButtonGradientEnd,
                  ],
                )
              : null,
          color: primary ? null : Colors.transparent,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: borderColor),
          boxShadow: primary
              ? [
                  BoxShadow(
                    color: shadowColor,
                    blurRadius: 24,
                    offset: const Offset(0, 8),
                  ),
                ]
              : null,
        ),
        child: Material(
          type: MaterialType.transparency,
          child: InkWell(
            onTap: onPressed,
            borderRadius: BorderRadius.circular(14),
            child: Padding(
              padding: const EdgeInsets.symmetric(
                vertical: 15,
                horizontal: 22,
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.center,
                mainAxisSize: MainAxisSize.min,
                children: [
                  _MaterialSymbol(
                    iconName,
                    size: 20,
                    fill: iconFill,
                    color: primary
                        ? AppDesignTokens.textOnAccent
                        : colors.onSurface,
                  ),
                  const SizedBox(width: 8),
                  Text(
                    label,
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontFamily: AppDesignTokens.fontFamilyBody,
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                          fontVariations: const [FontVariation('wght', 600)],
                          color: primary
                              ? AppDesignTokens.textOnAccent
                              : colors.onSurface,
                          letterSpacing: -0.1,
                        ),
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

class _MaterialSymbol extends StatelessWidget {
  const _MaterialSymbol(
    this.name, {
    required this.size,
    required this.color,
    this.fill = 0,
  });

  final String name;
  final double size;
  final Color color;
  final int fill;

  @override
  Widget build(BuildContext context) {
    return Text(
      name,
      strutStyle: const StrutStyle(forceStrutHeight: true, height: 1),
      style: TextStyle(
        fontFamily: 'MaterialSymbolsRounded',
        fontSize: size,
        height: 1,
        color: color,
        fontVariations: [
          FontVariation('FILL', fill.toDouble()),
          const FontVariation('wght', 500),
          const FontVariation('GRAD', 0),
          const FontVariation('opsz', 24),
        ],
      ),
    );
  }
}
