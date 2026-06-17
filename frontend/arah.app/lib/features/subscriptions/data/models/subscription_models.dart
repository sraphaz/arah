class SubscriptionPlan {
  const SubscriptionPlan({
    required this.id,
    required this.name,
    required this.description,
    required this.priceAmount,
    required this.currency,
  });

  factory SubscriptionPlan.fromJson(Map<String, dynamic> json) {
    return SubscriptionPlan(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? 'Plano',
      description: json['description']?.toString() ?? '',
      priceAmount: (json['pricePerCycle'] as num?)?.toDouble() ?? 0,
      currency: 'BRL',
    );
  }

  final String id;
  final String name;
  final String description;
  final double priceAmount;
  final String currency;
}

class MySubscription {
  const MySubscription({
    required this.id,
    required this.planId,
    required this.status,
  });

  factory MySubscription.fromJson(Map<String, dynamic> json) {
    return MySubscription(
      id: json['id']?.toString() ?? '',
      planId: json['planId']?.toString() ?? '',
      status: json['status']?.toString() ?? '',
    );
  }

  final String id;
  final String planId;
  final String status;
}
