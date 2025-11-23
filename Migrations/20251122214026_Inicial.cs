using Microsoft.EntityFrameworkCore.Migrations;

namespace FinTrack.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migration intencionalmente vazia.
            // Esta migration existia apenas no ambiente de desenvolvimento
            // e o banco já possui todas as tabelas criadas anteriormente.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down vazio — não desfaz nada.
        }
    }
}
