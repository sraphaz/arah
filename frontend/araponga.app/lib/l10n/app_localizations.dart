import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_en.dart';
import 'app_localizations_pt.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'l10n/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
      : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations? of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations);
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
    delegate,
    GlobalMaterialLocalizations.delegate,
    GlobalCupertinoLocalizations.delegate,
    GlobalWidgetsLocalizations.delegate,
  ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('en'),
    Locale('pt')
  ];

  /// No description provided for @appTitle.
  ///
  /// In pt, this message translates to:
  /// **'Ará'**
  String get appTitle;

  /// No description provided for @login.
  ///
  /// In pt, this message translates to:
  /// **'Entrar'**
  String get login;

  /// No description provided for @loginSubtitle.
  ///
  /// In pt, this message translates to:
  /// **'Entre na sua conta'**
  String get loginSubtitle;

  /// No description provided for @email.
  ///
  /// In pt, this message translates to:
  /// **'E-mail'**
  String get email;

  /// No description provided for @emailHint.
  ///
  /// In pt, this message translates to:
  /// **'seu@email.com'**
  String get emailHint;

  /// No description provided for @nameOptional.
  ///
  /// In pt, this message translates to:
  /// **'Nome (opcional)'**
  String get nameOptional;

  /// No description provided for @informEmail.
  ///
  /// In pt, this message translates to:
  /// **'Informe o e-mail'**
  String get informEmail;

  /// No description provided for @home.
  ///
  /// In pt, this message translates to:
  /// **'Início'**
  String get home;

  /// No description provided for @explore.
  ///
  /// In pt, this message translates to:
  /// **'Explorar'**
  String get explore;

  /// No description provided for @post.
  ///
  /// In pt, this message translates to:
  /// **'Publicar'**
  String get post;

  /// No description provided for @notifications.
  ///
  /// In pt, this message translates to:
  /// **'Notificações'**
  String get notifications;

  /// No description provided for @profile.
  ///
  /// In pt, this message translates to:
  /// **'Perfil'**
  String get profile;

  /// No description provided for @chooseTerritory.
  ///
  /// In pt, this message translates to:
  /// **'Escolha um território para ver o feed da região'**
  String get chooseTerritory;

  /// No description provided for @territories.
  ///
  /// In pt, this message translates to:
  /// **'Territórios'**
  String get territories;

  /// No description provided for @territoriesSubtitle.
  ///
  /// In pt, this message translates to:
  /// **'Toque em um território para ver o feed da região ou trocar de região.'**
  String get territoriesSubtitle;

  /// No description provided for @noTerritoryAvailable.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum território disponível'**
  String get noTerritoryAvailable;

  /// No description provided for @onboardingTitle.
  ///
  /// In pt, this message translates to:
  /// **'Escolha seu território'**
  String get onboardingTitle;

  /// No description provided for @onboardingDescription.
  ///
  /// In pt, this message translates to:
  /// **'Para ver o feed e participar da comunidade, escolha um território próximo a você.'**
  String get onboardingDescription;

  /// No description provided for @useMyLocation.
  ///
  /// In pt, this message translates to:
  /// **'Usar minha localização'**
  String get useMyLocation;

  /// No description provided for @enableLocationHint.
  ///
  /// In pt, this message translates to:
  /// **'Ative a localização para ver territórios próximos.'**
  String get enableLocationHint;

  /// No description provided for @noTerritoryInRegion.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum território encontrado nesta região. Tente aumentar o raio ou ative a localização.'**
  String get noTerritoryInRegion;

  /// No description provided for @onboardingNearbyTitle.
  ///
  /// In pt, this message translates to:
  /// **'Próximos a você'**
  String get onboardingNearbyTitle;

  /// No description provided for @onboardingAllTerritoriesTitle.
  ///
  /// In pt, this message translates to:
  /// **'Todos os territórios'**
  String get onboardingAllTerritoriesTitle;

  /// No description provided for @onboardingOrChooseFromList.
  ///
  /// In pt, this message translates to:
  /// **'Ou escolha um território na lista abaixo'**
  String get onboardingOrChooseFromList;

  /// No description provided for @onboardingLocationEnabled.
  ///
  /// In pt, this message translates to:
  /// **'Localização ativa'**
  String get onboardingLocationEnabled;

  /// No description provided for @onboardingLocationPrivacy.
  ///
  /// In pt, this message translates to:
  /// **'Sua localização é privada e não fica visível para outros usuários.'**
  String get onboardingLocationPrivacy;

  /// No description provided for @onboardingAllowLocationToCenter.
  ///
  /// In pt, this message translates to:
  /// **'Permita a localização para centralizar o mapa e ver territórios próximos.'**
  String get onboardingAllowLocationToCenter;

  /// No description provided for @onboardingContinueWith.
  ///
  /// In pt, this message translates to:
  /// **'Continuar com {name}'**
  String onboardingContinueWith(Object name);

  /// No description provided for @onboardingVisitorOnContinue.
  ///
  /// In pt, this message translates to:
  /// **'Ao continuar, você entrará como visitante neste território e poderá ver o feed da região.'**
  String get onboardingVisitorOnContinue;

  /// No description provided for @onboardingGettingLocation.
  ///
  /// In pt, this message translates to:
  /// **'Obtendo localização...'**
  String get onboardingGettingLocation;

  /// No description provided for @onboardingLoadingTerritories.
  ///
  /// In pt, this message translates to:
  /// **'Carregando territórios...'**
  String get onboardingLoadingTerritories;

  /// No description provided for @tryAgain.
  ///
  /// In pt, this message translates to:
  /// **'Tentar de novo'**
  String get tryAgain;

  /// No description provided for @noPostsHere.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum post nesta região'**
  String get noPostsHere;

  /// No description provided for @beFirstToPost.
  ///
  /// In pt, this message translates to:
  /// **'Seja o primeiro a publicar aqui.'**
  String get beFirstToPost;

  /// No description provided for @loadMore.
  ///
  /// In pt, this message translates to:
  /// **'Carregar mais'**
  String get loadMore;

  /// No description provided for @editProfile.
  ///
  /// In pt, this message translates to:
  /// **'Editar perfil'**
  String get editProfile;

  /// No description provided for @myTerritory.
  ///
  /// In pt, this message translates to:
  /// **'Meu território'**
  String get myTerritory;

  /// No description provided for @logout.
  ///
  /// In pt, this message translates to:
  /// **'Sair'**
  String get logout;

  /// No description provided for @save.
  ///
  /// In pt, this message translates to:
  /// **'Salvar'**
  String get save;

  /// No description provided for @name.
  ///
  /// In pt, this message translates to:
  /// **'Nome'**
  String get name;

  /// No description provided for @bioOptional.
  ///
  /// In pt, this message translates to:
  /// **'Bio (opcional)'**
  String get bioOptional;

  /// No description provided for @profileUpdated.
  ///
  /// In pt, this message translates to:
  /// **'Perfil atualizado'**
  String get profileUpdated;

  /// No description provided for @nameRequired.
  ///
  /// In pt, this message translates to:
  /// **'Nome é obrigatório'**
  String get nameRequired;

  /// No description provided for @createPost.
  ///
  /// In pt, this message translates to:
  /// **'Publicar'**
  String get createPost;

  /// No description provided for @postCreated.
  ///
  /// In pt, this message translates to:
  /// **'Post criado com sucesso.'**
  String get postCreated;

  /// No description provided for @title.
  ///
  /// In pt, this message translates to:
  /// **'Título'**
  String get title;

  /// No description provided for @titleHint.
  ///
  /// In pt, this message translates to:
  /// **'Dê um título ao seu post'**
  String get titleHint;

  /// No description provided for @content.
  ///
  /// In pt, this message translates to:
  /// **'Conteúdo'**
  String get content;

  /// No description provided for @contentHint.
  ///
  /// In pt, this message translates to:
  /// **'O que você quer compartilhar?'**
  String get contentHint;

  /// No description provided for @type.
  ///
  /// In pt, this message translates to:
  /// **'Tipo'**
  String get type;

  /// No description provided for @visibility.
  ///
  /// In pt, this message translates to:
  /// **'Visibilidade'**
  String get visibility;

  /// No description provided for @general.
  ///
  /// In pt, this message translates to:
  /// **'Geral'**
  String get general;

  /// No description provided for @alert.
  ///
  /// In pt, this message translates to:
  /// **'Alerta'**
  String get alert;

  /// No description provided for @public.
  ///
  /// In pt, this message translates to:
  /// **'Público'**
  String get public;

  /// No description provided for @residentsOnly.
  ///
  /// In pt, this message translates to:
  /// **'Só moradores'**
  String get residentsOnly;

  /// No description provided for @informTitle.
  ///
  /// In pt, this message translates to:
  /// **'Informe o título.'**
  String get informTitle;

  /// No description provided for @informContent.
  ///
  /// In pt, this message translates to:
  /// **'Informe o conteúdo.'**
  String get informContent;

  /// No description provided for @noTerritorySelected.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum território selecionado'**
  String get noTerritorySelected;

  /// No description provided for @chooseTerritoryInExplore.
  ///
  /// In pt, this message translates to:
  /// **'Toque em Explorar, escolha um território e volte aqui para publicar.'**
  String get chooseTerritoryInExplore;

  /// No description provided for @comingSoon.
  ///
  /// In pt, this message translates to:
  /// **'Em breve'**
  String get comingSoon;

  /// No description provided for @errorLoad.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao carregar.'**
  String get errorLoad;

  /// No description provided for @sessionExpired.
  ///
  /// In pt, this message translates to:
  /// **'Sessão expirada. Faça login novamente.'**
  String get sessionExpired;

  /// No description provided for @enterToAccess.
  ///
  /// In pt, this message translates to:
  /// **'Entre na sua conta para acessar perfil, publicar e notificações.'**
  String get enterToAccess;

  /// No description provided for @map.
  ///
  /// In pt, this message translates to:
  /// **'Mapa'**
  String get map;

  /// No description provided for @viewOnMap.
  ///
  /// In pt, this message translates to:
  /// **'Ver no mapa'**
  String get viewOnMap;

  /// No description provided for @mapEntity.
  ///
  /// In pt, this message translates to:
  /// **'Estabelecimento / ponto no mapa'**
  String get mapEntity;

  /// No description provided for @mapPost.
  ///
  /// In pt, this message translates to:
  /// **'Post'**
  String get mapPost;

  /// No description provided for @mapEvent.
  ///
  /// In pt, this message translates to:
  /// **'Evento'**
  String get mapEvent;

  /// No description provided for @mapAsset.
  ///
  /// In pt, this message translates to:
  /// **'Mídia / asset'**
  String get mapAsset;

  /// No description provided for @mapAlert.
  ///
  /// In pt, this message translates to:
  /// **'Alerta'**
  String get mapAlert;

  /// No description provided for @mapPin.
  ///
  /// In pt, this message translates to:
  /// **'Pin no mapa'**
  String get mapPin;

  /// No description provided for @noNotifications.
  ///
  /// In pt, this message translates to:
  /// **'Nenhuma notificação'**
  String get noNotifications;

  /// No description provided for @events.
  ///
  /// In pt, this message translates to:
  /// **'Eventos'**
  String get events;

  /// No description provided for @noEvents.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum evento neste território'**
  String get noEvents;

  /// No description provided for @eventInterested.
  ///
  /// In pt, this message translates to:
  /// **'Tenho interesse'**
  String get eventInterested;

  /// No description provided for @eventConfirm.
  ///
  /// In pt, this message translates to:
  /// **'Confirmar presença'**
  String get eventConfirm;

  /// No description provided for @eventConfirmed.
  ///
  /// In pt, this message translates to:
  /// **'Presença confirmada'**
  String get eventConfirmed;

  /// No description provided for @myInterests.
  ///
  /// In pt, this message translates to:
  /// **'Meus interesses'**
  String get myInterests;

  /// No description provided for @myInterestsHint.
  ///
  /// In pt, this message translates to:
  /// **'Interesses ajudam a personalizar o feed.'**
  String get myInterestsHint;

  /// No description provided for @interestTag.
  ///
  /// In pt, this message translates to:
  /// **'Interesse'**
  String get interestTag;

  /// No description provided for @interestAdded.
  ///
  /// In pt, this message translates to:
  /// **'Interesse adicionado'**
  String get interestAdded;

  /// No description provided for @interestRemoved.
  ///
  /// In pt, this message translates to:
  /// **'Interesse removido'**
  String get interestRemoved;

  /// No description provided for @notificationPreferences.
  ///
  /// In pt, this message translates to:
  /// **'Preferências de notificação'**
  String get notificationPreferences;

  /// No description provided for @notifPosts.
  ///
  /// In pt, this message translates to:
  /// **'Posts'**
  String get notifPosts;

  /// No description provided for @notifComments.
  ///
  /// In pt, this message translates to:
  /// **'Comentários'**
  String get notifComments;

  /// No description provided for @notifEvents.
  ///
  /// In pt, this message translates to:
  /// **'Eventos'**
  String get notifEvents;

  /// No description provided for @notifAlerts.
  ///
  /// In pt, this message translates to:
  /// **'Alertas'**
  String get notifAlerts;

  /// No description provided for @filterByInterests.
  ///
  /// In pt, this message translates to:
  /// **'Por interesses'**
  String get filterByInterests;

  /// No description provided for @inTerritory.
  ///
  /// In pt, this message translates to:
  /// **'Em'**
  String get inTerritory;

  /// No description provided for @loginWithGoogle.
  ///
  /// In pt, this message translates to:
  /// **'Entrar com Google'**
  String get loginWithGoogle;

  /// No description provided for @noAccountYet.
  ///
  /// In pt, this message translates to:
  /// **'Não tem conta?'**
  String get noAccountYet;

  /// No description provided for @createAccount.
  ///
  /// In pt, this message translates to:
  /// **'Criar conta'**
  String get createAccount;

  /// No description provided for @loginOr.
  ///
  /// In pt, this message translates to:
  /// **'ou'**
  String get loginOr;

  /// No description provided for @continueButton.
  ///
  /// In pt, this message translates to:
  /// **'Continuar'**
  String get continueButton;

  /// No description provided for @password.
  ///
  /// In pt, this message translates to:
  /// **'Senha'**
  String get password;

  /// No description provided for @passwordHint.
  ///
  /// In pt, this message translates to:
  /// **'Digite sua senha'**
  String get passwordHint;

  /// No description provided for @enterPassword.
  ///
  /// In pt, this message translates to:
  /// **'Digite sua senha para entrar'**
  String get enterPassword;

  /// No description provided for @signUp.
  ///
  /// In pt, this message translates to:
  /// **'Criar conta'**
  String get signUp;

  /// No description provided for @signUpSubtitle.
  ///
  /// In pt, this message translates to:
  /// **'Preencha os dados para criar sua conta'**
  String get signUpSubtitle;

  /// No description provided for @confirmPassword.
  ///
  /// In pt, this message translates to:
  /// **'Confirmar senha'**
  String get confirmPassword;

  /// No description provided for @confirmPasswordHint.
  ///
  /// In pt, this message translates to:
  /// **'Repita a senha'**
  String get confirmPasswordHint;

  /// No description provided for @displayName.
  ///
  /// In pt, this message translates to:
  /// **'Nome'**
  String get displayName;

  /// No description provided for @displayNameHint.
  ///
  /// In pt, this message translates to:
  /// **'Como quer ser chamado'**
  String get displayNameHint;

  /// No description provided for @back.
  ///
  /// In pt, this message translates to:
  /// **'Voltar'**
  String get back;

  /// No description provided for @accountCreated.
  ///
  /// In pt, this message translates to:
  /// **'Conta criada. Bem-vindo!'**
  String get accountCreated;

  /// No description provided for @passwordMinLength.
  ///
  /// In pt, this message translates to:
  /// **'Mín. 6 caracteres'**
  String get passwordMinLength;

  /// No description provided for @passwordsDontMatch.
  ///
  /// In pt, this message translates to:
  /// **'As senhas não coincidem'**
  String get passwordsDontMatch;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['en', 'pt'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en':
      return AppLocalizationsEn();
    case 'pt':
      return AppLocalizationsPt();
  }

  throw FlutterError(
      'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
      'an issue with the localizations generation tool. Please file an issue '
      'on GitHub with a reproducible sample app and the gen-l10n configuration '
      'that was used.');
}
