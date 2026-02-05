/// Evento do território (BFF events/territory-events). Alinhado a EventItemJourneyDto.
class EventItem {
  const EventItem({
    required this.id,
    required this.territoryId,
    required this.title,
    this.description,
    required this.startsAtUtc,
    required this.endsAtUtc,
    this.latitude,
    this.longitude,
    this.locationLabel,
    required this.status,
    this.createdByDisplayName,
    this.participants,
    this.userParticipationStatus,
  });

  final String id;
  final String territoryId;
  final String title;
  final String? description;
  final DateTime startsAtUtc;
  final DateTime endsAtUtc;
  final double? latitude;
  final double? longitude;
  final String? locationLabel;
  final String status;
  final String? createdByDisplayName;
  final EventParticipantsSummary? participants;
  final String? userParticipationStatus; // NONE | INTERESTED | CONFIRMED

  factory EventItem.fromJson(Map<String, dynamic> json) {
    final event = json['event'] as Map<String, dynamic>? ?? json;
    final id = event['id'];
    final startsAt = event['startsAtUtc'];
    final endsAt = event['endsAtUtc'];

    final participantsJson = json['participants'] as Map<String, dynamic>?;
    final userPart = json['userParticipation'] as Map<String, dynamic>?;

    return EventItem(
      id: id == null ? '' : id.toString(),
      territoryId: (event['territoryId'] ?? '').toString(),
      title: event['title'] as String? ?? '',
      description: event['description'] as String?,
      startsAtUtc: startsAt != null
          ? DateTime.tryParse(startsAt.toString()) ?? DateTime.now()
          : DateTime.now(),
      endsAtUtc: endsAt != null
          ? DateTime.tryParse(endsAt.toString()) ?? DateTime.now()
          : DateTime.now(),
      latitude: (event['latitude'] as num?)?.toDouble(),
      longitude: (event['longitude'] as num?)?.toDouble(),
      locationLabel: event['locationLabel'] as String?,
      status: event['status'] as String? ?? 'Draft',
      createdByDisplayName: event['createdByDisplayName'] as String?,
      participants: participantsJson != null
          ? EventParticipantsSummary.fromJson(participantsJson)
          : null,
      userParticipationStatus: userPart?['status'] as String?,
    );
  }
}

class EventParticipantsSummary {
  const EventParticipantsSummary({
    this.interestedCount = 0,
    this.confirmedCount = 0,
  });

  final int interestedCount;
  final int confirmedCount;

  factory EventParticipantsSummary.fromJson(Map<String, dynamic> json) {
    return EventParticipantsSummary(
      interestedCount: (json['interestedCount'] as num?)?.toInt() ?? 0,
      confirmedCount: (json['confirmedCount'] as num?)?.toInt() ?? 0,
    );
  }
}
