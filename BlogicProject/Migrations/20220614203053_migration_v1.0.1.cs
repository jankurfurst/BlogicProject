using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogicProject.Migrations
{
    public partial class migration_v101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Contract");

            migrationBuilder.AlterColumn<string>(
                name: "PI_Number",
                table: "Users",
                type: "varchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "Contract",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contract_InstitutionId",
                table: "Contract",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Institution_InstitutionId",
                table: "Contract",
                column: "InstitutionId",
                principalTable: "Institution",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Institution_InstitutionId",
                table: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Contract_InstitutionId",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Contract");

            migrationBuilder.AlterColumn<string>(
                name: "PI_Number",
                table: "Users",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldMaxLength: 11)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "Contract",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
