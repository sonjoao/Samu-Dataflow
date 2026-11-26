using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAMUDataFlow.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class DetalhesOcorrenciaERBAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Papel",
                table: "usuarios",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Administrador 1");

            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Acompanhante",
                table: "ocorrencias",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnotacoesParamedicos",
                table: "ocorrencias",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Desfecho",
                table: "ocorrencias",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HospitalDestino",
                table: "ocorrencias",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacoesExtras",
                table: "ocorrencias",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Login",
                table: "usuarios",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_usuarios_Login",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Papel",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "Acompanhante",
                table: "ocorrencias");

            migrationBuilder.DropColumn(
                name: "AnotacoesParamedicos",
                table: "ocorrencias");

            migrationBuilder.DropColumn(
                name: "Desfecho",
                table: "ocorrencias");

            migrationBuilder.DropColumn(
                name: "HospitalDestino",
                table: "ocorrencias");

            migrationBuilder.DropColumn(
                name: "ObservacoesExtras",
                table: "ocorrencias");
        }
    }
}
