-- Marca a migração unificada como já aplicada (banco já tem tabelas parciais)
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260205204020_UnifiedInitialCreate', '8.0.8')
ON CONFLICT ("MigrationId") DO NOTHING;
