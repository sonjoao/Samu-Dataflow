using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SAMUDataFlow.Infrastructure.Persistencia.Migrations
{
    public partial class HistoricoConfiguracoesArquivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuracoes_sistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeServico = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Municipio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UnidadeFederativa = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    DiasPadraoHistorico = table.Column<int>(type: "integer", nullable: false),
                    AtualizadoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoPor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                }, constraints: table => table.PrimaryKey("PK_configuracoes_sistema", x => x.Id));

            migrationBuilder.CreateTable(
                name: "eventos_historicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoRegistro = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    RegistroId = table.Column<int>(type: "integer", nullable: true),
                    CodigoOcorrencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Acao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Responsavel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Detalhes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                }, constraints: table => table.PrimaryKey("PK_eventos_historicos", x => x.Id));

            migrationBuilder.InsertData(
                table: "configuracoes_sistema",
                columns: new[] { "Id", "AtualizadoEmUtc", "AtualizadoPor", "DiasPadraoHistorico", "Municipio", "NomeServico", "UnidadeFederativa" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sistema", 30, "Recife", "SAMU DataFlow", "PE" });

            migrationBuilder.CreateIndex(name: "IX_eventos_historicos_CodigoOcorrencia", table: "eventos_historicos", column: "CodigoOcorrencia");
            migrationBuilder.CreateIndex(name: "IX_eventos_historicos_CriadoEmUtc", table: "eventos_historicos", column: "CriadoEmUtc");

            migrationBuilder.DropIndex(name: "IX_ocorrencias_CodigoExterno", table: "ocorrencias");
            migrationBuilder.AddColumn<bool>(name: "EstaArquivada", table: "ocorrencias", type: "boolean", nullable: false, defaultValue: false);
            migrationBuilder.CreateIndex(name: "IX_ocorrencias_CodigoExterno", table: "ocorrencias", column: "CodigoExterno", unique: true, filter: "\"EstaArquivada\" = false");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_ocorrencias_CodigoExterno", table: "ocorrencias");
            migrationBuilder.DropColumn(name: "EstaArquivada", table: "ocorrencias");
            migrationBuilder.CreateIndex(name: "IX_ocorrencias_CodigoExterno", table: "ocorrencias", column: "CodigoExterno", unique: true);
            migrationBuilder.DropTable(name: "configuracoes_sistema");
            migrationBuilder.DropTable(name: "eventos_historicos");
        }
    }
}
