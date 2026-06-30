-- Território: Socorro — Socorro/ (IBGE 3552106)
-- Fonte: IBGE Malhas v3 — limites legais do município (SIRGAS 2000)
-- Gerado por: scripts/seed/fetch-ibge-municipality-boundary.ps1
SET client_encoding = 'UTF8';

INSERT INTO territories (
    "Id", "ParentTerritoryId", "Name", "Description", "Status", "City", "State",
    "Latitude", "Longitude", "CreatedAtUtc", "RadiusKm", "BoundaryPolygonJson"
)
SELECT
    'de4010fe-af52-405f-9c53-b061a4557779'::uuid,
    NULL,
    'Socorro',
    'Perímetro oficial do município (malha IBGE 3552106, qualidade intermediaria, resolução 3). SIRGAS 2000.',
    2,
    'Socorro',
    'SP',
    -22.609906,
    -46.509602,
    NOW() AT TIME ZONE 'UTC',
    NULL,
    '[{"Latitude":-22.7231,"Longitude":-46.6218},{"Latitude":-22.7212,"Longitude":-46.537},{"Latitude":-22.7282,"Longitude":-46.5318},{"Latitude":-22.7253,"Longitude":-46.5243},{"Latitude":-22.7304,"Longitude":-46.5095},{"Latitude":-22.7276,"Longitude":-46.5068},{"Latitude":-22.7192,"Longitude":-46.4935},{"Latitude":-22.7248,"Longitude":-46.4884},{"Latitude":-22.7215,"Longitude":-46.4811},{"Latitude":-22.7149,"Longitude":-46.481},{"Latitude":-22.705,"Longitude":-46.4731},{"Latitude":-22.6978,"Longitude":-46.4786},{"Latitude":-22.694,"Longitude":-46.47},{"Latitude":-22.6859,"Longitude":-46.4786},{"Latitude":-22.6799,"Longitude":-46.4806},{"Latitude":-22.6732,"Longitude":-46.4691},{"Latitude":-22.6685,"Longitude":-46.4421},{"Latitude":-22.6614,"Longitude":-46.4277},{"Latitude":-22.6645,"Longitude":-46.4179},{"Latitude":-22.6599,"Longitude":-46.4134},{"Latitude":-22.6626,"Longitude":-46.4057},{"Latitude":-22.671,"Longitude":-46.4022},{"Latitude":-22.663,"Longitude":-46.3931},{"Latitude":-22.6547,"Longitude":-46.3923},{"Latitude":-22.646,"Longitude":-46.3963},{"Latitude":-22.6406,"Longitude":-46.406},{"Latitude":-22.6381,"Longitude":-46.4206},{"Latitude":-22.6336,"Longitude":-46.4246},{"Latitude":-22.6248,"Longitude":-46.423},{"Latitude":-22.6222,"Longitude":-46.406},{"Latitude":-22.6073,"Longitude":-46.4134},{"Latitude":-22.5994,"Longitude":-46.4243},{"Latitude":-22.5854,"Longitude":-46.4124},{"Latitude":-22.5841,"Longitude":-46.4274},{"Latitude":-22.5712,"Longitude":-46.4326},{"Latitude":-22.5579,"Longitude":-46.4146},{"Latitude":-22.5415,"Longitude":-46.4061},{"Latitude":-22.5367,"Longitude":-46.4117},{"Latitude":-22.5475,"Longitude":-46.4179},{"Latitude":-22.5476,"Longitude":-46.4263},{"Latitude":-22.5406,"Longitude":-46.4393},{"Latitude":-22.5347,"Longitude":-46.4395},{"Latitude":-22.532,"Longitude":-46.4472},{"Latitude":-22.5217,"Longitude":-46.4565},{"Latitude":-22.5185,"Longitude":-46.4669},{"Latitude":-22.5192,"Longitude":-46.4846},{"Latitude":-22.5104,"Longitude":-46.4912},{"Latitude":-22.5155,"Longitude":-46.5066},{"Latitude":-22.507,"Longitude":-46.507},{"Latitude":-22.4944,"Longitude":-46.5421},{"Latitude":-22.4878,"Longitude":-46.5361},{"Latitude":-22.4709,"Longitude":-46.5307},{"Latitude":-22.4678,"Longitude":-46.543},{"Latitude":-22.4695,"Longitude":-46.5499},{"Latitude":-22.4708,"Longitude":-46.5554},{"Latitude":-22.4853,"Longitude":-46.5631},{"Latitude":-22.487,"Longitude":-46.5774},{"Latitude":-22.4929,"Longitude":-46.5853},{"Latitude":-22.4991,"Longitude":-46.5858},{"Latitude":-22.5077,"Longitude":-46.5798},{"Latitude":-22.5163,"Longitude":-46.5843},{"Latitude":-22.515,"Longitude":-46.599},{"Latitude":-22.5249,"Longitude":-46.6075},{"Latitude":-22.5482,"Longitude":-46.6138},{"Latitude":-22.5643,"Longitude":-46.6035},{"Latitude":-22.5747,"Longitude":-46.6041},{"Latitude":-22.5872,"Longitude":-46.6114},{"Latitude":-22.6037,"Longitude":-46.61},{"Latitude":-22.6316,"Longitude":-46.6016},{"Latitude":-22.6369,"Longitude":-46.6079},{"Latitude":-22.6359,"Longitude":-46.6183},{"Latitude":-22.6492,"Longitude":-46.6209},{"Latitude":-22.6536,"Longitude":-46.629},{"Latitude":-22.6649,"Longitude":-46.6267},{"Latitude":-22.6745,"Longitude":-46.632},{"Latitude":-22.6835,"Longitude":-46.6204},{"Latitude":-22.6898,"Longitude":-46.6239},{"Latitude":-22.7017,"Longitude":-46.6184},{"Latitude":-22.7111,"Longitude":-46.626},{"Latitude":-22.718,"Longitude":-46.6271},{"Latitude":-22.7231,"Longitude":-46.6218}]'::jsonb
WHERE NOT EXISTS (
    SELECT 1 FROM territories
    WHERE "Name" = 'Socorro' AND "City" = 'Socorro' AND "State" = 'SP'
);

INSERT INTO users (
    "Id", "DisplayName", "Email", "AuthProvider", "ExternalId",
    "TwoFactorEnabled", "IdentityVerificationStatus", "CreatedAtUtc", "ForeignDocument"
)
VALUES (
    '2369212f-a62f-4d7f-a406-5f55071b70cd'::uuid,
    'Comunidade Socorro',
    'seed-ibge-3552106@arah.local',
    'seed',
    'seed-ibge-3552106',
    false,
    1,
    NOW() AT TIME ZONE 'UTC',
    'SEED'
)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO territory_memberships (
    "Id", "UserId", "TerritoryId", "Role", "ResidencyVerification", "CreatedAtUtc", "RowVersion"
)
SELECT
    '52ea984d-2a85-409b-8a59-512499da5497'::uuid,
    '2369212f-a62f-4d7f-a406-5f55071b70cd'::uuid,
    t."Id",
    2,
    0,
    NOW() AT TIME ZONE 'UTC',
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = 'Socorro' AND t."City" = 'Socorro' AND t."State" = 'SP'
LIMIT 1
ON CONFLICT ("UserId", "TerritoryId") DO NOTHING;

INSERT INTO community_posts (
    "Id", "TerritoryId", "AuthorUserId", "Title", "Content", "Type", "Visibility", "Status",
    "MapEntityId", "ReferenceType", "ReferenceId", "CreatedAtUtc", "EditedAtUtc", "EditCount", "TagsJson", "RowVersion"
)
SELECT
    '1049642d-701d-4e86-bb10-d556fa4a36fb'::uuid,
    t."Id",
    '2369212f-a62f-4d7f-a406-5f55071b70cd'::uuid,
    'Bem-vindo a Socorro',
    'Território com contorno oficial IBGE (3552106). Use para validar mapa, onboarding e feed.',
    1,
    1,
    0,
    NULL,
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC',
    NULL,
    0,
    '["ibge","oficial"]'::jsonb,
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = 'Socorro' AND t."City" = 'Socorro' AND t."State" = 'SP'
  AND NOT EXISTS (SELECT 1 FROM community_posts p WHERE p."TerritoryId" = t."Id" LIMIT 1);
