using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    /// <inheritdoc />
    public partial class AddLootTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LootItemId",
                table: "Monsters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_LootItemId",
                table: "Monsters",
                column: "LootItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_RoomId",
                table: "Items",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Rooms_RoomId",
                table: "Items",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Monsters_Items_LootItemId",
                table: "Monsters",
                column: "LootItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.Sql(
                "UPDATE Monsters SET LootItemId = (SELECT Id FROM Items WHERE Name = 'Steel Longsword') WHERE Name = 'Goblin Warrior'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Rooms_RoomId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Monsters_Items_LootItemId",
                table: "Monsters");

            migrationBuilder.DropIndex(
                name: "IX_Monsters_LootItemId",
                table: "Monsters");

            migrationBuilder.DropIndex(
                name: "IX_Items_RoomId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "LootItemId",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Items");
        }
    }
}
