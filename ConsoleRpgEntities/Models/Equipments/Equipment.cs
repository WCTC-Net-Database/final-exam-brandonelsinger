using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleRpgEntities.Models.Equipments
{
    /// <summary>
    /// Represents a player's equipped gear slots.
    /// Links a player to their currently equipped weapon and armor.
    /// Each player has one Equipment record with optional weapon/armor slots.
    /// </summary>
    public class Equipment
    {
        // ===== Primary Key =====
        public int Id { get; set; }

        // ===== Foreign Keys =====
        // Nullable to allow empty equipment slots and avoid cascade delete issues
        /// <summary>ID of the equipped weapon (null if no weapon equipped)</summary>
        public int? WeaponId { get; set; }

        /// <summary>ID of the equipped armor (null if no armor equipped)</summary>
        public int? ArmorId { get; set; }

        // ===== Navigation Properties =====
        /// <summary>The currently equipped weapon item</summary>
        [ForeignKey("WeaponId")]
        public virtual Item Weapon { get; set; }

        /// <summary>The currently equipped armor item</summary>
        [ForeignKey("ArmorId")]
        public virtual Item Armor { get; set; }
    }
}