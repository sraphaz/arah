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
/// import 'gen_l10n/app_localizations.dart';
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
  AppLocalizations(String locale) : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations? of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations);
  }

  static const LocalizationsDelegate<AppLocalizations> delegate = _AppLocalizationsDelegate();

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
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates = <LocalizationsDelegate<dynamic>>[
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
  /// **'Arah'**
  String get appTitle;

  /// No description provided for @brandTagline.
  ///
  /// In pt, this message translates to:
  /// **'Território primeiro. Comunidade primeiro.'**
  String get brandTagline;

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

  /// No description provided for @chooseTerritoryFirst.
  ///
  /// In pt, this message translates to:
  /// **'Escolha um território primeiro.'**
  String get chooseTerritoryFirst;

  /// No description provided for @cancel.
  ///
  /// In pt, this message translates to:
  /// **'Cancelar'**
  String get cancel;

  /// No description provided for @create.
  ///
  /// In pt, this message translates to:
  /// **'Criar'**
  String get create;

  /// No description provided for @send.
  ///
  /// In pt, this message translates to:
  /// **'Enviar'**
  String get send;

  /// No description provided for @delete.
  ///
  /// In pt, this message translates to:
  /// **'Excluir'**
  String get delete;

  /// No description provided for @deletePost.
  ///
  /// In pt, this message translates to:
  /// **'Excluir post'**
  String get deletePost;

  /// No description provided for @deletePostConfirm.
  ///
  /// In pt, this message translates to:
  /// **'Esta ação não pode ser desfeita.'**
  String get deletePostConfirm;

  /// No description provided for @marketplace.
  ///
  /// In pt, this message translates to:
  /// **'Marketplace'**
  String get marketplace;

  /// No description provided for @chat.
  ///
  /// In pt, this message translates to:
  /// **'Chat'**
  String get chat;

  /// No description provided for @alertsTitle.
  ///
  /// In pt, this message translates to:
  /// **'Alertas'**
  String get alertsTitle;

  /// No description provided for @moderation.
  ///
  /// In pt, this message translates to:
  /// **'Moderação'**
  String get moderation;

  /// No description provided for @assetsTitle.
  ///
  /// In pt, this message translates to:
  /// **'Assets'**
  String get assetsTitle;

  /// No description provided for @membership.
  ///
  /// In pt, this message translates to:
  /// **'Membership'**
  String get membership;

  /// No description provided for @subscriptions.
  ///
  /// In pt, this message translates to:
  /// **'Assinaturas'**
  String get subscriptions;

  /// No description provided for @connections.
  ///
  /// In pt, this message translates to:
  /// **'Conexões'**
  String get connections;

  /// No description provided for @add.
  ///
  /// In pt, this message translates to:
  /// **'Adicionar'**
  String get add;

  /// No description provided for @newGroup.
  ///
  /// In pt, this message translates to:
  /// **'Novo grupo'**
  String get newGroup;

  /// No description provided for @newAsset.
  ///
  /// In pt, this message translates to:
  /// **'Novo asset'**
  String get newAsset;

  /// No description provided for @assetName.
  ///
  /// In pt, this message translates to:
  /// **'Nome'**
  String get assetName;

  /// No description provided for @assetType.
  ///
  /// In pt, this message translates to:
  /// **'Tipo'**
  String get assetType;

  /// No description provided for @validate.
  ///
  /// In pt, this message translates to:
  /// **'Validar'**
  String get validate;

  /// No description provided for @archive.
  ///
  /// In pt, this message translates to:
  /// **'Arquivar'**
  String get archive;

  /// No description provided for @approveCurator.
  ///
  /// In pt, this message translates to:
  /// **'Aprovar (curador)'**
  String get approveCurator;

  /// No description provided for @rejectCurator.
  ///
  /// In pt, this message translates to:
  /// **'Rejeitar (curador)'**
  String get rejectCurator;

  /// No description provided for @noAssetsRegistered.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum asset cadastrado.'**
  String get noAssetsRegistered;

  /// No description provided for @noItemsFound.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum item encontrado.'**
  String get noItemsFound;

  /// No description provided for @noQueueItems.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum item nesta fila.'**
  String get noQueueItems;

  /// No description provided for @noAlertsActive.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum alerta ativo'**
  String get noAlertsActive;

  /// No description provided for @reportAlert.
  ///
  /// In pt, this message translates to:
  /// **'Reportar alerta'**
  String get reportAlert;

  /// No description provided for @chooseTerritoryForAlerts.
  ///
  /// In pt, this message translates to:
  /// **'Escolha um território para ver alertas.'**
  String get chooseTerritoryForAlerts;

  /// No description provided for @pending.
  ///
  /// In pt, this message translates to:
  /// **'Pendentes'**
  String get pending;

  /// No description provided for @connect.
  ///
  /// In pt, this message translates to:
  /// **'Conectar'**
  String get connect;

  /// No description provided for @searchPeople.
  ///
  /// In pt, this message translates to:
  /// **'Buscar pessoas'**
  String get searchPeople;

  /// No description provided for @statusLabel.
  ///
  /// In pt, this message translates to:
  /// **'Status: {status}'**
  String statusLabel(String status);

  /// No description provided for @checkoutWithCount.
  ///
  /// In pt, this message translates to:
  /// **'Checkout ({count})'**
  String checkoutWithCount(int count);

  /// No description provided for @myStore.
  ///
  /// In pt, this message translates to:
  /// **'Minha loja'**
  String get myStore;

  /// No description provided for @itemsTab.
  ///
  /// In pt, this message translates to:
  /// **'Itens'**
  String get itemsTab;

  /// No description provided for @availablePlans.
  ///
  /// In pt, this message translates to:
  /// **'Planos disponíveis'**
  String get availablePlans;

  /// No description provided for @mySubscription.
  ///
  /// In pt, this message translates to:
  /// **'Minha assinatura'**
  String get mySubscription;

  /// No description provided for @subscribe.
  ///
  /// In pt, this message translates to:
  /// **'Assinar'**
  String get subscribe;

  /// No description provided for @cancelSubscription.
  ///
  /// In pt, this message translates to:
  /// **'Cancelar'**
  String get cancelSubscription;

  /// No description provided for @yourRole.
  ///
  /// In pt, this message translates to:
  /// **'Seu papel'**
  String get yourRole;

  /// No description provided for @verificationLabel.
  ///
  /// In pt, this message translates to:
  /// **'Verificação: {value}'**
  String verificationLabel(String value);

  /// No description provided for @requestResidency.
  ///
  /// In pt, this message translates to:
  /// **'Solicitar residência'**
  String get requestResidency;

  /// No description provided for @verifyByLocation.
  ///
  /// In pt, this message translates to:
  /// **'Verificar por localização'**
  String get verifyByLocation;

  /// No description provided for @channelsTab.
  ///
  /// In pt, this message translates to:
  /// **'Canais'**
  String get channelsTab;

  /// No description provided for @groupsTab.
  ///
  /// In pt, this message translates to:
  /// **'Grupos'**
  String get groupsTab;

  /// No description provided for @groupName.
  ///
  /// In pt, this message translates to:
  /// **'Nome do grupo'**
  String get groupName;

  /// No description provided for @openStreetMapAttribution.
  ///
  /// In pt, this message translates to:
  /// **'OpenStreetMap contributors'**
  String get openStreetMapAttribution;

  /// No description provided for @priceLabel.
  ///
  /// In pt, this message translates to:
  /// **'{currency} {amount}'**
  String priceLabel(String currency, String amount);

  /// No description provided for @storeAndPrice.
  ///
  /// In pt, this message translates to:
  /// **'{store} · {price}'**
  String storeAndPrice(String store, String price);

  /// No description provided for @conversationMeta.
  ///
  /// In pt, this message translates to:
  /// **'{kind} · {status}'**
  String conversationMeta(String kind, String status);

  /// No description provided for @noCommentsYet.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum comentário ainda.'**
  String get noCommentsYet;

  /// No description provided for @commentHint.
  ///
  /// In pt, this message translates to:
  /// **'Escreva um comentário'**
  String get commentHint;

  /// No description provided for @errorLoadComments.
  ///
  /// In pt, this message translates to:
  /// **'Não foi possível carregar comentários.'**
  String get errorLoadComments;

  /// No description provided for @errorComment.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao comentar.'**
  String get errorComment;

  /// No description provided for @groupCreated.
  ///
  /// In pt, this message translates to:
  /// **'Grupo criado.'**
  String get groupCreated;

  /// No description provided for @errorCreateGroup.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao criar grupo.'**
  String get errorCreateGroup;

  /// No description provided for @errorLoadConversations.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao carregar conversas.'**
  String get errorLoadConversations;

  /// No description provided for @noChannelsAvailable.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum canal disponível.'**
  String get noChannelsAvailable;

  /// No description provided for @noGroupsYet.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum grupo ainda. Toque + para criar.'**
  String get noGroupsYet;

  /// No description provided for @conversation.
  ///
  /// In pt, this message translates to:
  /// **'Conversa'**
  String get conversation;

  /// No description provided for @messageHint.
  ///
  /// In pt, this message translates to:
  /// **'Mensagem'**
  String get messageHint;

  /// No description provided for @errorSendMessage.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao enviar mensagem.'**
  String get errorSendMessage;

  /// No description provided for @searchTab.
  ///
  /// In pt, this message translates to:
  /// **'Buscar'**
  String get searchTab;

  /// No description provided for @searchItemsHint.
  ///
  /// In pt, this message translates to:
  /// **'Buscar itens'**
  String get searchItemsHint;

  /// No description provided for @orderSent.
  ///
  /// In pt, this message translates to:
  /// **'Pedido enviado.'**
  String get orderSent;

  /// No description provided for @errorCheckout.
  ///
  /// In pt, this message translates to:
  /// **'Erro no checkout.'**
  String get errorCheckout;

  /// No description provided for @errorSearchItems.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao buscar itens.'**
  String get errorSearchItems;

  /// No description provided for @addedToCart.
  ///
  /// In pt, this message translates to:
  /// **'Adicionado ao carrinho.'**
  String get addedToCart;

  /// No description provided for @errorAddToCart.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao adicionar.'**
  String get errorAddToCart;

  /// No description provided for @createMyStore.
  ///
  /// In pt, this message translates to:
  /// **'Criar minha loja'**
  String get createMyStore;

  /// No description provided for @updateStore.
  ///
  /// In pt, this message translates to:
  /// **'Atualizar loja'**
  String get updateStore;

  /// No description provided for @storeNameLabel.
  ///
  /// In pt, this message translates to:
  /// **'Nome da loja'**
  String get storeNameLabel;

  /// No description provided for @descriptionLabel.
  ///
  /// In pt, this message translates to:
  /// **'Descrição'**
  String get descriptionLabel;

  /// No description provided for @informStoreName.
  ///
  /// In pt, this message translates to:
  /// **'Informe o nome da loja.'**
  String get informStoreName;

  /// No description provided for @storeCreated.
  ///
  /// In pt, this message translates to:
  /// **'Loja criada.'**
  String get storeCreated;

  /// No description provided for @storeUpdated.
  ///
  /// In pt, this message translates to:
  /// **'Loja atualizada.'**
  String get storeUpdated;

  /// No description provided for @errorSaveStore.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao salvar loja.'**
  String get errorSaveStore;

  /// No description provided for @createStore.
  ///
  /// In pt, this message translates to:
  /// **'Criar loja'**
  String get createStore;

  /// No description provided for @saveChanges.
  ///
  /// In pt, this message translates to:
  /// **'Salvar alterações'**
  String get saveChanges;

  /// No description provided for @requestSent.
  ///
  /// In pt, this message translates to:
  /// **'Solicitação enviada.'**
  String get requestSent;

  /// No description provided for @errorSendRequest.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao enviar solicitação.'**
  String get errorSendRequest;

  /// No description provided for @errorLoadConnections.
  ///
  /// In pt, this message translates to:
  /// **'Não foi possível carregar conexões.'**
  String get errorLoadConnections;

  /// No description provided for @noConnectionsYet.
  ///
  /// In pt, this message translates to:
  /// **'Nenhuma conexão ainda. Toque em Adicionar para buscar pessoas.'**
  String get noConnectionsYet;

  /// No description provided for @errorLoadSuggestions.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao carregar sugestões.'**
  String get errorLoadSuggestions;

  /// No description provided for @errorSearch.
  ///
  /// In pt, this message translates to:
  /// **'Erro na busca.'**
  String get errorSearch;

  /// No description provided for @searchMinCharsHint.
  ///
  /// In pt, this message translates to:
  /// **'Digite ao menos 2 caracteres ou veja sugestões acima.'**
  String get searchMinCharsHint;

  /// No description provided for @connectionRequestIncoming.
  ///
  /// In pt, this message translates to:
  /// **'Solicitação recebida'**
  String get connectionRequestIncoming;

  /// No description provided for @connectionRequestOutgoing.
  ///
  /// In pt, this message translates to:
  /// **'Solicitação enviada'**
  String get connectionRequestOutgoing;

  /// No description provided for @activeConnection.
  ///
  /// In pt, this message translates to:
  /// **'Conexão ativa'**
  String get activeConnection;

  /// No description provided for @errorAccept.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao aceitar.'**
  String get errorAccept;

  /// No description provided for @errorReject.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao rejeitar.'**
  String get errorReject;

  /// No description provided for @errorRemove.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao remover.'**
  String get errorRemove;

  /// No description provided for @assetCreated.
  ///
  /// In pt, this message translates to:
  /// **'Asset criado.'**
  String get assetCreated;

  /// No description provided for @errorCreateAsset.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao criar asset.'**
  String get errorCreateAsset;

  /// No description provided for @errorLoadAssets.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao carregar assets.'**
  String get errorLoadAssets;

  /// No description provided for @validationRegistered.
  ///
  /// In pt, this message translates to:
  /// **'Validação registrada ({percent}% da comunidade).'**
  String validationRegistered(String percent);

  /// No description provided for @assetArchived.
  ///
  /// In pt, this message translates to:
  /// **'Asset arquivado.'**
  String get assetArchived;

  /// No description provided for @assetApproved.
  ///
  /// In pt, this message translates to:
  /// **'Asset aprovado.'**
  String get assetApproved;

  /// No description provided for @assetRejected.
  ///
  /// In pt, this message translates to:
  /// **'Asset rejeitado.'**
  String get assetRejected;

  /// No description provided for @errorCompleteAction.
  ///
  /// In pt, this message translates to:
  /// **'Não foi possível concluir a ação.'**
  String get errorCompleteAction;

  /// No description provided for @assetValidationsMeta.
  ///
  /// In pt, this message translates to:
  /// **'{count} validações ({percent}%)'**
  String assetValidationsMeta(int count, String percent);

  /// No description provided for @alertCreated.
  ///
  /// In pt, this message translates to:
  /// **'Alerta criado.'**
  String get alertCreated;

  /// No description provided for @errorCreateAlert.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao criar alerta.'**
  String get errorCreateAlert;

  /// No description provided for @errorLoadAlerts.
  ///
  /// In pt, this message translates to:
  /// **'Não foi possível carregar alertas.'**
  String get errorLoadAlerts;

  /// No description provided for @alertsRequireResidency.
  ///
  /// In pt, this message translates to:
  /// **'Alertas do território exigem residência ou curadoria.'**
  String get alertsRequireResidency;

  /// No description provided for @filterAll.
  ///
  /// In pt, this message translates to:
  /// **'Todos'**
  String get filterAll;

  /// No description provided for @postDefaultTitle.
  ///
  /// In pt, this message translates to:
  /// **'Post'**
  String get postDefaultTitle;

  /// No description provided for @chooseTerritoryBeforePost.
  ///
  /// In pt, this message translates to:
  /// **'Escolha um território antes de publicar.'**
  String get chooseTerritoryBeforePost;

  /// No description provided for @addImage.
  ///
  /// In pt, this message translates to:
  /// **'Adicionar imagem'**
  String get addImage;

  /// No description provided for @changeImage.
  ///
  /// In pt, this message translates to:
  /// **'Trocar imagem'**
  String get changeImage;

  /// No description provided for @moderationQueueTab.
  ///
  /// In pt, this message translates to:
  /// **'Fila'**
  String get moderationQueueTab;

  /// No description provided for @moderationCasesTab.
  ///
  /// In pt, this message translates to:
  /// **'Casos'**
  String get moderationCasesTab;

  /// No description provided for @moderationEvidencesTab.
  ///
  /// In pt, this message translates to:
  /// **'Evidências'**
  String get moderationEvidencesTab;

  /// No description provided for @noPermissionOrError.
  ///
  /// In pt, this message translates to:
  /// **'Sem permissão ou erro ao carregar.'**
  String get noPermissionOrError;

  /// No description provided for @downloadEvidenceTooltip.
  ///
  /// In pt, this message translates to:
  /// **'Baixar evidência'**
  String get downloadEvidenceTooltip;

  /// No description provided for @approveTooltip.
  ///
  /// In pt, this message translates to:
  /// **'Aprovar'**
  String get approveTooltip;

  /// No description provided for @rejectTooltip.
  ///
  /// In pt, this message translates to:
  /// **'Rejeitar'**
  String get rejectTooltip;

  /// No description provided for @decisionRegistered.
  ///
  /// In pt, this message translates to:
  /// **'Decisão registrada.'**
  String get decisionRegistered;

  /// No description provided for @errorDecideItem.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao decidir item.'**
  String get errorDecideItem;

  /// No description provided for @evidenceDownloaded.
  ///
  /// In pt, this message translates to:
  /// **'Evidência baixada ({size} bytes).'**
  String evidenceDownloaded(int size);

  /// No description provided for @errorDownloadEvidence.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao baixar evidência.'**
  String get errorDownloadEvidence;

  /// No description provided for @errorLoadMembership.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao carregar membership.'**
  String get errorLoadMembership;

  /// No description provided for @enableLocationFirst.
  ///
  /// In pt, this message translates to:
  /// **'Ative a localização primeiro.'**
  String get enableLocationFirst;

  /// No description provided for @residencyVerifiedByGeo.
  ///
  /// In pt, this message translates to:
  /// **'Residência verificada por geo.'**
  String get residencyVerifiedByGeo;

  /// No description provided for @errorResidencyVerification.
  ///
  /// In pt, this message translates to:
  /// **'Erro na verificação.'**
  String get errorResidencyVerification;

  /// No description provided for @alreadyResident.
  ///
  /// In pt, this message translates to:
  /// **'Você já é morador neste território.'**
  String get alreadyResident;

  /// No description provided for @errorLoadPlans.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao carregar planos.'**
  String get errorLoadPlans;

  /// No description provided for @subscriptionCancelled.
  ///
  /// In pt, this message translates to:
  /// **'Assinatura cancelada.'**
  String get subscriptionCancelled;

  /// No description provided for @errorCancelSubscription.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao cancelar.'**
  String get errorCancelSubscription;

  /// No description provided for @subscriptionActivated.
  ///
  /// In pt, this message translates to:
  /// **'Assinatura ativada.'**
  String get subscriptionActivated;

  /// No description provided for @errorSubscribe.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao assinar.'**
  String get errorSubscribe;

  /// No description provided for @errorRequestResidency.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao solicitar residência.'**
  String get errorRequestResidency;

  /// No description provided for @moderationEvidenceSuffix.
  ///
  /// In pt, this message translates to:
  /// **'evidência'**
  String get moderationEvidenceSuffix;

  /// No description provided for @darkMode.
  ///
  /// In pt, this message translates to:
  /// **'Modo escuro'**
  String get darkMode;

  /// No description provided for @useSystemTheme.
  ///
  /// In pt, this message translates to:
  /// **'Usar tema do sistema'**
  String get useSystemTheme;

  /// No description provided for @viewDetails.
  ///
  /// In pt, this message translates to:
  /// **'Ver detalhes'**
  String get viewDetails;

  /// No description provided for @createFirstPost.
  ///
  /// In pt, this message translates to:
  /// **'Criar primeiro post'**
  String get createFirstPost;

  /// No description provided for @appearance.
  ///
  /// In pt, this message translates to:
  /// **'Aparência'**
  String get appearance;

  /// No description provided for @governance.
  ///
  /// In pt, this message translates to:
  /// **'Governança'**
  String get governance;

  /// No description provided for @governanceSubtitle.
  ///
  /// In pt, this message translates to:
  /// **'Decisões coletivas do território.'**
  String get governanceSubtitle;

  /// No description provided for @chooseTerritoryForGovernance.
  ///
  /// In pt, this message translates to:
  /// **'Escolha um território para ver as votações.'**
  String get chooseTerritoryForGovernance;

  /// No description provided for @noVotings.
  ///
  /// In pt, this message translates to:
  /// **'Nenhuma votação no momento.'**
  String get noVotings;

  /// No description provided for @errorLoadVotings.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao carregar votações.'**
  String get errorLoadVotings;

  /// No description provided for @createVoting.
  ///
  /// In pt, this message translates to:
  /// **'Nova votação'**
  String get createVoting;

  /// No description provided for @filterAllVotings.
  ///
  /// In pt, this message translates to:
  /// **'Todas'**
  String get filterAllVotings;

  /// No description provided for @filterOpenVotings.
  ///
  /// In pt, this message translates to:
  /// **'Abertas'**
  String get filterOpenVotings;

  /// No description provided for @filterClosedVotings.
  ///
  /// In pt, this message translates to:
  /// **'Fechadas'**
  String get filterClosedVotings;

  /// No description provided for @voteAction.
  ///
  /// In pt, this message translates to:
  /// **'Votar'**
  String get voteAction;

  /// No description provided for @voteRegistered.
  ///
  /// In pt, this message translates to:
  /// **'Voto registrado.'**
  String get voteRegistered;

  /// No description provided for @errorVote.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao registrar voto.'**
  String get errorVote;

  /// No description provided for @alreadyVoted.
  ///
  /// In pt, this message translates to:
  /// **'Você já votou.'**
  String get alreadyVoted;

  /// No description provided for @viewResults.
  ///
  /// In pt, this message translates to:
  /// **'Ver resultados'**
  String get viewResults;

  /// No description provided for @closeVoting.
  ///
  /// In pt, this message translates to:
  /// **'Fechar votação'**
  String get closeVoting;

  /// No description provided for @votingClosedMsg.
  ///
  /// In pt, this message translates to:
  /// **'Votação fechada.'**
  String get votingClosedMsg;

  /// No description provided for @errorCloseVoting.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao fechar votação.'**
  String get errorCloseVoting;

  /// No description provided for @votingCreated.
  ///
  /// In pt, this message translates to:
  /// **'Votação criada.'**
  String get votingCreated;

  /// No description provided for @errorCreateVoting.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao criar votação.'**
  String get errorCreateVoting;

  /// No description provided for @votingTitleLabel.
  ///
  /// In pt, this message translates to:
  /// **'Título da votação'**
  String get votingTitleLabel;

  /// No description provided for @votingDescriptionLabel.
  ///
  /// In pt, this message translates to:
  /// **'Descrição'**
  String get votingDescriptionLabel;

  /// No description provided for @votingTypeLabel.
  ///
  /// In pt, this message translates to:
  /// **'Tipo'**
  String get votingTypeLabel;

  /// No description provided for @votingVisibilityLabel.
  ///
  /// In pt, this message translates to:
  /// **'Quem pode votar'**
  String get votingVisibilityLabel;

  /// No description provided for @votingOptionsLabel.
  ///
  /// In pt, this message translates to:
  /// **'Opções'**
  String get votingOptionsLabel;

  /// No description provided for @optionLabel.
  ///
  /// In pt, this message translates to:
  /// **'Opção'**
  String get optionLabel;

  /// No description provided for @addOption.
  ///
  /// In pt, this message translates to:
  /// **'Adicionar opção'**
  String get addOption;

  /// No description provided for @removeOption.
  ///
  /// In pt, this message translates to:
  /// **'Remover opção'**
  String get removeOption;

  /// No description provided for @votingNeedsTwoOptions.
  ///
  /// In pt, this message translates to:
  /// **'Informe pelo menos 2 opções.'**
  String get votingNeedsTwoOptions;

  /// No description provided for @requiredField.
  ///
  /// In pt, this message translates to:
  /// **'Campo obrigatório.'**
  String get requiredField;

  /// No description provided for @totalVotesLabel.
  ///
  /// In pt, this message translates to:
  /// **'{count} votos'**
  String totalVotesLabel(int count);

  /// No description provided for @statusOpen.
  ///
  /// In pt, this message translates to:
  /// **'Aberta'**
  String get statusOpen;

  /// No description provided for @statusClosed.
  ///
  /// In pt, this message translates to:
  /// **'Fechada'**
  String get statusClosed;

  /// No description provided for @statusCancelled.
  ///
  /// In pt, this message translates to:
  /// **'Cancelada'**
  String get statusCancelled;

  /// No description provided for @votingTypeThemePrioritization.
  ///
  /// In pt, this message translates to:
  /// **'Priorização de temas'**
  String get votingTypeThemePrioritization;

  /// No description provided for @votingTypeModerationRule.
  ///
  /// In pt, this message translates to:
  /// **'Regra de moderação'**
  String get votingTypeModerationRule;

  /// No description provided for @votingTypeFeatureFlag.
  ///
  /// In pt, this message translates to:
  /// **'Funcionalidade'**
  String get votingTypeFeatureFlag;

  /// No description provided for @votingTypeTerritoryCharacterization.
  ///
  /// In pt, this message translates to:
  /// **'Caracterização do território'**
  String get votingTypeTerritoryCharacterization;

  /// No description provided for @votingTypeCommunityPolicy.
  ///
  /// In pt, this message translates to:
  /// **'Política comunitária'**
  String get votingTypeCommunityPolicy;

  /// No description provided for @votingVisibilityAllMembers.
  ///
  /// In pt, this message translates to:
  /// **'Todos os membros'**
  String get votingVisibilityAllMembers;

  /// No description provided for @votingVisibilityResidentsOnly.
  ///
  /// In pt, this message translates to:
  /// **'Apenas moradores'**
  String get votingVisibilityResidentsOnly;

  /// No description provided for @votingVisibilityCuratorsOnly.
  ///
  /// In pt, this message translates to:
  /// **'Apenas curadores'**
  String get votingVisibilityCuratorsOnly;

  /// No description provided for @createEvent.
  ///
  /// In pt, this message translates to:
  /// **'Novo evento'**
  String get createEvent;

  /// No description provided for @eventTitleLabel.
  ///
  /// In pt, this message translates to:
  /// **'Título do evento'**
  String get eventTitleLabel;

  /// No description provided for @eventDescriptionLabel.
  ///
  /// In pt, this message translates to:
  /// **'Descrição'**
  String get eventDescriptionLabel;

  /// No description provided for @eventStartLabel.
  ///
  /// In pt, this message translates to:
  /// **'Início'**
  String get eventStartLabel;

  /// No description provided for @eventEndLabel.
  ///
  /// In pt, this message translates to:
  /// **'Término (opcional)'**
  String get eventEndLabel;

  /// No description provided for @eventLocationLabel.
  ///
  /// In pt, this message translates to:
  /// **'Local (opcional)'**
  String get eventLocationLabel;

  /// No description provided for @selectDateTime.
  ///
  /// In pt, this message translates to:
  /// **'Selecionar data e hora'**
  String get selectDateTime;

  /// No description provided for @eventCreated.
  ///
  /// In pt, this message translates to:
  /// **'Evento criado.'**
  String get eventCreated;

  /// No description provided for @errorCreateEvent.
  ///
  /// In pt, this message translates to:
  /// **'Erro ao criar evento.'**
  String get errorCreateEvent;

  /// No description provided for @eventStartRequired.
  ///
  /// In pt, this message translates to:
  /// **'Defina a data e hora de início.'**
  String get eventStartRequired;

  /// No description provided for @eventEndBeforeStart.
  ///
  /// In pt, this message translates to:
  /// **'O término deve ser após o início.'**
  String get eventEndBeforeStart;

  /// No description provided for @close.
  ///
  /// In pt, this message translates to:
  /// **'Fechar'**
  String get close;

  /// No description provided for @searchTerritoriesHint.
  ///
  /// In pt, this message translates to:
  /// **'Buscar território por nome ou cidade'**
  String get searchTerritoriesHint;

  /// No description provided for @noSearchResults.
  ///
  /// In pt, this message translates to:
  /// **'Nenhum território encontrado para a busca.'**
  String get noSearchResults;

  /// No description provided for @clear.
  ///
  /// In pt, this message translates to:
  /// **'Limpar'**
  String get clear;
}

class _AppLocalizationsDelegate extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) => <String>['en', 'pt'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {


  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en': return AppLocalizationsEn();
    case 'pt': return AppLocalizationsPt();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.'
  );
}
