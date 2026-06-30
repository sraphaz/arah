-- Massa de socorro: território na região central de São Paulo (capital)
-- Para quem testa longe do litoral (Camburi/Boiçucanga ~100 km).
-- Centro: Av. Paulista / Sé (-23.55052, -46.63331). Raio: 12 km.
-- Execução: scripts/seed/run-seed-territory-local.ps1 -Preset sao-paulo-centro
--           ou via run-local-stack.ps1 (aplicado automaticamente após camburi/boicucanga)
SET client_encoding = 'UTF8';

INSERT INTO territories (
    "Id", "ParentTerritoryId", "Name", "Description", "Status", "City", "State",
    "Latitude", "Longitude", "CreatedAtUtc", "RadiusKm", "BoundaryPolygonJson"
)
SELECT
    'd4444444-4444-4444-4444-444444444444'::uuid,
    NULL,
    'Centro',
    'Massa de socorro — região central de São Paulo (dev/local). Paulista, Sé, Pinheiros, Vila Mariana.',
    2,
    'São Paulo',
    'SP',
    -23.55052,
    -46.63331,
    NOW() AT TIME ZONE 'UTC',
    12.0,
    NULL
WHERE NOT EXISTS (
    SELECT 1 FROM territories
    WHERE "Name" = 'Centro' AND "City" = 'São Paulo' AND "State" = 'SP'
);

INSERT INTO users (
    "Id", "DisplayName", "Email", "Cpf", "ForeignDocument", "PhoneNumber", "Address",
    "AuthProvider", "ExternalId", "TwoFactorEnabled", "TwoFactorSecret", "TwoFactorRecoveryCodesHash",
    "TwoFactorVerifiedAtUtc", "IdentityVerificationStatus", "IdentityVerifiedAtUtc",
    "AvatarMediaAssetId", "Bio", "CreatedAtUtc"
)
VALUES (
    'a4444444-4444-4444-4444-444444444444'::uuid,
    'Comunidade Centro SP',
    'seed-centro-sp@arah.local',
    NULL,
    'SEED',
    NULL,
    NULL,
    'seed',
    'seed-centro-sp',
    false,
    NULL,
    NULL,
    NULL,
    1,
    NULL,
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC'
)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO territory_memberships (
    "Id", "UserId", "TerritoryId", "Role", "ResidencyVerification",
    "LastGeoVerifiedAtUtc", "LastDocumentVerifiedAtUtc", "CreatedAtUtc", "RowVersion"
)
SELECT
    'e4444444-4444-4444-4444-444444444444'::uuid,
    'a4444444-4444-4444-4444-444444444444'::uuid,
    t."Id",
    2,
    0,
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC',
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = 'Centro' AND t."City" = 'São Paulo' AND t."State" = 'SP'
LIMIT 1
ON CONFLICT ("UserId", "TerritoryId") DO NOTHING;

INSERT INTO community_posts (
    "Id", "TerritoryId", "AuthorUserId", "Title", "Content", "Type", "Visibility", "Status",
    "MapEntityId", "ReferenceType", "ReferenceId", "CreatedAtUtc", "EditedAtUtc", "EditCount", "TagsJson", "RowVersion"
)
SELECT
    'f5555555-5555-5555-5555-555555555551'::uuid,
    t."Id",
    'a4444444-4444-4444-4444-444444444444'::uuid,
    'Bem-vindo ao Centro',
    'Território de socorro para testes locais na capital. Valide onboarding e feed aqui.',
    1,
    1,
    0,
    NULL,
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC',
    NULL,
    0,
    '["dev","socorro"]'::jsonb,
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = 'Centro' AND t."City" = 'São Paulo' AND t."State" = 'SP'
  AND NOT EXISTS (SELECT 1 FROM community_posts p WHERE p."Id" = 'f5555555-5555-5555-5555-555555555551'::uuid);
