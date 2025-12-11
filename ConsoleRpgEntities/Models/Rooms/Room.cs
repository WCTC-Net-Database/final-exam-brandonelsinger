using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Equipments;

namespace ConsoleRpgEntities.Models.Rooms
{
    /// <summary>
    /// Represents a location in the game world that players can explore.
    /// Rooms are connected via directional navigation (North, South, East, West).
    /// Contains collections of players, monsters, and items present in the room.
    /// </summary>
    public class Room : IRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Coordinates for map display
        public int X { get; set; }
        public int Y { get; set; }

        // Exit foreign keys (nullable - rooms may not have all exits)
        public int? NorthRoomId { get; set; }
        public int? SouthRoomId { get; set; }
        public int? EastRoomId { get; set; }
        public int? WestRoomId { get; set; }

        // Navigation properties for exits
        public virtual Room NorthRoom { get; set; }
        public virtual Room SouthRoom { get; set; }
        public virtual Room EastRoom { get; set; }
        public virtual Room WestRoom { get; set; }

        // Collections of entities in the room
        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Monster> Monsters { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public Room()
        {
            Items = new List<Item>();
        }
    }
}
