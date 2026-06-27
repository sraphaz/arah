import '../../../../l10n/app_localizations.dart';

/// Tipos de votação suportados pela API (Domain.Governance.VotingType).
const List<String> votingTypes = [
  'CommunityPolicy',
  'ThemePrioritization',
  'TerritoryCharacterization',
  'ModerationRule',
  'FeatureFlag',
];

/// Visibilidades suportadas pela API (Domain.Governance.VotingVisibility).
const List<String> votingVisibilities = [
  'AllMembers',
  'ResidentsOnly',
  'CuratorsOnly',
];

String votingTypeLabel(AppLocalizations l10n, String type) {
  switch (type) {
    case 'ThemePrioritization':
      return l10n.votingTypeThemePrioritization;
    case 'ModerationRule':
      return l10n.votingTypeModerationRule;
    case 'FeatureFlag':
      return l10n.votingTypeFeatureFlag;
    case 'TerritoryCharacterization':
      return l10n.votingTypeTerritoryCharacterization;
    case 'CommunityPolicy':
      return l10n.votingTypeCommunityPolicy;
    default:
      return type;
  }
}

String votingVisibilityLabel(AppLocalizations l10n, String visibility) {
  switch (visibility) {
    case 'AllMembers':
      return l10n.votingVisibilityAllMembers;
    case 'ResidentsOnly':
      return l10n.votingVisibilityResidentsOnly;
    case 'CuratorsOnly':
      return l10n.votingVisibilityCuratorsOnly;
    default:
      return visibility;
  }
}

String votingStatusLabel(AppLocalizations l10n, String status) {
  switch (status.toLowerCase()) {
    case 'open':
      return l10n.statusOpen;
    case 'closed':
      return l10n.statusClosed;
    case 'cancelled':
      return l10n.statusCancelled;
    default:
      return status;
  }
}
