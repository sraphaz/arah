import '../../../../core/network/api_exception.dart';
import '../../../../core/network/bff_client.dart';
import '../models/subscription_models.dart';

class SubscriptionsRepository {
  SubscriptionsRepository({required BffClient client}) : _client = client;

  final BffClient _client;

  Future<List<SubscriptionPlan>> listPlans({String? territoryId}) async {
    final params = territoryId != null && territoryId.isNotEmpty
        ? {'territoryId': territoryId}
        : null;
    final response = await _client.get('subscription-plans', '', queryParameters: params);
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data;
    if (data is! List) return const [];
    return data.whereType<Map<String, dynamic>>().map(SubscriptionPlan.fromJson).toList();
  }

  Future<MySubscription?> getMySubscription({String? territoryId}) async {
    final params = territoryId != null && territoryId.isNotEmpty
        ? {'territoryId': territoryId}
        : null;
    final response = await _client.get('subscriptions', 'me', queryParameters: params);
    if (response.statusCode == 404) return null;
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    final data = response.data as Map<String, dynamic>?;
    if (data == null) return null;
    return MySubscription.fromJson(data);
  }

  Future<MySubscription> subscribe({
    required String planId,
    String? territoryId,
  }) async {
    final response = await _client.post(
      'subscriptions',
      '',
      body: {
        'planId': planId,
        if (territoryId != null && territoryId.isNotEmpty) 'territoryId': territoryId,
      },
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
    return MySubscription.fromJson(response.data as Map<String, dynamic>);
  }

  Future<void> cancelSubscription({
    required String subscriptionId,
    bool cancelAtPeriodEnd = true,
  }) async {
    final response = await _client.post(
      'subscriptions',
      '$subscriptionId/cancel',
      body: {'cancelAtPeriodEnd': cancelAtPeriodEnd},
    );
    if (response.statusCode < 200 || response.statusCode >= 300) {
      throw ApiException('HTTP ${response.statusCode}', statusCode: response.statusCode);
    }
  }
}
