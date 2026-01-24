using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <summary>
/// Migration para adicionar índices GIN para full-text search no marketplace.
/// </summary>
public partial class AddFullTextSearchIndexes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Criar coluna tsvector para store_items (título + descrição)
        migrationBuilder.Sql(@"
            ALTER TABLE store_items 
            ADD COLUMN IF NOT EXISTS search_vector tsvector;
        ");

        // Criar função para atualizar search_vector
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION store_items_search_vector_update() 
            RETURNS trigger AS $$
            BEGIN
                NEW.search_vector := 
                    setweight(to_tsvector('portuguese', COALESCE(NEW.title, '')), 'A') ||
                    setweight(to_tsvector('portuguese', COALESCE(NEW.description, '')), 'B');
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

        // Criar trigger para atualizar search_vector automaticamente
        migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS store_items_search_vector_trigger ON store_items;
            CREATE TRIGGER store_items_search_vector_trigger
            BEFORE INSERT OR UPDATE ON store_items
            FOR EACH ROW
            EXECUTE FUNCTION store_items_search_vector_update();
        ");

        // Atualizar search_vector para registros existentes
        migrationBuilder.Sql(@"
            UPDATE store_items 
            SET search_vector = 
                setweight(to_tsvector('portuguese', COALESCE(title, '')), 'A') ||
                setweight(to_tsvector('portuguese', COALESCE(description, '')), 'B');
        ");

        // Criar índice GIN para busca full-text
        migrationBuilder.Sql(@"
            CREATE INDEX IF NOT EXISTS idx_store_items_search_vector 
            ON store_items USING GIN(search_vector);
        ");

        // Criar coluna tsvector para territory_stores (nome + descrição)
        migrationBuilder.Sql(@"
            ALTER TABLE territory_stores 
            ADD COLUMN IF NOT EXISTS search_vector tsvector;
        ");

        // Criar função para atualizar search_vector de stores
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION territory_stores_search_vector_update() 
            RETURNS trigger AS $$
            BEGIN
                NEW.search_vector := 
                    setweight(to_tsvector('portuguese', COALESCE(NEW.display_name, '')), 'A') ||
                    setweight(to_tsvector('portuguese', COALESCE(NEW.description, '')), 'B');
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

        // Criar trigger para stores
        migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS territory_stores_search_vector_trigger ON territory_stores;
            CREATE TRIGGER territory_stores_search_vector_trigger
            BEFORE INSERT OR UPDATE ON territory_stores
            FOR EACH ROW
            EXECUTE FUNCTION territory_stores_search_vector_update();
        ");

        // Atualizar search_vector para stores existentes
        migrationBuilder.Sql(@"
            UPDATE territory_stores 
            SET search_vector = 
                setweight(to_tsvector('portuguese', COALESCE(display_name, '')), 'A') ||
                setweight(to_tsvector('portuguese', COALESCE(description, '')), 'B');
        ");

        // Criar índice GIN para stores
        migrationBuilder.Sql(@"
            CREATE INDEX IF NOT EXISTS idx_territory_stores_search_vector 
            ON territory_stores USING GIN(search_vector);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Remover índices
        migrationBuilder.Sql(@"
            DROP INDEX IF EXISTS idx_store_items_search_vector;
            DROP INDEX IF EXISTS idx_territory_stores_search_vector;
        ");

        // Remover triggers
        migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS store_items_search_vector_trigger ON store_items;
            DROP TRIGGER IF EXISTS territory_stores_search_vector_trigger ON territory_stores;
        ");

        // Remover funções
        migrationBuilder.Sql(@"
            DROP FUNCTION IF EXISTS store_items_search_vector_update();
            DROP FUNCTION IF EXISTS territory_stores_search_vector_update();
        ");

        // Remover colunas
        migrationBuilder.Sql(@"
            ALTER TABLE store_items DROP COLUMN IF EXISTS search_vector;
            ALTER TABLE territory_stores DROP COLUMN IF EXISTS search_vector;
        ");
    }
}
