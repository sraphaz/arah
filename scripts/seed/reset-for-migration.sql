-- Remove tabelas app parciais e o registro da migração para a migração unificada rodar do zero
DELETE FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260205204020_UnifiedInitialCreate';
DROP TABLE IF EXISTS chat_messages CASCADE;
DROP TABLE IF EXISTS chat_conversation_participants CASCADE;
DROP TABLE IF EXISTS chat_conversation_stats CASCADE;
DROP TABLE IF EXISTS chat_conversations CASCADE;
