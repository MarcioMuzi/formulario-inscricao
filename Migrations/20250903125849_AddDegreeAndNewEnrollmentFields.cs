using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CA1814

namespace WebApp.Migrations
{
    public partial class AddDegreeAndNewEnrollmentFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ===== Identity: soltar PKs antes de alterar tamanho das colunas =====
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey", "UserId" });

            // ===== Novas colunas em Enrollments =====
            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Enrollments",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DegreeId",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 1); // GRADUAÇÃO

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Enrollments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Outro");

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "Enrollments",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Organization",
                table: "Enrollments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // ===== Tabela de domínio: Degrees + seed =====
            migrationBuilder.CreateTable(
                name: "Degrees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                              .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degrees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Degrees",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "GRADUAÇÃO" },
                    { 2, "ESPECIALIZAÇÃO" },
                    { 3, "DOUTORADO" },
                    { 4, "PÓS-DOUTORADO" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_DegreeId",
                table: "Enrollments",
                column: "DegreeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Degrees_DegreeId",
                table: "Enrollments",
                column: "DegreeId",
                principalTable: "Degrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // FK e colunas adicionadas
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Degrees_DegreeId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_DegreeId",
                table: "Enrollments");

            migrationBuilder.DropTable(
                name: "Degrees");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "DegreeId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "Organization",
                table: "Enrollments");

            // Identity: soltar PKs, reverter tamanho, recriar PKs
            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey", "UserId" });
        }
    }
}
