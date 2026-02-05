// Gerado manualmente. Execute `flutter gen-l10n` para regenerar a partir dos .arb.
import 'package:flutter/material.dart';
import 'package:flutter_localizations/flutter_localizations.dart';

class AppLocalizations {
  AppLocalizations(this.locale);
  final Locale locale;

  static AppLocalizations? of(BuildContext context) =>
      Localizations.of<AppLocalizations>(context, AppLocalizations);

  static const LocalizationsDelegate<AppLocalizations> delegate = _AppLocalizationsDelegate();

  static List<LocalizationsDelegate<dynamic>> get localizationsDelegates => [
    delegate,
    ...GlobalMaterialLocalizations.delegates,
    GlobalWidgetsLocalizations.delegate,
  ];

  static const List<Locale> supportedLocales = [
    Locale('pt'),
    Locale('en'),
  ];

  static const _localizedValues = <String, Map<String, String>>{
    'pt': {
      'appTitle': 'Ará',
      'login': 'Entrar',
      'loginSubtitle': 'Entre na sua conta',
      'email': 'E-mail',
      'emailHint': 'seu@email.com',
      'nameOptional': 'Nome (opcional)',
      'informEmail': 'Informe o e-mail',
      'home': 'Início',
      'explore': 'Explorar',
      'post': 'Publicar',
      'notifications': 'Notificações',
      'profile': 'Perfil',
      'chooseTerritory': 'Escolha um território para ver o feed da região',
      'territories': 'Territórios',
      'territoriesSubtitle': 'Toque em um território para ver o feed da região ou trocar de região.',
      'noTerritoryAvailable': 'Nenhum território disponível',
      'onboardingTitle': 'Escolha seu território',
      'onboardingDescription': 'Para ver o feed e participar da comunidade, escolha um território próximo a você.',
      'useMyLocation': 'Usar minha localização',
      'enableLocationHint': 'Ative a localização para ver territórios próximos.',
      'noTerritoryInRegion': 'Nenhum território encontrado nesta região. Tente aumentar o raio ou ative a localização.',
      'tryAgain': 'Tentar de novo',
      'noPostsHere': 'Nenhum post nesta região',
      'beFirstToPost': 'Seja o primeiro a publicar aqui.',
      'loadMore': 'Carregar mais',
      'editProfile': 'Editar perfil',
      'myTerritory': 'Meu território',
      'logout': 'Sair',
      'save': 'Salvar',
      'name': 'Nome',
      'bioOptional': 'Bio (opcional)',
      'profileUpdated': 'Perfil atualizado',
      'nameRequired': 'Nome é obrigatório',
      'createPost': 'Publicar',
      'postCreated': 'Post criado com sucesso.',
      'title': 'Título',
      'titleHint': 'Dê um título ao seu post',
      'content': 'Conteúdo',
      'contentHint': 'O que você quer compartilhar?',
      'type': 'Tipo',
      'visibility': 'Visibilidade',
      'general': 'Geral',
      'alert': 'Alerta',
      'public': 'Público',
      'residentsOnly': 'Só moradores',
      'informTitle': 'Informe o título.',
      'informContent': 'Informe o conteúdo.',
      'noTerritorySelected': 'Nenhum território selecionado',
      'chooseTerritoryInExplore': 'Toque em Explorar, escolha um território e volte aqui para publicar.',
      'comingSoon': 'Em breve',
      'errorLoad': 'Erro ao carregar.',
      'sessionExpired': 'Sessão expirada. Faça login novamente.',
      'enterToAccess': 'Entre na sua conta para acessar perfil, publicar e notificações.',
      'map': 'Mapa',
      'viewOnMap': 'Ver no mapa',
      'mapEntity': 'Estabelecimento / ponto no mapa',
      'mapPost': 'Post',
      'mapEvent': 'Evento',
      'mapAsset': 'Mídia / asset',
      'mapAlert': 'Alerta',
      'mapPin': 'Pin no mapa',
      'noNotifications': 'Nenhuma notificação',
      'events': 'Eventos',
      'noEvents': 'Nenhum evento neste território',
      'eventInterested': 'Tenho interesse',
      'eventConfirm': 'Confirmar presença',
      'eventConfirmed': 'Presença confirmada',
      'myInterests': 'Meus interesses',
      'myInterestsHint': 'Interesses ajudam a personalizar o feed.',
      'interestTag': 'Interesse',
      'interestAdded': 'Interesse adicionado',
      'interestRemoved': 'Interesse removido',
      'notificationPreferences': 'Preferências de notificação',
      'notifPosts': 'Posts',
      'notifComments': 'Comentários',
      'notifEvents': 'Eventos',
      'notifAlerts': 'Alertas',
      'filterByInterests': 'Por interesses',
      'inTerritory': 'Em',
    },
    'en': {
      'appTitle': 'Ará',
      'login': 'Log in',
      'loginSubtitle': 'Sign in to your account',
      'email': 'Email',
      'emailHint': 'you@email.com',
      'nameOptional': 'Name (optional)',
      'informEmail': 'Enter your email',
      'home': 'Home',
      'explore': 'Explore',
      'post': 'Post',
      'notifications': 'Notifications',
      'profile': 'Profile',
      'chooseTerritory': 'Choose a territory to see the feed',
      'territories': 'Territories',
      'territoriesSubtitle': 'Tap a territory to see its feed or switch region.',
      'noTerritoryAvailable': 'No territory available',
      'onboardingTitle': 'Choose your territory',
      'onboardingDescription': 'To see the feed and join the community, choose a territory near you.',
      'useMyLocation': 'Use my location',
      'enableLocationHint': 'Enable location to see nearby territories.',
      'noTerritoryInRegion': 'No territory found in this region. Try a larger radius or enable location.',
      'tryAgain': 'Try again',
      'noPostsHere': 'No posts in this region',
      'beFirstToPost': 'Be the first to post here.',
      'loadMore': 'Load more',
      'editProfile': 'Edit profile',
      'myTerritory': 'My territory',
      'logout': 'Log out',
      'save': 'Save',
      'name': 'Name',
      'bioOptional': 'Bio (optional)',
      'profileUpdated': 'Profile updated',
      'nameRequired': 'Name is required',
      'createPost': 'Post',
      'postCreated': 'Post created successfully.',
      'title': 'Title',
      'titleHint': 'Give your post a title',
      'content': 'Content',
      'contentHint': 'What do you want to share?',
      'type': 'Type',
      'visibility': 'Visibility',
      'general': 'General',
      'alert': 'Alert',
      'public': 'Public',
      'residentsOnly': 'Residents only',
      'informTitle': 'Enter the title.',
      'informContent': 'Enter the content.',
      'noTerritorySelected': 'No territory selected',
      'chooseTerritoryInExplore': 'Tap Explore, choose a territory, and come back here to post.',
      'comingSoon': 'Coming soon',
      'errorLoad': 'Error loading.',
      'sessionExpired': 'Session expired. Please log in again.',
      'enterToAccess': 'Sign in to access profile, post, and notifications.',
      'map': 'Map',
      'viewOnMap': 'View on map',
      'mapEntity': 'Place / map point',
      'mapPost': 'Post',
      'mapEvent': 'Event',
      'mapAsset': 'Media / asset',
      'mapAlert': 'Alert',
      'mapPin': 'Map pin',
      'noNotifications': 'No notifications',
      'events': 'Events',
      'noEvents': 'No events in this territory',
      'eventInterested': 'I\'m interested',
      'eventConfirm': 'Confirm attendance',
      'eventConfirmed': 'Attendance confirmed',
      'myInterests': 'My interests',
      'myInterestsHint': 'Interests help personalize your feed.',
      'interestTag': 'Interest',
      'interestAdded': 'Interest added',
      'interestRemoved': 'Interest removed',
      'notificationPreferences': 'Notification preferences',
      'notifPosts': 'Posts',
      'notifComments': 'Comments',
      'notifEvents': 'Events',
      'notifAlerts': 'Alerts',
      'filterByInterests': 'By interests',
      'inTerritory': 'In',
    },
  };

  String _t(String key) =>
      _localizedValues[locale.languageCode]?[key] ?? _localizedValues['pt']![key]!;

  String get appTitle => _t('appTitle');
  String get login => _t('login');
  String get loginSubtitle => _t('loginSubtitle');
  String get email => _t('email');
  String get emailHint => _t('emailHint');
  String get nameOptional => _t('nameOptional');
  String get informEmail => _t('informEmail');
  String get home => _t('home');
  String get explore => _t('explore');
  String get post => _t('post');
  String get notifications => _t('notifications');
  String get profile => _t('profile');
  String get chooseTerritory => _t('chooseTerritory');
  String get territories => _t('territories');
  String get territoriesSubtitle => _t('territoriesSubtitle');
  String get noTerritoryAvailable => _t('noTerritoryAvailable');
  String get onboardingTitle => _t('onboardingTitle');
  String get onboardingDescription => _t('onboardingDescription');
  String get useMyLocation => _t('useMyLocation');
  String get enableLocationHint => _t('enableLocationHint');
  String get noTerritoryInRegion => _t('noTerritoryInRegion');
  String get tryAgain => _t('tryAgain');
  String get noPostsHere => _t('noPostsHere');
  String get beFirstToPost => _t('beFirstToPost');
  String get loadMore => _t('loadMore');
  String get editProfile => _t('editProfile');
  String get myTerritory => _t('myTerritory');
  String get logout => _t('logout');
  String get save => _t('save');
  String get name => _t('name');
  String get bioOptional => _t('bioOptional');
  String get profileUpdated => _t('profileUpdated');
  String get nameRequired => _t('nameRequired');
  String get createPost => _t('createPost');
  String get postCreated => _t('postCreated');
  String get title => _t('title');
  String get titleHint => _t('titleHint');
  String get content => _t('content');
  String get contentHint => _t('contentHint');
  String get type => _t('type');
  String get visibility => _t('visibility');
  String get general => _t('general');
  String get alert => _t('alert');
  String get public => _t('public');
  String get residentsOnly => _t('residentsOnly');
  String get informTitle => _t('informTitle');
  String get informContent => _t('informContent');
  String get noTerritorySelected => _t('noTerritorySelected');
  String get chooseTerritoryInExplore => _t('chooseTerritoryInExplore');
  String get comingSoon => _t('comingSoon');
  String get errorLoad => _t('errorLoad');
  String get sessionExpired => _t('sessionExpired');
  String get enterToAccess => _t('enterToAccess');
  String get map => _t('map');
  String get viewOnMap => _t('viewOnMap');
  String get mapEntity => _t('mapEntity');
  String get mapPost => _t('mapPost');
  String get mapEvent => _t('mapEvent');
  String get mapAsset => _t('mapAsset');
  String get mapAlert => _t('mapAlert');
  String get mapPin => _t('mapPin');
  String get noNotifications => _t('noNotifications');
  String get events => _t('events');
  String get noEvents => _t('noEvents');
  String get eventInterested => _t('eventInterested');
  String get eventConfirm => _t('eventConfirm');
  String get eventConfirmed => _t('eventConfirmed');
  String get myInterests => _t('myInterests');
  String get myInterestsHint => _t('myInterestsHint');
  String get interestTag => _t('interestTag');
  String get interestAdded => _t('interestAdded');
  String get interestRemoved => _t('interestRemoved');
  String get notificationPreferences => _t('notificationPreferences');
  String get notifPosts => _t('notifPosts');
  String get notifComments => _t('notifComments');
  String get notifEvents => _t('notifEvents');
  String get notifAlerts => _t('notifAlerts');
  String get filterByInterests => _t('filterByInterests');
  String get inTerritory => _t('inTerritory');
}

class _AppLocalizationsDelegate extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  bool isSupported(Locale locale) =>
      ['pt', 'en'].contains(locale.languageCode);

  @override
  Future<AppLocalizations> load(Locale locale) async =>
      AppLocalizations(locale);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}
