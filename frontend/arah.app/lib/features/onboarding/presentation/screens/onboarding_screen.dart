import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/geo/geo_location_provider.dart';
import '../../../../core/network/api_exception.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../../../core/widgets/app_snackbar.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../auth/presentation/providers/auth_state_provider.dart';
import '../../data/models/onboarding_models.dart';
import '../providers/onboarding_providers.dart';
import '../widgets/onboarding_body.dart';

/// Onboarding: localização → mapa + territórios próximos → seleção → Continuar → /home.
///
/// Exibida apenas após login/cadastro quando **não há preferência de território salva**.
/// Se já houvesse território selecionado, o app iria direto para o feed (sem pedir email, senha ou seleção).
/// O usuário **só se torna visitante** do território ao tocar em "Continuar" nesta tela (complete onboarding).
/// Por enquanto exibe apenas a lista de territórios próximos (suggested-territories).
class OnboardingScreen extends ConsumerStatefulWidget {
  const OnboardingScreen({super.key});

  @override
  ConsumerState<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends ConsumerState<OnboardingScreen> {
  bool _completing = false;
  bool _locationRequested = false;
  bool _requestingLocation = false;
  /// Território escolhido na lista; null = usar o mais próximo. Só altera mapa e botão Continuar; não completa onboarding.
  String? _selectedTerritoryId;
  /// Território cadastrado/proposto localmente (IBGE ou desenho Pending) — pode não aparecer em suggested-territories.
  TerritorySuggestion? _provisionedTerritory;

  @override
  void initState() {
    super.initState();
    _requestLocationOnce();
  }

  Future<void> _requestLocationOnce() async {
    if (_locationRequested) return;
    _locationRequested = true;
    await ref.read(geoLocationStateProvider.notifier).fetch();
    if (mounted) setState(() {});
  }

  Future<void> _requestLocationAndRefresh() async {
    if (_requestingLocation) return;
    setState(() => _requestingLocation = true);
    await ref.read(geoLocationStateProvider.notifier).fetch();
    if (!mounted) return;
    setState(() => _requestingLocation = false);
    // Forçar rebuild para o ícone/estado de localização atualizar logo após a permissão (evita ter de clicar de novo).
    setState(() {});
  }

  Future<void> _completeWithId(String territoryId) async {
    if (_completing) return;
    setState(() => _completing = true);
    try {
      final repo = ref.read(onboardingRepositoryProvider);
      await repo.completeOnboarding(territoryId);
      if (!mounted) return;
      await ref.read(selectedTerritoryIdProvider.notifier).setTerritoryId(territoryId);
      if (!mounted) return;
      context.go('/home');
    } on ApiException catch (e) {
      if (mounted) showErrorSnackBar(context, e.userMessage);
    } catch (e) {
      if (mounted) showErrorSnackBar(context, e.toString());
    } finally {
      if (mounted) setState(() => _completing = false);
    }
  }

  Future<void> _completeWith(TerritorySuggestion territory) async {
    await _completeWithId(territory.id);
  }

  /// Volta para a tela de login/cadastro (faz logout). O router observa authStateProvider;
  /// ao ficar sem token, o redirect envia para /login. Não chamar context.go para não
  /// disputar com o redirect (que poderia ainda ver token antigo).
  Future<void> _goBackToLogin(BuildContext context) async {
    await ref.read(selectedTerritoryIdProvider.notifier).setTerritoryId(null);
    await ref.read(authStateProvider.notifier).logout();
    // Navega na próxima frame para o estado de auth já estar atualizado no router.
    if (!context.mounted) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!context.mounted) return;
      context.go('/login');
    });
  }

  @override
  Widget build(BuildContext context) {
    final geo = ref.watch(geoLocationStateProvider);
    final hasGeo = geo != null;
    final l10n = AppLocalizations.of(context)!;
    final suggestedAsync = hasGeo
        ? ref.watch(suggestedTerritoriesProvider((lat: geo.latitude, lng: geo.longitude)))
        : const AsyncValue<List<TerritorySuggestion>>.data([]);
    final nearestSuggestion = suggestedAsync.valueOrNull?.isNotEmpty == true
        ? suggestedAsync.valueOrNull!.first
        : null;
    // Seleção inicial: mostrar o mais próximo como selecionado (mapa + destaque na lista)
    if (_selectedTerritoryId == null && nearestSuggestion != null) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted && _selectedTerritoryId == null) {
          setState(() => _selectedTerritoryId = nearestSuggestion.id);
        }
      });
    }

    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => _goBackToLogin(context),
          tooltip: l10n.back,
        ),
        title: Text(l10n.onboardingTitle),
        centerTitle: true,
      ),
      body: SafeArea(
        child: OnboardingBody(
          latitude: geo?.latitude,
          longitude: geo?.longitude,
          suggestedAsync: suggestedAsync,
          nearestSuggestion: nearestSuggestion,
          selectedTerritoryId: _selectedTerritoryId,
          provisionedTerritory: _provisionedTerritory,
          completing: _completing,
          requestingLocation: _requestingLocation,
          onRequestLocation: _requestLocationAndRefresh,
          onCompleteWith: _completeWith,
          onSelectTerritory: (t) => setState(() => _selectedTerritoryId = t.id),
          onTerritoryProvisioned: (t) => setState(() {
            _provisionedTerritory = t;
            _selectedTerritoryId = t.id;
          }),
        ),
      ),
    );
  }
}
