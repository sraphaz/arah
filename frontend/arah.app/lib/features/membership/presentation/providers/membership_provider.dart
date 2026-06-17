import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../../../core/providers/territory_provider.dart';
import '../../data/models/membership_info.dart';
import '../../data/repositories/membership_repository.dart';

class MembershipState {
  const MembershipState({this.membership, this.isLoading = false, this.error});

  final MembershipInfo? membership;
  final bool isLoading;
  final Object? error;
}

class MembershipNotifier extends StateNotifier<MembershipState> {
  MembershipNotifier(this._ref) : super(const MembershipState()) {
    refresh();
  }

  final Ref _ref;

  MembershipRepository get _repo => MembershipRepository(client: _ref.read(bffClientProvider));

  Future<void> refresh() async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) {
      state = const MembershipState();
      return;
    }
    state = MembershipState(membership: state.membership, isLoading: true);
    try {
      final membership = await _repo.getTerritoryMembership(territoryId);
      state = MembershipState(membership: membership);
    } catch (e) {
      state = MembershipState(error: e);
    }
  }

  Future<void> becomeResident({String? message}) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;
    await _repo.becomeResident(territoryId: territoryId, message: message);
    await refresh();
  }

  Future<void> verifyByGeo(double lat, double lng) async {
    final territoryId = _ref.read(selectedTerritoryIdValueProvider);
    if (territoryId == null || territoryId.isEmpty) return;
    await _repo.verifyResidencyByGeo(territoryId: territoryId, lat: lat, lng: lng);
    await refresh();
  }
}

final membershipProvider =
    StateNotifierProvider.autoDispose<MembershipNotifier, MembershipState>((ref) {
  return MembershipNotifier(ref);
});
