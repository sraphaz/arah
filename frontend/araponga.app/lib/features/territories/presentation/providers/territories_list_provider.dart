import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/providers/app_providers.dart';
import '../../data/repositories/territories_repository.dart';

final territoriesRepositoryProvider = Provider<TerritoriesRepository>((ref) {
  return TerritoriesRepository(client: ref.watch(bffClientProvider));
});

final territoriesListProvider = FutureProvider.autoDispose<List<TerritoryItem>>((ref) async {
  final client = ref.watch(bffClientProvider);
  const path = 'paged?pageNumber=1&pageSize=50';
  final response = await client.get('territories', path);
  final data = response.data as Map<String, dynamic>?;
  final items = data != null ? (data['items'] as List?) ?? (data['territories'] as List?) ?? [] : [];
  return items.map((e) => TerritoryItem.fromJson(e as Map<String, dynamic>)).toList();
});

class TerritoryItem {
  const TerritoryItem({required this.id, required this.name, this.description});
  final String id;
  final String name;
  final String? description;

  factory TerritoryItem.fromJson(Map<String, dynamic> json) {
    final id = json['id'];
    return TerritoryItem(
      id: id == null ? '' : id.toString(),
      name: json['name'] as String? ?? json['title'] as String? ?? '',
      description: json['description'] as String?,
    );
  }
}
