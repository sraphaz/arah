-- Ingestão de dados: território Boiçucanga (São Sebastião, SP) + feed, eventos e alertas para teste do front
-- Polígono: 11 vértices. Centroide: -23.77613, -45.59439
-- Execução: via run-local-stack.ps1 (após seed-camburi) ou:
--   Get-Content scripts\seed\seed-boicucanga.sql -Raw -Encoding UTF8 | docker exec -i -e PGCLIENTENCODING=UTF8 araponga-postgres psql -U araponga -d araponga
SET client_encoding = 'UTF8';

-- 1) Território Boiçucanga
INSERT INTO territories (
    "Id", "ParentTerritoryId", "Name", "Description", "Status", "City", "State",
    "Latitude", "Longitude", "CreatedAtUtc", "RadiusKm", "BoundaryPolygonJson"
)
SELECT
    'c3333333-3333-3333-3333-333333333334'::uuid,
    NULL,
    'Boiçucanga',
    'Praia e bairro de Boiçucanga, São Sebastião, SP. Perímetro definido pelo polígono da região.',
    2,
    'São Sebastião',
    'SP',
    -23.77613,
    -45.59439,
    NOW() AT TIME ZONE 'UTC',
    6.0,
    '[{"Latitude":-23.78328,"Longitude":-45.64631},{"Latitude":-23.80207,"Longitude":-45.64396},{"Latitude":-23.80883,"Longitude":-45.61649},{"Latitude":-23.80626,"Longitude":-45.59984},{"Latitude":-23.78699,"Longitude":-45.59423},{"Latitude":-23.77657,"Longitude":-45.58737},{"Latitude":-23.76746,"Longitude":-45.58084},{"Latitude":-23.75594,"Longitude":-45.55607},{"Latitude":-23.75317,"Longitude":-45.54480},{"Latitude":-23.74300,"Longitude":-45.56231},{"Latitude":-23.75903,"Longitude":-45.61008}]'::jsonb
WHERE NOT EXISTS (
    SELECT 1 FROM territories
    WHERE "Name" = 'Boiçucanga' AND "City" = 'São Sebastião' AND "State" = 'SP'
);

-- 2) Usuário seed "Comunidade Boiçucanga"
INSERT INTO users (
    "Id", "DisplayName", "Email", "Cpf", "ForeignDocument", "PhoneNumber", "Address",
    "AuthProvider", "ExternalId", "TwoFactorEnabled", "TwoFactorSecret", "TwoFactorRecoveryCodesHash",
    "TwoFactorVerifiedAtUtc", "IdentityVerificationStatus", "IdentityVerifiedAtUtc",
    "AvatarMediaAssetId", "Bio", "CreatedAtUtc"
)
VALUES (
    'a2222222-2222-2222-2222-222222222222'::uuid,
    'Comunidade Boiçucanga',
    'seed-boicucanga@araponga.local',
    NULL,
    'SEED',
    NULL,
    NULL,
    'seed',
    'seed-boicucanga',
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

-- 3) Membership do usuário seed no território Boiçucanga
INSERT INTO territory_memberships (
    "Id", "UserId", "TerritoryId", "Role", "ResidencyVerification",
    "LastGeoVerifiedAtUtc", "LastDocumentVerifiedAtUtc", "CreatedAtUtc", "RowVersion"
)
SELECT
    'c4444444-4444-4444-4444-444444444444'::uuid,
    'a2222222-2222-2222-2222-222222222222'::uuid,
    t."Id",
    2,
    0,
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC',
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = 'Boiçucanga' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
LIMIT 1
ON CONFLICT ("UserId", "TerritoryId") DO NOTHING;

-- 4) Posts no feed do território
INSERT INTO community_posts (
    "Id", "TerritoryId", "AuthorUserId", "Title", "Content", "Type", "Visibility", "Status",
    "MapEntityId", "ReferenceType", "ReferenceId", "CreatedAtUtc", "EditedAtUtc", "EditCount", "TagsJson", "RowVersion"
)
SELECT
    id,
    t."Id",
    'a2222222-2222-2222-2222-222222222222'::uuid,
    title,
    content,
    1,
    1,
    0,
    NULL,
    NULL,
    NULL,
    created,
    NULL,
    0,
    NULL,
    decode('0000000000000000', 'hex')
FROM territories t
CROSS JOIN (VALUES
    ('d5555555-5555-5555-5555-555555555551'::uuid, 'Bem-vindos a Boiçucanga',
     'Praia e bairro de Boiçucanga, em São Sebastião. Espaço da comunidade para notícias, eventos e dicas da região.',
     (NOW() AT TIME ZONE 'UTC') - INTERVAL '2 days'),
    ('d5555555-5555-5555-5555-555555555552'::uuid, 'Trilhas e cachoeiras',
     'A região oferece trilhas e cachoeiras. Consulte sempre o estado das trilhas antes de sair. Leve água e protetor solar.',
     (NOW() AT TIME ZONE 'UTC') - INTERVAL '1 day'),
    ('d5555555-5555-5555-5555-555555555553'::uuid, 'Horário do bar na orla',
     'O bar da orla abre aos fins de semana a partir das 10h. Venha tomar um caldo de cana!',
     NOW() AT TIME ZONE 'UTC')
) AS v(id, title, content, created)
WHERE t."Name" = 'Boiçucanga' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
  AND NOT EXISTS (SELECT 1 FROM community_posts p WHERE p."Id" = v.id);

-- 5) Eventos no território
INSERT INTO territory_events (
    "Id", "TerritoryId", "Title", "Description", "StartsAtUtc", "EndsAtUtc",
    "Latitude", "Longitude", "LocationLabel", "CreatedByUserId", "CreatedByMembership",
    "Status", "CreatedAtUtc", "UpdatedAtUtc", "RowVersion"
)
SELECT
    id,
    t."Id",
    title,
    description,
    starts,
    ends,
    lat,
    lon,
    location,
    'a2222222-2222-2222-2222-222222222222'::uuid,
    'Resident',
    'Scheduled',
    NOW() AT TIME ZONE 'UTC',
    NOW() AT TIME ZONE 'UTC',
    decode('0000000000000000', 'hex')
FROM territories t
CROSS JOIN (VALUES
    ('e6666666-6666-6666-6666-666666666661'::uuid,
     'Caminhada na praia',
     'Caminhada matinal na orla de Boiçucanga. Encontro no quiosque central.',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '5 days')::date + TIME '07:00',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '5 days')::date + TIME '08:30',
     -23.77613, -45.59439, 'Orla de Boiçucanga'),
    ('e6666666-6666-6666-6666-666666666662'::uuid,
     'Feijoada na comunidade',
     'Feijoada comunitária no salão da igreja. Contribuição voluntária.',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '10 days')::date + TIME '12:00',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '10 days')::date + TIME '16:00',
     -23.772, -45.592, 'Salão comunitário')
) AS v(id, title, description, starts, ends, lat, lon, location)
WHERE t."Name" = 'Boiçucanga' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
  AND NOT EXISTS (SELECT 1 FROM territory_events e WHERE e."Id" = v.id);

-- 6) Alertas de saúde no território
INSERT INTO health_alerts ("Id", "TerritoryId", "ReporterUserId", "Title", "Description", "Status", "CreatedAtUtc")
SELECT id, t."Id", 'a2222222-2222-2222-2222-222222222222'::uuid, title, description, 2, created
FROM territories t
CROSS JOIN (VALUES
    ('f5555555-5555-5555-5555-555555555551'::uuid, 'Condição do mar',
     'Ondas fortes previstas para esta semana. Evite nadar em áreas sem vigilância.', (NOW() AT TIME ZONE 'UTC') - INTERVAL '1 day'),
    ('f5555555-5555-5555-5555-555555555552'::uuid, 'Trilha do Mirante',
     'Trilha liberada. Leve calçado adequado e água. Previsão de sol.', NOW() AT TIME ZONE 'UTC')
) AS v(id, title, description, created)
WHERE t."Name" = 'Boiçucanga' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
  AND NOT EXISTS (SELECT 1 FROM health_alerts a WHERE a."Id" = v.id);
