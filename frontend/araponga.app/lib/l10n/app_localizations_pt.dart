// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Portuguese (`pt`).
class AppLocalizationsPt extends AppLocalizations {
  AppLocalizationsPt([String locale = 'pt']) : super(locale);

  @override
  String get appTitle => 'Ará';

  @override
  String get login => 'Entrar';

  @override
  String get loginSubtitle => 'Entre na sua conta';

  @override
  String get email => 'E-mail';

  @override
  String get emailHint => 'seu@email.com';

  @override
  String get nameOptional => 'Nome (opcional)';

  @override
  String get informEmail => 'Informe o e-mail';

  @override
  String get home => 'Início';

  @override
  String get explore => 'Explorar';

  @override
  String get post => 'Publicar';

  @override
  String get notifications => 'Notificações';

  @override
  String get profile => 'Perfil';

  @override
  String get chooseTerritory =>
      'Escolha um território para ver o feed da região';

  @override
  String get territories => 'Territórios';

  @override
  String get territoriesSubtitle =>
      'Toque em um território para ver o feed da região ou trocar de região.';

  @override
  String get noTerritoryAvailable => 'Nenhum território disponível';

  @override
  String get onboardingTitle => 'Escolha seu território';

  @override
  String get onboardingDescription =>
      'Para ver o feed e participar da comunidade, escolha um território próximo a você.';

  @override
  String get useMyLocation => 'Usar minha localização';

  @override
  String get enableLocationHint =>
      'Ative a localização para ver territórios próximos.';

  @override
  String get noTerritoryInRegion =>
      'Nenhum território encontrado nesta região. Tente aumentar o raio ou ative a localização.';

  @override
  String get onboardingNearbyTitle => 'Próximos a você';

  @override
  String get onboardingAllTerritoriesTitle => 'Todos os territórios';

  @override
  String get onboardingOrChooseFromList =>
      'Ou escolha um território na lista abaixo';

  @override
  String get onboardingLocationEnabled => 'Localização ativa';

  @override
  String get onboardingLocationPrivacy =>
      'Sua localização é privada e não fica visível para outros usuários.';

  @override
  String get onboardingAllowLocationToCenter =>
      'Permita a localização para centralizar o mapa e ver territórios próximos.';

  @override
  String onboardingContinueWith(Object name) {
    return 'Continuar com $name';
  }

  @override
  String get onboardingVisitorOnContinue =>
      'Ao continuar, você entrará como visitante neste território e poderá ver o feed da região.';

  @override
  String get onboardingGettingLocation => 'Obtendo localização...';

  @override
  String get onboardingLoadingTerritories => 'Carregando territórios...';

  @override
  String get tryAgain => 'Tentar de novo';

  @override
  String get noPostsHere => 'Nenhum post nesta região';

  @override
  String get beFirstToPost => 'Seja o primeiro a publicar aqui.';

  @override
  String get loadMore => 'Carregar mais';

  @override
  String get editProfile => 'Editar perfil';

  @override
  String get myTerritory => 'Meu território';

  @override
  String get logout => 'Sair';

  @override
  String get save => 'Salvar';

  @override
  String get name => 'Nome';

  @override
  String get bioOptional => 'Bio (opcional)';

  @override
  String get profileUpdated => 'Perfil atualizado';

  @override
  String get nameRequired => 'Nome é obrigatório';

  @override
  String get createPost => 'Publicar';

  @override
  String get postCreated => 'Post criado com sucesso.';

  @override
  String get title => 'Título';

  @override
  String get titleHint => 'Dê um título ao seu post';

  @override
  String get content => 'Conteúdo';

  @override
  String get contentHint => 'O que você quer compartilhar?';

  @override
  String get type => 'Tipo';

  @override
  String get visibility => 'Visibilidade';

  @override
  String get general => 'Geral';

  @override
  String get alert => 'Alerta';

  @override
  String get public => 'Público';

  @override
  String get residentsOnly => 'Só moradores';

  @override
  String get informTitle => 'Informe o título.';

  @override
  String get informContent => 'Informe o conteúdo.';

  @override
  String get noTerritorySelected => 'Nenhum território selecionado';

  @override
  String get chooseTerritoryInExplore =>
      'Toque em Explorar, escolha um território e volte aqui para publicar.';

  @override
  String get comingSoon => 'Em breve';

  @override
  String get errorLoad => 'Erro ao carregar.';

  @override
  String get sessionExpired => 'Sessão expirada. Faça login novamente.';

  @override
  String get enterToAccess =>
      'Entre na sua conta para acessar perfil, publicar e notificações.';

  @override
  String get map => 'Mapa';

  @override
  String get viewOnMap => 'Ver no mapa';

  @override
  String get mapEntity => 'Estabelecimento / ponto no mapa';

  @override
  String get mapPost => 'Post';

  @override
  String get mapEvent => 'Evento';

  @override
  String get mapAsset => 'Mídia / asset';

  @override
  String get mapAlert => 'Alerta';

  @override
  String get mapPin => 'Pin no mapa';

  @override
  String get noNotifications => 'Nenhuma notificação';

  @override
  String get events => 'Eventos';

  @override
  String get noEvents => 'Nenhum evento neste território';

  @override
  String get eventInterested => 'Tenho interesse';

  @override
  String get eventConfirm => 'Confirmar presença';

  @override
  String get eventConfirmed => 'Presença confirmada';

  @override
  String get myInterests => 'Meus interesses';

  @override
  String get myInterestsHint => 'Interesses ajudam a personalizar o feed.';

  @override
  String get interestTag => 'Interesse';

  @override
  String get interestAdded => 'Interesse adicionado';

  @override
  String get interestRemoved => 'Interesse removido';

  @override
  String get notificationPreferences => 'Preferências de notificação';

  @override
  String get notifPosts => 'Posts';

  @override
  String get notifComments => 'Comentários';

  @override
  String get notifEvents => 'Eventos';

  @override
  String get notifAlerts => 'Alertas';

  @override
  String get filterByInterests => 'Por interesses';

  @override
  String get inTerritory => 'Em';

  @override
  String get loginWithGoogle => 'Entrar com Google';

  @override
  String get noAccountYet => 'Não tem conta?';

  @override
  String get createAccount => 'Criar conta';

  @override
  String get loginOr => 'ou';

  @override
  String get continueButton => 'Continuar';

  @override
  String get password => 'Senha';

  @override
  String get passwordHint => 'Digite sua senha';

  @override
  String get enterPassword => 'Digite sua senha para entrar';

  @override
  String get signUp => 'Criar conta';

  @override
  String get signUpSubtitle => 'Preencha os dados para criar sua conta';

  @override
  String get confirmPassword => 'Confirmar senha';

  @override
  String get confirmPasswordHint => 'Repita a senha';

  @override
  String get displayName => 'Nome';

  @override
  String get displayNameHint => 'Como quer ser chamado';

  @override
  String get back => 'Voltar';

  @override
  String get accountCreated => 'Conta criada. Bem-vindo!';

  @override
  String get passwordMinLength => 'Mín. 6 caracteres';

  @override
  String get passwordsDontMatch => 'As senhas não coincidem';
}
