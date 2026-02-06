import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../data/repositories/territories_repository.dart';

final territoriesRepositoryProvider = Provider<TerritoriesRepository>((ref) {
  return TerritoriesRepository(client: ref.watch(bffClientProvider));
});

/// Detalhe do território (centro, raio, polígono) para desenhar o perímetro no mapa.
final territoryDetailProvider = FutureProvider.autoDispose.family<TerritoryDetail?, String>((ref, territoryId) async {
  if (territoryId.isEmpty) return null;
  final repo = ref.watch(territoriesRepositoryProvider);
  return repo.getTerritoryById(territoryId);
});

/// Extrai lista de itens da resposta.
/// API/BFF retornam PagedResponse: { "items": [ { "id", "name", "description", ... } ], "pageNumber", ... }.
/// Aceita também: data como List no root, ou wrapper com "data": { "items": [...] }.
List<dynamic> _extractItemsFromResponse(dynamic data) {
  if (data == null) return [];
  // Resposta pode vir como String (ex.: Flutter Web)
  if (data is String) {
    try {
      data = jsonDecode(data);
    } catch (_) {
      return [];
    }
  }
  // Lista no root (ex.: algum proxy)
  if (data is List) return data;
  if (data is! Map<String, dynamic>) return [];
  final map = data as Map<String, dynamic>;
  // Wrapper tipo { "data": { "items": [...] } }
  final inner = map['data'];
  if (inner is Map<String, dynamic>) {
    final list = inner['items'] as List? ?? inner['Items'] as List? ?? inner['territories'] as List? ?? inner['Territories'] as List?;
    if (list != null) return list;
  }
  // PagedResponse direto: items (camelCase) ou Items (PascalCase)
  return map['items'] as List? ?? map['Items'] as List? ?? map['territories'] as List? ?? map['Territories'] as List? ?? [];
}

final territoriesListProvider = FutureProvider.autoDispose<List<TerritoryItem>>((ref) async {
  final client = ref.watch(bffClientProvider);
  const path = 'paged?pageNumber=1&pageSize=50';
  final response = await client.get('territories', path);
  final rawItems = _extractItemsFromResponse(response.data);
  final list = <TerritoryItem>[];
  for (final e in rawItems) {
    if (e is! Map<String, dynamic>) continue;
    list.add(TerritoryItem.fromJson(e));
  }
  return list;
});

/// Item de território para listagem (onboarding, seletor).
/// Espera da API: id (Guid string), name, description (opcional); aceita camelCase ou PascalCase.
class TerritoryItem {
  const TerritoryItem({required this.id, required this.name, this.description});
  final String id;
  final String name;
  final String? description;

  factory TerritoryItem.fromJson(Map<String, dynamic> json) {
    final id = _readString(json, ['id', 'Id']);
    final name = _readString(json, ['name', 'Name', 'title', 'Title']) ?? '';
    final description = _readString(json, ['description', 'Description']);
    return TerritoryItem(
      id: id ?? '',
      name: name,
      description: description?.isEmpty == true ? null : description,
    );
  }

  static String? _readString(Map<String, dynamic> json, List<String> keys) {
    for (final k in keys) {
      final v = json[k];
      if (v == null) continue;
      if (v is String) return v;
      return v.toString();
    }
    return null;
  }
}
