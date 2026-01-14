erDiagram
  %% =========================================================
  %% NOTE (ATUALIZAÇÃO)
  %% =========================================================
  %% Este MER é uma referência conceitual histórica e pode conter nomes legados
  %% (ex.: USER_TERRITORY). O modelo atual do projeto está descrito em:
  %% - docs/12_DOMAIN_MODEL.md
  %%
  %% A seguir, são adicionadas entidades do P0 Admin/Filas/Evidências:
  %% - SYSTEM_CONFIG (configurações globais)
  %% - WORK_ITEM (fila genérica para revisão humana)
  %% - DOCUMENT_EVIDENCE (metadados de evidências; conteúdo em storage)
  %% =========================================================
  %% 1) IDENTITY, AUTH, DEVICES
  %% =========================================================

  USER {
    uuid id PK
    string name
    string email
    string phone
    string status                       "ACTIVE|SUSPENDED"
    datetime created_at
    datetime last_login_at
  }

  USER_SECURITY_SETTINGS {
    uuid user_id PK,FK
    boolean biometric_enabled
    datetime last_strong_auth_at
    datetime updated_at
  }

  USER_DEVICE {
    uuid id PK
    uuid user_id FK
    string platform                     "IOS|ANDROID|WEB"
    string device_label
    string device_fingerprint           "hashed, optional"
    string public_key                   "optional: passkeys later"
    boolean is_trusted
    boolean is_revoked
    datetime created_at
    datetime last_seen_at
  }

  IDENTITY_PROVIDER_ACCOUNT {
    uuid id PK
    uuid user_id FK
    string provider                     "GOOGLE|APPLE|MICROSOFT"
    string provider_subject             "OIDC sub"
    string email                        "nullable (Apple)"
    boolean email_verified
    string display_name
    datetime linked_at
    datetime last_login_at
  }

  AUTH_SESSION {
    uuid id PK
    uuid user_id FK
    uuid device_id FK
    uuid identity_provider_account_id FK "nullable"
    string auth_method                  "OTP|OIDC_GOOGLE|OIDC_APPLE|OIDC_MICROSOFT"
    string session_type                 "ACCESS|REFRESH"
    string token_hash
    datetime created_at
    datetime expires_at
    datetime revoked_at
  }

  %% =========================================================
  %% 2) TERRITORY, MEMBERSHIP, GOVERNANCE
  %% =========================================================

  TERRITORY {
    uuid id PK
    string name
    string description
    string sensitivity_level            "LOW|MEDIUM|HIGH"
    boolean is_pilot
    string polygon_geojson
    datetime created_at
  }

  USER_TERRITORY {
    uuid id PK
    uuid user_id FK
    uuid territory_id FK
    string role                         "VISITOR|RESIDENT"
    string status                       "ACTIVE|PENDING|PROVISIONAL|CONFIRMED|EXPIRED"
    datetime declared_at
    datetime last_presence_confirmation_at
    datetime created_at
  }

  FEATURE_FLAG {
    string key PK
    string description
    boolean default_enabled
    datetime created_at
  }

  TERRITORY_FEATURE_FLAG {
    uuid territory_id FK
    string feature_key FK
    boolean enabled
    string enabled_for_roles_json       "optional"
    datetime updated_at
  }

  RESIDENT_VERIFICATION {
    uuid id PK
    uuid territory_id FK
    uuid requester_user_id FK
    uuid verified_by_user_id FK
    string status                       "APPROVED|REJECTED"
    string note
    datetime created_at
  }

  %% =========================================================
  %% 3) CONTENT (FEED + MAP)
  %% =========================================================

  POST {
    uuid id PK
    uuid territory_id FK
    uuid author_id FK
    string type                         "NOTICE|HELP|INVITE|SHARE"
    string visibility                   "PUBLIC|RESIDENT_ONLY"
    text content
    datetime created_at
    datetime updated_at
  }

  EVENT {
    uuid id PK
    uuid territory_id FK
    uuid creator_id FK
    string title
    text description
    float location_lat
    float location_lng
    datetime start_datetime
    datetime end_datetime
    string visibility                   "PUBLIC|RESIDENT_ONLY"
    datetime created_at
    datetime updated_at
  }

  EVENT_TERRITORY {
    uuid id PK
    uuid event_id FK
    uuid territory_id FK
    string visibility                   "PUBLIC|RESIDENT_ONLY"
    boolean pinned
    datetime created_at
  }

  ALERT {
    uuid id PK
    uuid territory_id FK
    uuid creator_id FK
    string category                     "WATER|ROAD|SECURITY|WEATHER|OTHER"
    string severity                     "INFO|WARNING|URGENT"
    text message
    datetime created_at
    datetime expires_at
  }

  PLACE {
    uuid id PK
    uuid territory_id FK
    uuid created_by_user_id FK
    string name
    string category                     "MARKET|FOOD|SERVICE|HEALTH|COMMUNITY|NATURE|OTHER"
    text description
    float location_lat
    float location_lng
    string address_text
    string contact_phone
    string contact_whatsapp
    string opening_hours_text
    string status                       "PENDING|SOFT_PUBLISHED|PUBLISHED|REVIEW|HIDDEN"
    int confirmation_score
    datetime confirmed_at
    datetime created_at
    datetime updated_at
  }

  PLACE_CONFIRMATION {
    uuid id PK
    uuid place_id FK
    uuid user_id FK
    string action                       "CONFIRM|DISCONFIRM|UPDATE_SUGGESTION|REPORT"
    string note
    datetime created_at
  }

  %% =========================================================
  %% 4) SOCIAL INTERACTIONS (LIKES, COMMENTS, SHARES)
  %% =========================================================

  REACTION {
    uuid id PK
    uuid territory_id FK
    uuid user_id FK
    string target_type                  "POST|COMMENT|EVENT|ALERT|PLACE|OFFER|NATURAL_ASSET"
    uuid target_id
    string reaction_type                "LIKE|THANKS|SUPPORT"
    datetime created_at
  }

  COMMENT {
    uuid id PK
    uuid territory_id FK
    uuid author_user_id FK
    string target_type                  "POST|EVENT|ALERT|PLACE|OFFER|NATURAL_ASSET"
    uuid target_id
    uuid parent_comment_id FK           "nullable (1-level threads in MVP)"
    text content
    string status                       "VISIBLE|HIDDEN|DELETED"
    datetime created_at
    datetime updated_at
  }

  SHARE {
    uuid id PK
    uuid territory_id FK
    uuid user_id FK
    string target_type                  "POST|EVENT|ALERT|PLACE|OFFER|NATURAL_ASSET"
    uuid target_id
    string share_type                   "INTERNAL|EXTERNAL"
    uuid repost_post_id FK              "nullable (INTERNAL)"
    datetime created_at
  }

  %% =========================================================
  %% 5) MEDIA
  %% =========================================================

  MEDIA_ASSET {
    uuid id PK
    uuid uploaded_by_user_id FK
    string media_type                   "IMAGE|VIDEO|AUDIO|DOC"
    string mime_type
    string storage_key
    int size_bytes
    int width_px
    int height_px
    string checksum
    datetime created_at
  }

  MEDIA_ATTACHMENT {
    uuid id PK
    uuid media_asset_id FK
    string owner_type                   "USER|POST|EVENT|ALERT|PLACE|OFFER|NATURAL_ASSET|HEALTH_OBSERVATION|TERRITORY_ACTION"
    uuid owner_id
    string purpose                      "AVATAR|COVER|GALLERY|EVIDENCE"
    int sort_order
    datetime created_at
  }

  %% =========================================================
  %% 6) MARKET (OFFERS) + LOCAL VALUE (CURRENCY / WALLETS)
  %% =========================================================

  OFFER {
    uuid id PK
    uuid territory_id FK
    uuid created_by_user_id FK
    string type                         "SERVICE|PRODUCT"
    string title
    text description
    string visibility                   "INTERNAL|PUBLIC"
    decimal price_amount
    uuid price_currency_id FK           "nullable (free / contact)"
    string status                       "DRAFT|PUBLISHED|PAUSED|ARCHIVED"
    datetime created_at
    datetime updated_at
  }

  TERRITORY_CURRENCY {
    uuid id PK
    uuid territory_id FK
    string symbol                       "e.g. ARA"
    string name
    string description
    int decimals
    string status                       "DISABLED|PILOT|ENABLED"
    datetime created_at
  }

  USER_WALLET {
    uuid id PK
    uuid user_id FK
    uuid territory_id FK
    uuid currency_id FK
    string status                       "ACTIVE|SUSPENDED"
    datetime created_at
  }

  WALLET_TRANSACTION {
    uuid id PK
    uuid wallet_id FK
    string direction                    "CREDIT|DEBIT"
    decimal amount
    string reason                       "SERVICE|PRODUCT|DONATION|REWARD|SYSTEM|ADJUSTMENT"
    string reference_type               "OFFER|PAYMENT|MANUAL|SYSTEM"
    uuid reference_id
    datetime created_at
  }

  TERRITORY_FUND {
    uuid id PK
    uuid territory_id FK
    uuid currency_id FK
    string name
    string description
    decimal balance_amount
    datetime created_at
    datetime updated_at
  }

  FUND_ALLOCATION {
    uuid id PK
    uuid fund_id FK
    string allocation_type              "SERVICE|INFRA|MAINTENANCE|DONATION|OTHER"
    decimal amount
    string note
    string reference_type               "PAYMENT|MANUAL|SYSTEM"
    uuid reference_id
    datetime created_at
  }

  %% =========================================================
  %% 7) PAYMENTS (FIAT VIA PROVIDERS)
  %% =========================================================

  PAYMENT_PROVIDER {
    uuid id PK
    string name                         "PAYPAL|MERCADOPAGO"
    string status                       "ENABLED|DISABLED"
    datetime created_at
  }

  PAYMENT_TRANSACTION {
    uuid id PK
    uuid provider_id FK
    uuid territory_id FK
    uuid payer_user_id FK
    uuid payee_user_id FK
    uuid related_offer_id FK            "nullable"
    decimal amount_fiat
    string fiat_currency                "BRL|USD|EUR..."
    string status                       "PENDING|PAID|FAILED|REFUNDED|CANCELLED"
    string external_ref                 "provider reference"
    datetime created_at
    datetime updated_at
  }

  %% =========================================================
  %% 8) TERRITORY HEALTH (OBSERVATIONS, SENSORS, INDICATORS, ACTIONS)
  %% =========================================================

  HEALTH_DOMAIN {
    uuid id PK
    string name                         "WATER|AIR|SOIL|BIODIVERSITY|WASTE|SAFETY|MOBILITY|WELLBEING"
    string description
    datetime created_at
  }

  HEALTH_METRIC {
    uuid id PK
    uuid domain_id FK
    string key                          "e.g. water.turbidity_ntu"
    string name
    string unit                         "NTU|PPM|UG_M3|MM|CM|INDEX"
    string value_type                   "DECIMAL|INTEGER|BOOLEAN|TEXT|INDEX"
    string description
    datetime created_at
  }

  HEALTH_OBSERVATION {
    uuid id PK
    uuid territory_id FK
    uuid domain_id FK
    uuid metric_id FK                   "nullable"
    uuid reporter_user_id FK            "nullable (anonymous/internal mode)"
    uuid related_natural_asset_id FK    "nullable"
    string severity                     "INFO|WARNING|URGENT"
    string visibility                   "PUBLIC|RESIDENT_ONLY"
    float location_lat
    float location_lng
    text description
    string status                       "OPEN|UNDER_REVIEW|CONFIRMED|RESOLVED|REJECTED"
    datetime observed_at
    datetime created_at
    datetime updated_at
  }

  HEALTH_OBSERVATION_CONFIRMATION {
    uuid id PK
    uuid observation_id FK
    uuid user_id FK
    string action                       "CONFIRM|DISCONFIRM|ADD_CONTEXT|REPORT"
    string note
    datetime created_at
  }

  SENSOR_DEVICE {
    uuid id PK
    uuid territory_id FK
    string name
    string device_type                  "RAIN_GAUGE|WATER_LEVEL|AIR_QUALITY|WATER_QUALITY|WEATHER"
    string status                       "ACTIVE|MAINTENANCE|RETIRED"
    float location_lat
    float location_lng
    string external_ref                 "optional"
    datetime installed_at
    datetime created_at
  }

  SENSOR_READING {
    uuid id PK
    uuid device_id FK
    uuid metric_id FK
    decimal value_decimal
    int value_int
    boolean value_bool
    string value_text
    datetime measured_at
    datetime created_at
  }

  TERRITORY_HEALTH_INDICATOR {
    uuid id PK
    uuid territory_id FK
    uuid metric_id FK
    string period                       "DAILY|WEEKLY|MONTHLY"
    datetime period_start
    datetime period_end
    decimal value_decimal
    string calculation_method           "AVG|MAX|INDEX_FORMULA"
    datetime created_at
  }

  TERRITORY_ACTION {
    uuid id PK
    uuid territory_id FK
    uuid related_observation_id FK      "nullable"
    uuid organizer_user_id FK
    string type                         "MUTIRAO|MAINTENANCE|EDUCATION|RESTORATION|MONITORING"
    string title
    text description
    datetime start_datetime
    datetime end_datetime
    string visibility                   "PUBLIC|RESIDENT_ONLY"
    string status                       "PLANNED|IN_PROGRESS|DONE|CANCELLED"
    datetime created_at
    datetime updated_at
  }

  %% =========================================================
  %% 9) NATURAL ASSETS (SPRINGS, WATERFALLS, TRAILS, TREES, ETC.)
  %% =========================================================

  NATURAL_ASSET {
    uuid id PK
    uuid territory_id FK
    uuid created_by_user_id FK
    string type                         "SPRING|WATERFALL|NATIVE_TREE|SANCTUARY|VIEWPOINT|POTABLE_WATER|TRAIL"
    string name
    text description
    float location_lat
    float location_lng
    string visibility                   "PUBLIC|RESIDENT_ONLY"
    string access_level                 "OPEN|RESTRICTED|PRIVATE|SEASONAL"
    string sensitivity_level            "LOW|MEDIUM|HIGH"
    string status                       "PENDING|PUBLISHED|HIDDEN|REVIEW"
    int confirmation_score
    datetime confirmed_at
    datetime created_at
    datetime updated_at
  }

  NATURAL_ASSET_CONFIRMATION {
    uuid id PK
    uuid natural_asset_id FK
    uuid user_id FK
    string action                       "CONFIRM|DISCONFIRM|UPDATE_SUGGESTION|REPORT"
    string note
    datetime created_at
  }

  TRAIL_SEGMENT {
    uuid id PK
    uuid trail_asset_id FK              "FK -> NATURAL_ASSET (type=TRAIL)"
    int sequence
    float start_lat
    float start_lng
    float end_lat
    float end_lng
    string difficulty                   "EASY|MODERATE|HARD"
    decimal distance_km
    decimal elevation_gain_m
    string path_geojson                 "optional"
    datetime created_at
  }

  WATER_POINT_DETAILS {
    uuid natural_asset_id PK,FK         "FK -> NATURAL_ASSET (type=POTABLE_WATER|SPRING)"
    string water_type                   "SPRING|TAP|WELL|FILTERED_SOURCE"
    string potability_status            "UNKNOWN|POTABLE|NOT_POTABLE|SEASONAL"
    datetime last_tested_at
    string tested_by                    "LAB|COMMUNITY|UNKNOWN"
    text notes
  }

  NATIVE_TREE_DETAILS {
    uuid natural_asset_id PK,FK         "FK -> NATURAL_ASSET (type=NATIVE_TREE)"
    string species_common_name
    string species_scientific_name
    string protection_status            "UNKNOWN|PROTECTED|ENDANGERED"
    decimal estimated_age_years
    text notes
  }

  SANCTUARY_DETAILS {
    uuid natural_asset_id PK,FK         "FK -> NATURAL_ASSET (type=SANCTUARY)"
    string sanctuary_kind               "CULTURAL|SPIRITUAL|ECOLOGICAL"
    string etiquette_rules              "optional"
    text notes
  }

  %% =========================================================
  %% 10) AUDIT & SECURITY LOGS
  %% =========================================================

  AUDIT_EVENT {
    uuid id PK
    uuid actor_user_id FK
    uuid territory_id FK                "nullable"
    string action
    string entity_type
    uuid entity_id
    string result                       "SUCCESS|FAIL"
    string reason
    string meta_json
    datetime created_at
  }

  SECURITY_EVENT {
  %% =========================================================
  %% 11) ADMIN CONFIG + WORK QUEUE + EVIDENCES
  %% =========================================================

  SYSTEM_CONFIG {
    uuid id PK
    string key                          "unique"
    string value
    string category
    string description
    uuid created_by_user_id
    datetime created_at
    uuid updated_by_user_id
    datetime updated_at
  }

  WORK_ITEM {
    uuid id PK
    string type                         "IDENTITY_VERIFICATION|RESIDENCY_VERIFICATION|ASSET_CURATION|MODERATION_CASE"
    string status                       "OPEN|REQUIRES_HUMAN_REVIEW|COMPLETED|CANCELLED"
    string outcome                      "NONE|APPROVED|REJECTED|NOACTION"
    uuid territory_id                   "nullable"
    uuid created_by_user_id
    datetime created_at
    string required_system_permission   "nullable"
    string required_capability          "nullable"
    string subject_type
    uuid subject_id
    string payload_json                 "nullable"
    uuid completed_by_user_id           "nullable"
    datetime completed_at               "nullable"
    string completion_notes             "nullable"
  }

  DOCUMENT_EVIDENCE {
    uuid id PK
    uuid user_id FK
    uuid territory_id FK                "nullable (identity=global)"
    string kind                         "IDENTITY|RESIDENCY"
    string storage_provider             "LOCAL|S3"
    string storage_key
    string content_type
    int size_bytes
    string sha256
    string original_file_name
    datetime created_at
  }
    uuid id PK
    uuid user_id FK
    uuid device_id FK                   "nullable"
    string event_type
    string meta_json
    datetime created_at
  }

  %% =========================================================
  %% RELATIONSHIPS (CARDINALITIES)
  %% =========================================================

  %% Identity
  USER ||--|o USER_SECURITY_SETTINGS : has_settings
  USER ||--o{ USER_DEVICE : has_devices
  USER ||--o{ IDENTITY_PROVIDER_ACCOUNT : links_identities
  USER ||--o{ AUTH_SESSION : has_sessions
  USER_DEVICE ||--o{ AUTH_SESSION : has_sessions
  IDENTITY_PROVIDER_ACCOUNT |o--o{ AUTH_SESSION : used_in

  %% Territory & membership
  USER      ||--o{ USER_TERRITORY : memberships
  TERRITORY ||--o{ USER_TERRITORY : memberships

  FEATURE_FLAG ||--o{ TERRITORY_FEATURE_FLAG : configured_for
  TERRITORY    ||--o{ TERRITORY_FEATURE_FLAG : has_flags

  TERRITORY ||--o{ RESIDENT_VERIFICATION : resident_requests
  USER      ||--o{ RESIDENT_VERIFICATION : requests
  USER      ||--o{ RESIDENT_VERIFICATION : verifies

  %% Content
  TERRITORY ||--o{ POST  : contains
  TERRITORY ||--o{ EVENT : contains
  TERRITORY ||--o{ ALERT : contains
  TERRITORY ||--o{ PLACE : contains
  TERRITORY ||--o{ OFFER : contains

  USER ||--o{ POST  : authors
  USER ||--o{ EVENT : creates
  USER ||--o{ ALERT : creates
  USER ||--o{ PLACE : creates
  USER ||--o{ OFFER : creates

  EVENT     ||--o{ EVENT_TERRITORY : distributed_to
  TERRITORY ||--o{ EVENT_TERRITORY : receives

  PLACE ||--o{ PLACE_CONFIRMATION : confirmations
  USER  ||--o{ PLACE_CONFIRMATION : confirmations

  %% Social interactions
  TERRITORY ||--o{ REACTION : scopes
  USER      ||--o{ REACTION : makes

  TERRITORY ||--o{ COMMENT : scopes
  USER      ||--o{ COMMENT : authors
  COMMENT   |o--o{ COMMENT : replies_to

  TERRITORY ||--o{ SHARE : scopes
  USER      ||--o{ SHARE : shares
  POST      |o--o{ SHARE : repost

  %% Media
  USER        ||--o{ MEDIA_ASSET : uploads
  MEDIA_ASSET ||--o{ MEDIA_ATTACHMENT : attached_as

  %% Currency & wallets
  TERRITORY ||--o{ TERRITORY_CURRENCY : has_currencies
  TERRITORY_CURRENCY ||--o{ USER_WALLET : used_by_wallets
  USER ||--o{ USER_WALLET : has_wallets
  TERRITORY ||--o{ USER_WALLET : has_wallets
  USER_WALLET ||--o{ WALLET_TRANSACTION : has_transactions

  TERRITORY ||--|o TERRITORY_FUND : has_fund
  TERRITORY_CURRENCY ||--o{ TERRITORY_FUND : denominates
  TERRITORY_FUND ||--o{ FUND_ALLOCATION : allocations

  %% Payments
  PAYMENT_PROVIDER ||--o{ PAYMENT_TRANSACTION : processes
  TERRITORY ||--o{ PAYMENT_TRANSACTION : scopes
  USER ||--o{ PAYMENT_TRANSACTION : pays
  USER ||--o{ PAYMENT_TRANSACTION : receives
  OFFER |o--o{ PAYMENT_TRANSACTION : paid_for
  PAYMENT_TRANSACTION |o--o{ WALLET_TRANSACTION : mirrors

  %% Territory health
  HEALTH_DOMAIN ||--o{ HEALTH_METRIC : defines
  TERRITORY ||--o{ HEALTH_OBSERVATION : has_observations
  HEALTH_DOMAIN ||--o{ HEALTH_OBSERVATION : classifies
  HEALTH_METRIC |o--o{ HEALTH_OBSERVATION : measures
  USER |o--o{ HEALTH_OBSERVATION : reports

  HEALTH_OBSERVATION ||--o{ HEALTH_OBSERVATION_CONFIRMATION : confirmations
  USER ||--o{ HEALTH_OBSERVATION_CONFIRMATION : confirms

  TERRITORY ||--o{ SENSOR_DEVICE : has_sensors
  SENSOR_DEVICE ||--o{ SENSOR_READING : produces
  HEALTH_METRIC ||--o{ SENSOR_READING : measures

  TERRITORY ||--o{ TERRITORY_HEALTH_INDICATOR : has_indicators
  HEALTH_METRIC ||--o{ TERRITORY_HEALTH_INDICATOR : computes

  TERRITORY ||--o{ TERRITORY_ACTION : has_actions
  USER ||--o{ TERRITORY_ACTION : organizes
  HEALTH_OBSERVATION |o--o{ TERRITORY_ACTION : responds_to

  %% Natural assets
  TERRITORY ||--o{ NATURAL_ASSET : contains
  USER      ||--o{ NATURAL_ASSET : creates

  NATURAL_ASSET ||--o{ NATURAL_ASSET_CONFIRMATION : confirmations
  USER          ||--o{ NATURAL_ASSET_CONFIRMATION : confirms

  NATURAL_ASSET |o--o{ TRAIL_SEGMENT : trail_geometry
  NATURAL_ASSET |o--|| WATER_POINT_DETAILS : water_details
  NATURAL_ASSET |o--|| NATIVE_TREE_DETAILS : tree_details
  NATURAL_ASSET |o--|| SANCTUARY_DETAILS : sanctuary_details

  NATURAL_ASSET |o--o{ HEALTH_OBSERVATION : related_observations

  %% Logs
  USER      ||--o{ AUDIT_EVENT : performs
  TERRITORY |o--o{ AUDIT_EVENT : scopes
  USER       ||--o{ SECURITY_EVENT : generates
  USER_DEVICE |o--o{ SECURITY_EVENT : from_device

  %% Admin/Work Queue/Evidences
  USER ||--o{ SYSTEM_CONFIG : config_changes
  USER ||--o{ WORK_ITEM : creates_work
  TERRITORY |o--o{ WORK_ITEM : scopes_work
  USER ||--o{ DOCUMENT_EVIDENCE : submits_evidence
  TERRITORY |o--o{ DOCUMENT_EVIDENCE : scopes_evidence
