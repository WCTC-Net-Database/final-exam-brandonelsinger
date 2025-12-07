using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "Distance",
                table: "Abilities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Damage",
                table: "Abilities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Distance",
                table: "Abilities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Damage",
                table: "Abilities",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
