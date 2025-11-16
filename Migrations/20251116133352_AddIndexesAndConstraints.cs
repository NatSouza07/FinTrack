using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_TiposPagamento_TipoPagamentoId",
                table: "Transacoes");

            migrationBuilder.CreateIndex(
                name: "IX_TiposPagamento_Nome",
                table: "TiposPagamento",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nome_Tipo",
                table: "Categorias",
                columns: new[] { "Nome", "Tipo" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_TiposPagamento_TipoPagamentoId",
                table: "Transacoes",
                column: "TipoPagamentoId",
                principalTable: "TiposPagamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_TiposPagamento_TipoPagamentoId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_TiposPagamento_Nome",
                table: "TiposPagamento");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_Nome_Tipo",
                table: "Categorias");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_TiposPagamento_TipoPagamentoId",
                table: "Transacoes",
                column: "TipoPagamentoId",
                principalTable: "TiposPagamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
