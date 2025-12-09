using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    /// <inheritdoc />
    public partial class AddMonsterArmor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArmorClass",
                table: "Monsters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE Monsters SET ArmorClass = 2 WHERE MonsterType = 'Goblin'");
            migrationBuilder.Sql("UPDATE Monsters SET ArmorClass = 3 WHERE MonsterType = 'Beast'");
            migrationBuilder.Sql("UPDATE Monsters SET ArmorClass = 4 WHERE MonsterType = 'Bandit'");
            migrationBuilder.Sql("UPDATE Monsters SET ArmorClass = 5 WHERE MonsterType = 'Undead'");
            migrationBuilder.Sql("UPDATE Monsters SET ArmorClass = 0 WHERE Id = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmorClass",
                table: "Monsters");
        }
    }
}
