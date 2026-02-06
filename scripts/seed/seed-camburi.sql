-- Ingestão de dados: território Camburi (São Sebastião, SP) + conteúdo de exemplo
-- Execução: psql -h <host> -U <user> -d <database> -f seed-camburi.sql
-- Ou via script: scripts/seed/run-seed-camburi.ps1 (chama API do território e opcionalmente aplica este SQL)
-- Encoding: garante UTF-8 para acentuação (ã, ó, ç) correta.
SET client_encoding = 'UTF8';

-- 1) Território Camburi: insere só se ainda não existir (id fixo para referência).
-- Polígono: 9 vértices da região (lat, lng).
INSERT INTO territories (
    "Id", "ParentTerritoryId", "Name", "Description", "Status", "City", "State",
    "Latitude", "Longitude", "CreatedAtUtc", "RadiusKm", "BoundaryPolygonJson"
)
SELECT
    'b2222222-2222-2222-2222-222222222222'::uuid,
    NULL,
    'Camburi',
    'Praia e bairro de Camburi, São Sebastião, SP. Perímetro definido pelo polígono da região.',
    2,  -- TerritoryStatus.Active
    'São Sebastião',
    'SP',
    -23.76281,
    -45.63691,
    NOW() AT TIME ZONE 'UTC',
    3.5,
    '[{"Latitude":-23.77712,"Longitude":-45.66050},{"Latitude":-23.76220,"Longitude":-45.66387},{"Latitude":-23.74968,"Longitude":-45.66118},{"Latitude":-23.73371,"Longitude":-45.62439},{"Latitude":-23.75068,"Longitude":-45.60883},{"Latitude":-23.76304,"Longitude":-45.60608},{"Latitude":-23.77456,"Longitude":-45.62988},{"Latitude":-23.78168,"Longitude":-45.63744},{"Latitude":-23.78252,"Longitude":-45.65002}]'::jsonb
WHERE NOT EXISTS (
    SELECT 1 FROM territories
    WHERE "Name" = 'Camburi' AND "City" = 'São Sebastião' AND "State" = 'SP'
);

-- 2) Usuário seed "Comunidade Camburi" (id fixo)
INSERT INTO users (
    "Id", "DisplayName", "Email", "Cpf", "ForeignDocument", "PhoneNumber", "Address",
    "AuthProvider", "ExternalId", "TwoFactorEnabled", "TwoFactorSecret", "TwoFactorRecoveryCodesHash",
    "TwoFactorVerifiedAtUtc", "IdentityVerificationStatus", "IdentityVerifiedAtUtc",
    "AvatarMediaAssetId", "Bio", "CreatedAtUtc"
)
VALUES (
    'a1111111-1111-1111-1111-111111111111'::uuid,
    'Comunidade Camburi',
    'seed@araponga.local',
    NULL,
    'SEED',
    NULL,
    NULL,
    'seed',
    'seed-camburi',
    false,
    NULL,
    NULL,
    NULL,
    1,   -- UserIdentityVerificationStatus.Unverified
    NULL,
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC'
)
ON CONFLICT ("Id") DO NOTHING;

-- 3) Membership do usuário seed no território Camburi
INSERT INTO territory_memberships (
    "Id", "UserId", "TerritoryId", "Role", "ResidencyVerification",
    "LastGeoVerifiedAtUtc", "LastDocumentVerifiedAtUtc", "CreatedAtUtc", "RowVersion"
)
SELECT
    'c3333333-3333-3333-3333-333333333333'::uuid,
    'a1111111-1111-1111-1111-111111111111'::uuid,
    t."Id",
    2,   -- MembershipRole.Resident
    0,   -- ResidencyVerification.None
    NULL,
    NULL,
    NOW() AT TIME ZONE 'UTC',
    decode('0000000000000000', 'hex')
FROM territories t
WHERE t."Name" = 'Camburi' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
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
    'a1111111-1111-1111-1111-111111111111'::uuid,
    title,
    content,
    1,  -- PostType.General
    1,  -- PostVisibility.Public
    0,  -- PostStatus.Published
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
    ('d4444444-4444-4444-4444-444444444441'::uuid, 'Bem-vindos a Camburi',
     'Praia e bairro de Camburi, em São Sebastião. Este é o espaço da comunidade para compartilhar notícias, eventos e dicas da região.',
     (NOW() AT TIME ZONE 'UTC') - INTERVAL '2 days'),
    ('d4444444-4444-4444-4444-444444444442'::uuid, 'Dica: melhores horários para a praia',
     'Pela manhã o mar costuma estar mais calmo. À tarde a onda é melhor para o surf. Sempre use protetor solar!',
     (NOW() AT TIME ZONE 'UTC') - INTERVAL '1 day'),
    ('d4444444-4444-4444-4444-444444444443'::uuid, 'Feira de artesanato aos sábados',
     'Aos sábados temos feira de artesanato na orla. Venha conhecer o trabalho dos artistas locais.',
     NOW() AT TIME ZONE 'UTC')
) AS v(id, title, content, created)
WHERE t."Name" = 'Camburi' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
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
    'a1111111-1111-1111-1111-111111111111'::uuid,
    'Resident',
    'Scheduled',
    NOW() AT TIME ZONE 'UTC',
    NOW() AT TIME ZONE 'UTC',
    decode('0000000000000000', 'hex')
FROM territories t
CROSS JOIN (VALUES
    ('e5555555-5555-5555-5555-555555555551'::uuid,
     'Mutirão de limpeza da praia',
     'Encontro na praia de Camburi para coleta de resíduos. Traga luvas e sacos. Café da manhã compartilhado depois.',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '7 days')::date + TIME '09:00',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '7 days')::date + TIME '12:00',
     -23.76281, -45.63691, 'Praia de Camburi'),
    ('e5555555-5555-5555-5555-555555555552'::uuid,
     'Pôr do sol na orla',
     'Encontro informal para ver o pôr do sol. Traga sua cadeira e um lanche.',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '14 days')::date + TIME '18:00',
     ((NOW() AT TIME ZONE 'UTC') + INTERVAL '14 days')::date + TIME '20:00',
     -23.768, -45.642, 'Orla de Camburi')
) AS v(id, title, description, starts, ends, lat, lon, location)
WHERE t."Name" = 'Camburi' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
  AND NOT EXISTS (SELECT 1 FROM territory_events e WHERE e."Id" = v.id);

-- 6) Alertas de saúde no território (base mínima para testar front)
INSERT INTO health_alerts ("Id", "TerritoryId", "ReporterUserId", "Title", "Description", "Status", "CreatedAtUtc")
SELECT id, t."Id", 'a1111111-1111-1111-1111-111111111111'::uuid, title, description, 2, created
FROM territories t
CROSS JOIN (VALUES
    ('f4444444-4444-4444-4444-444444444441'::uuid, 'Qualidade da água na praia', 'Monitoramento indica condições normais para banho nesta semana. Evite áreas próximas à foz após chuvas fortes.', (NOW() AT TIME ZONE 'UTC') - INTERVAL '1 day'),
    ('f4444444-4444-4444-4444-444444444442'::uuid, 'Mutirão de limpeza', 'Alerta positivo: mutirão programado para o próximo sábado. Participe!', NOW() AT TIME ZONE 'UTC')
) AS v(id, title, description, created)
WHERE t."Name" = 'Camburi' AND t."City" = 'São Sebastião' AND t."State" = 'SP'
  AND NOT EXISTS (SELECT 1 FROM health_alerts a WHERE a."Id" = v.id);
