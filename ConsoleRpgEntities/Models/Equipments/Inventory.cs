using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Equipments
{
    /// <summary>
    /// Represents a player's inventory (backpack) containing collected items.
    /// Each player has one inventory with a many-to-many relationship to items.
    /// </summary>
    public class Inventory
    {
        // ===== Primary Key =====
        public int Id { get; set; }

        // ===== Foreign Key =====
        /// <summary>The player who owns this inventory</summary>
        public int PlayerId { get; set; }

        // ===== Navigation Properties =====
        /// <summary>The player who owns this inventory</summary>
        public virtual Player Player { get; set; }

        /// <summary>Collection of items in the inventory (many-to-many with Items table)</summary>
        public virtual ICollection<Item> Items { get; set; }

        /// <summary>
        /// Default constructor - initializes empty items collection
        /// </summary>
        public Inventory()
        {
            Items = new List<Item>();
        }
    }
}