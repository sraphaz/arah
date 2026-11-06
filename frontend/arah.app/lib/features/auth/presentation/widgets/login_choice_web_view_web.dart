// ignore_for_file: avoid_web_libraries_in_flutter

import 'dart:html' as html;

import 'package:flutter/widgets.dart';

import '../../../../l10n/app_localizations.dart';

class LoginChoiceWebView extends StatefulWidget {
  const LoginChoiceWebView({
    super.key,
    required this.l10n,
    required this.onGoogleTap,
    required this.onEmailTap,
  });

  static const bool isSupported = true;
  static int _nextViewId = 0;

  final AppLocalizations l10n;
  final VoidCallback? onGoogleTap;
  final VoidCallback? onEmailTap;

  @override
  State<LoginChoiceWebView> createState() => _LoginChoiceWebViewState();
}

class _LoginChoiceWebViewState extends State<LoginChoiceWebView> {
  late final String _rootId =
      'arah-login-choice-${LoginChoiceWebView._nextViewId++}';
  late final html.DivElement _root = _createRoot();
  late final html.ButtonElement _googleButton;
  late final html.ButtonElement _emailButton;
  late final html.SpanElement _eyebrow;
  late final html.HeadingElement _title;
  late final html.ParagraphElement _lead;
  late final html.SpanElement _footerLead;
  late final html.SpanElement _footerAction;
  late final html.SpanElement _googleLabel;
  late final html.SpanElement _emailLabel;

  @override
  void initState() {
    super.initState();
    _ensureMaterialSymbolsLink();
    html.document.body?.append(_root);
    _syncText();
    _syncEnabledState();
  }

  @override
  void dispose() {
    _root.remove();
    super.dispose();
  }

  @override
  void didUpdateWidget(covariant LoginChoiceWebView oldWidget) {
    super.didUpdateWidget(oldWidget);
    _syncText();
    _syncEnabledState();
  }

  html.DivElement _createRoot() {
    final root = html.DivElement()
      ..id = _rootId
      ..classes.add('arah-login-choice')
      ..setInnerHtml(_markup, treeSanitizer: html.NodeTreeSanitizer.trusted);

    _googleButton =
        root.querySelector('[data-role="google-button"]')! as html.ButtonElement;
    _emailButton =
        root.querySelector('[data-role="email-button"]')! as html.ButtonElement;
    _eyebrow =
        root.querySelector('[data-role="eyebrow"]')! as html.SpanElement;
    _title = root.querySelector('[data-role="title"]')! as html.HeadingElement;
    _lead = root.querySelector('[data-role="lead"]')! as html.ParagraphElement;
    _footerLead =
        root.querySelector('[data-role="footer-lead"]')! as html.SpanElement;
    _footerAction =
        root.querySelector('[data-role="footer-action"]')! as html.SpanElement;
    _googleLabel =
        root.querySelector('[data-role="google-label"]')! as html.SpanElement;
    _emailLabel =
        root.querySelector('[data-role="email-label"]')! as html.SpanElement;

    _googleButton.onClick.listen((_) => widget.onGoogleTap?.call());
    _emailButton.onClick.listen((_) => widget.onEmailTap?.call());
    _footerAction.onClick.listen((_) => widget.onEmailTap?.call());

    return root;
  }

  void _syncText() {
    _eyebrow.text = widget.l10n.loginEyebrow;
    _title.text = widget.l10n.appTitle;
    _lead.text = widget.l10n.loginLead;
    _googleLabel.text = widget.l10n.loginWithGoogle;
    _emailLabel.text = widget.l10n.loginWithEmail;
    _footerLead.text = '${widget.l10n.noAccountYet} ';
    _footerAction.text = widget.l10n.createAccount;
  }

  void _syncEnabledState() {
    _googleButton.disabled = widget.onGoogleTap == null;
    _emailButton.disabled = widget.onEmailTap == null;

    final footerDisabled = widget.onEmailTap == null;
    _footerAction.style.pointerEvents = footerDisabled ? 'none' : 'auto';
    _footerAction.style.opacity = footerDisabled ? '0.65' : '1';
  }

  void _ensureMaterialSymbolsLink() {
    final head = html.document.head;
    if (head == null ||
        head.querySelector('link[data-arah-material-symbols="true"]') != null) {
      return;
    }

    final link = html.LinkElement()
      ..rel = 'stylesheet'
      ..href =
          'https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200&display=swap'
      ..setAttribute('data-arah-material-symbols', 'true');
    head.append(link);
  }

  @override
  Widget build(BuildContext context) {
    return const SizedBox.expand();
  }
}

const String _markup = '''
<style>
  @font-face {
    font-family: "Geist";
    src: url("assets/assets/fonts/Geist-Variable.ttf") format("truetype");
    font-weight: 100 900;
    font-display: block;
  }

  @font-face {
    font-family: "Sora";
    src: url("assets/assets/fonts/Sora-Variable.ttf") format("truetype");
    font-weight: 100 800;
    font-display: block;
  }

  .arah-login-choice,
  .arah-login-choice * {
    box-sizing: border-box;
  }

  .arah-login-choice {
    position: fixed;
    inset: 0;
    z-index: 2147483000;
    width: 100vw;
    height: 100vh;
    background: #0B0C0A;
    color: #F2F4EE;
    overflow: hidden;
  }

  .arah-login-choice__screen {
    height: 100%;
    display: flex;
    flex-direction: column;
    padding: 0 26px;
    background: #0B0C0A;
  }

  .arah-login-choice__hero {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

  .arah-login-choice__logo {
    width: 92px;
    margin: 0 0 22px 0;
  }

  .arah-login-choice__logo img {
    width: 92px;
    height: 92px;
    display: block;
  }

  .arah-login-choice__eyebrow {
    font-family: "Geist", system-ui, sans-serif;
    font-size: 11px;
    font-weight: 600;
    letter-spacing: 1.4px;
    text-transform: uppercase;
    color: #A6D6B9;
  }

  .arah-login-choice__title {
    margin: 10px 0 0;
    font-family: "Sora", system-ui, sans-serif;
    font-weight: 700;
    font-size: 44px;
    line-height: 1.02;
    color: #F2F4EE;
    letter-spacing: -1.2px;
  }

  .arah-login-choice__lead {
    margin: 14px 0 0;
    max-width: 300px;
    font-family: "Geist", system-ui, sans-serif;
    font-size: 16px;
    line-height: 1.5;
    color: #A6AC9E;
  }

  .arah-login-choice__actions {
    padding-bottom: 40px;
    display: flex;
    flex-direction: column;
    gap: 11px;
  }

  .arah-login-choice__button {
    appearance: none;
    border: none;
    border-radius: 14px;
    width: 100%;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 8px;
    padding: 15px 22px;
    background: transparent;
    color: #F2F4EE;
    font-family: "Geist", system-ui, sans-serif;
    font-size: 16px;
    font-weight: 600;
    letter-spacing: -0.1px;
    cursor: pointer;
    -webkit-tap-highlight-color: transparent;
  }

  .arah-login-choice__button:disabled {
    opacity: 0.65;
    cursor: default;
  }

  .arah-login-choice__button--primary {
    background: linear-gradient(150deg, #93D29C 0%, #6FB98C 100%);
    color: #0C1B10;
    box-shadow: 0 8px 24px rgba(129, 199, 132, 0.30);
  }

  .arah-login-choice__button--secondary {
    border: 1px solid rgba(255, 255, 255, 0.13);
  }

  .arah-login-choice__icon {
    font-family: "Material Symbols Rounded";
    font-size: 20px;
    line-height: 1;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    font-variation-settings: "FILL" 0, "wght" 500, "GRAD" 0, "opsz" 24;
  }

  .arah-login-choice__icon--fill {
    font-variation-settings: "FILL" 1, "wght" 500, "GRAD" 0, "opsz" 24;
  }

  .arah-login-choice__footer {
    margin-top: 8px;
    text-align: center;
    font-family: "Geist", system-ui, sans-serif;
    font-size: 13px;
    color: #6B7164;
  }

  .arah-login-choice__footer-action {
    color: #A6D6B9;
    font-weight: 600;
    cursor: pointer;
  }
</style>
<div class="arah-login-choice__screen">
  <div class="arah-login-choice__hero">
    <div class="arah-login-choice__logo">
      <img src="assets/assets/images/arah-logo.png" alt="Arah" />
    </div>
    <span class="arah-login-choice__eyebrow" data-role="eyebrow"></span>
    <h1 class="arah-login-choice__title" data-role="title"></h1>
    <p class="arah-login-choice__lead" data-role="lead"></p>
  </div>
  <div class="arah-login-choice__actions">
    <button type="button" class="arah-login-choice__button arah-login-choice__button--primary" data-role="google-button">
      <span class="arah-login-choice__icon arah-login-choice__icon--fill">login</span>
      <span data-role="google-label"></span>
    </button>
    <button type="button" class="arah-login-choice__button arah-login-choice__button--secondary" data-role="email-button">
      <span class="arah-login-choice__icon">mail</span>
      <span data-role="email-label"></span>
    </button>
    <div class="arah-login-choice__footer">
      <span data-role="footer-lead"></span><span class="arah-login-choice__footer-action" data-role="footer-action"></span>
    </div>
  </div>
</div>
''';
