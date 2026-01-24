using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Araponga.Infrastructure.Postgres.Migrations;

/// <inheritdoc />
public partial class AddPostTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Adicionar coluna TagsJson como JSONB
        migrationBuilder.AddColumn<string>(
            name: "TagsJson",
            table: "community_posts",
            type: "jsonb",
            nullable: true);

        // Criar índice GIN para busca eficiente em tags
        migrationBuilder.CreateIndex(
            name: "IX_community_posts_TagsJson",
            table: "community_posts",
            column: "TagsJson")
            .Annotation("Npgsql:IndexMethod", "gin")
            .Annotation("Npgsql:IndexOperators", new[] { "jsonb_path_ops" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_community_posts_TagsJson",
            table: "community_posts");

        migrationBuilder.DropColumn(
            name: "TagsJson",
            table: "community_posts");
    }
}
