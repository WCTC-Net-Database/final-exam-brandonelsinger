using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;

namespace ConsoleRpgEntities.Models.Characters
{
    /// <summary>
    /// Represents a playable character in the game.
    /// Implements ITargetable to allow being targeted by attacks and abilities.
    /// Implements IPlayer to define core player behavior contracts.
    /// </summary>
    public class Player : ITargetable, IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int Level { get; set; } = 1;
        public int MaxHealth { get; set; } = 100;

        // Foreign keys
        public int? EquipmentId { get; set; }
        public int? RoomId { get; set; }
        public virtual Inventory Inventory { get; set; }

        // Navigation properties
        public virtual Equipment Equipment { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }
        public virtual Room Room { get; set; }

        public Player()
        {
            Abilities = new List<Ability>();
        }

        /// <summary>
        /// Calculates and applies damage to the player, factoring in armor defense.
        /// Part of the ITargetable interface implementation.
        /// </summary>
        /// <param name="damage">Raw incoming damage before defense calculation</param>
        /// <returns>Actual damage dealt after defense reduction</returns>
        public int ReceiveAttack(int damage)
        {
            // Calculate total defense from equipped armor and weapon
            int defense = (Equipment?.Armor?.Defense ?? 0) + (Equipment?.Weapon?.Defense ?? 0);

            // Ensure damage is never negative (defense can't heal)
            int actualDamage = Math.Max(0, damage - defense);

            // Apply damage to health
            Health -= actualDamage;

            return actualDamage;
        }

        /// <summary>
        /// Awards experience points and handles level-up logic.
        /// Players level up every 100 XP, gaining +10 max HP and full healing.
        /// </summary>
        /// <param name="amount">Amount of XP to award</param>
        /// <returns>Formatted message describing XP gain or level up</returns>
        public string GainExperience(int amount)
        {
            Experience += amount;

            // Calculate what level the player should be based on total XP
            int calculatedLevel = (Experience / 100) + 1;

            // Check if player leveled up
            if (calculatedLevel > Level)
            {
                Level = calculatedLevel;

                // Level up rewards: +10 max HP and full heal
                MaxHealth += 10; 
                Health = MaxHealth; 

                return $"[yellow bold]LEVEL UP![/] You are now Level {Level}! (Max HP increased to {MaxHealth})";
            }

            // Return progress toward next level
            return $"Gained {amount} XP. ({Experience % 100}/100 to next level)";
        }
        
    }
}
