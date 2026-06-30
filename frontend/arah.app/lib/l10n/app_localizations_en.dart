// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'Ará';

  @override
  String get login => 'Log in';

  @override
  String get loginSubtitle => 'Sign in to your account';

  @override
  String get email => 'Email';

  @override
  String get emailHint => 'you@email.com';

  @override
  String get nameOptional => 'Name (optional)';

  @override
  String get informEmail => 'Enter your email';

  @override
  String get home => 'Home';

  @override
  String get explore => 'Explore';

  @override
  String get post => 'Post';

  @override
  String get notifications => 'Notifications';

  @override
  String get profile => 'Profile';

  @override
  String get chooseTerritory => 'Choose a territory to see the feed';

  @override
  String get territories => 'Territories';

  @override
  String get territoriesSubtitle =>
      'Tap a territory to see its feed or switch region.';

  @override
  String get noTerritoryAvailable => 'No territory available';

  @override
  String get onboardingTitle => 'Choose your territory';

  @override
  String get onboardingDescription =>
      'To see the feed and join the community, choose a territory near you.';

  @override
  String get useMyLocation => 'Use my location';

  @override
  String get enableLocationHint => 'Enable location to see nearby territories.';

  @override
  String get noTerritoryInRegion => 'No territory registered near you yet.';

  @override
  String get registerMunicipalityTitle => 'Register your municipality';

  @override
  String get registerMunicipalityDescription =>
      'We use the official IBGE boundary to create your city territory so you can enter as a visitor.';

  @override
  String registerMunicipalityButton(String city) {
    return 'Register $city';
  }

  @override
  String get registerMunicipalityButtonGeneric => 'Register my municipality';

  @override
  String get registerMunicipalityLoading => 'Fetching official boundary...';

  @override
  String registerMunicipalitySuccess(String city) {
    return 'Territory $city is ready to select.';
  }

  @override
  String get registerMunicipalityFailed =>
      'Could not fetch IBGE boundary. Adjust the pin and draw your cell.';

  @override
  String get proposeTerritoryButton => 'Draw my cell';

  @override
  String get proposeTerritoryTitle => 'Propose territory';

  @override
  String get proposeTerritoryDescription =>
      'Adjust the pin on the map, confirm city and state, and draw the boundary. A curator will validate before activation; you get provisional visitor access.';

  @override
  String get proposeTerritoryTapPin =>
      'Tap the map to adjust the territory center.';

  @override
  String get proposeTerritoryTapPolygon =>
      'Tap the map to add polygon vertices (minimum 3).';

  @override
  String get proposeTerritoryCity => 'City';

  @override
  String get proposeTerritoryState => 'State';

  @override
  String get proposeTerritoryNameOptional => 'Cell name (optional)';

  @override
  String get proposeTerritoryPolygonMode => 'Draw polygon';

  @override
  String get proposeTerritoryPolygonModeHint =>
      'Off: uses a circle with adjustable radius.';

  @override
  String proposeTerritoryRadiusLabel(String km) {
    return 'Radius: $km km';
  }

  @override
  String get proposeTerritoryClearPolygon => 'Clear polygon';

  @override
  String get proposeTerritorySubmit => 'Submit proposal';

  @override
  String get proposeTerritorySubmitting => 'Submitting proposal...';

  @override
  String proposeTerritorySuccess(String name) {
    return 'Proposal submitted: $name. Provisional access until validation.';
  }

  @override
  String get proposeTerritoryCityStateRequired =>
      'Enter city and state (2 letters).';

  @override
  String get proposeTerritoryPolygonMinPoints =>
      'The polygon needs at least 3 points.';

  @override
  String get territoryPendingBadge => 'Awaiting curator';

  @override
  String get onboardingPendingTerritoryHint =>
      'This territory is under validation. You can enter provisionally as a visitor.';

  @override
  String get onboardingNearbyTitle => 'Near you';

  @override
  String get onboardingAllTerritoriesTitle => 'All territories';

  @override
  String get onboardingOrChooseFromList =>
      'Or choose a territory from the list below';

  @override
  String get onboardingLocationEnabled => 'Location enabled';

  @override
  String get onboardingLocationPrivacy =>
      'Your location is private and not visible to other users.';

  @override
  String get onboardingAllowLocationToCenter =>
      'Allow location to center the map and see nearby territories.';

  @override
  String onboardingContinueWith(Object name) {
    return 'Continue with $name';
  }

  @override
  String get onboardingVisitorOnContinue =>
      'When you continue, you will enter as a visitor in this territory and will be able to see the feed.';

  @override
  String get onboardingGettingLocation => 'Getting location...';

  @override
  String get onboardingLoadingTerritories => 'Loading territories...';

  @override
  String get tryAgain => 'Try again';

  @override
  String get noPostsHere => 'No posts in this region';

  @override
  String get beFirstToPost => 'Be the first to post here.';

  @override
  String get loadMore => 'Load more';

  @override
  String get editProfile => 'Edit profile';

  @override
  String get myTerritory => 'My territory';

  @override
  String get logout => 'Log out';

  @override
  String get save => 'Save';

  @override
  String get name => 'Name';

  @override
  String get bioOptional => 'Bio (optional)';

  @override
  String get profileUpdated => 'Profile updated';

  @override
  String get nameRequired => 'Name is required';

  @override
  String get createPost => 'Post';

  @override
  String get postCreated => 'Post created successfully.';

  @override
  String get title => 'Title';

  @override
  String get titleHint => 'Give your post a title';

  @override
  String get content => 'Content';

  @override
  String get contentHint => 'What do you want to share?';

  @override
  String get type => 'Type';

  @override
  String get visibility => 'Visibility';

  @override
  String get general => 'General';

  @override
  String get alert => 'Alert';

  @override
  String get public => 'Public';

  @override
  String get residentsOnly => 'Residents only';

  @override
  String get informTitle => 'Enter the title.';

  @override
  String get informContent => 'Enter the content.';

  @override
  String get noTerritorySelected => 'No territory selected';

  @override
  String get chooseTerritoryInExplore =>
      'Tap Explore, choose a territory, and come back here to post.';

  @override
  String get comingSoon => 'Coming soon';

  @override
  String get errorLoad => 'Error loading.';

  @override
  String get sessionExpired => 'Session expired. Please log in again.';

  @override
  String get enterToAccess =>
      'Sign in to access profile, post, and notifications.';

  @override
  String get map => 'Map';

  @override
  String get viewOnMap => 'View on map';

  @override
  String get mapEntity => 'Place / map point';

  @override
  String get mapPost => 'Post';

  @override
  String get mapEvent => 'Event';

  @override
  String get mapAsset => 'Media / asset';

  @override
  String get mapAlert => 'Alert';

  @override
  String get mapPin => 'Map pin';

  @override
  String get noNotifications => 'No notifications';

  @override
  String get events => 'Events';

  @override
  String get noEvents => 'No events in this territory';

  @override
  String get eventInterested => 'I\'m interested';

  @override
  String get eventConfirm => 'Confirm attendance';

  @override
  String get eventConfirmed => 'Attendance confirmed';

  @override
  String get myInterests => 'My interests';

  @override
  String get myInterestsHint => 'Interests help personalize your feed.';

  @override
  String get interestTag => 'Interest';

  @override
  String get interestAdded => 'Interest added';

  @override
  String get interestRemoved => 'Interest removed';

  @override
  String get notificationPreferences => 'Notification preferences';

  @override
  String get notifPosts => 'Posts';

  @override
  String get notifComments => 'Comments';

  @override
  String get notifEvents => 'Events';

  @override
  String get notifAlerts => 'Alerts';

  @override
  String get filterByInterests => 'By interests';

  @override
  String get inTerritory => 'In';

  @override
  String get loginWithGoogle => 'Sign in with Google';

  @override
  String get noAccountYet => 'Don\'t have an account?';

  @override
  String get createAccount => 'Create account';

  @override
  String get loginOr => 'or';

  @override
  String get continueButton => 'Continue';

  @override
  String get password => 'Password';

  @override
  String get passwordHint => 'Enter your password';

  @override
  String get enterPassword => 'Enter your password to sign in';

  @override
  String get signUp => 'Create account';

  @override
  String get signUpSubtitle => 'Fill in your details to create an account';

  @override
  String get confirmPassword => 'Confirm password';

  @override
  String get confirmPasswordHint => 'Repeat password';

  @override
  String get displayName => 'Name';

  @override
  String get displayNameHint => 'What should we call you';

  @override
  String get back => 'Back';

  @override
  String get accountCreated => 'Account created. Welcome!';

  @override
  String get passwordMinLength => 'At least 6 characters';

  @override
  String get passwordsDontMatch => 'Passwords do not match';
}
