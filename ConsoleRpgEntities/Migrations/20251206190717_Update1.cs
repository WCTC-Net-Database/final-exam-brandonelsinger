using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AbilityType",
                table: "Abilities",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<int>(
                name: "FireballAbility_Damage",
                table: "Abilities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealAbility_Damage",
                table: "Abilities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShoveAbility_Damage",
                table: "Abilities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShoveAbility_Distance",
                table: "Abilities",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FireballAbility_Damage",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "HealAbility_Damage",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "ShoveAbility_Damage",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "ShoveAbility_Distance",
                table: "Abilities");

            migrationBuilder.AlterColumn<string>(
                name: "AbilityType",
                table: "Abilities",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
