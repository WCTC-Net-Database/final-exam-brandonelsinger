using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    /// <summary>
    /// Abstract base class for all player abilities.
    /// Uses Table-Per-Hierarchy (TPH) inheritance with AbilityType as the discriminator.
    /// Abilities are linked to players via a many-to-many relationship (PlayerAbilities join table).
    /// </summary>
    public abstract class Ability : IAbility
    {
        // ===== Primary Key =====
        public int Id { get; set; }

        // ===== Ability Information =====
        /// <summary>Display name of the ability</summary>
        public string Name { get; set; }

        /// <summary>Flavor text describing what the ability does</summary>
        public string Description { get; set; }

        /// <summary>TPH discriminator - identifies concrete ability type</summary>
        public string AbilityType { get; set; }

        // ===== Ability Stats =====
        /// <summary>Base damage or healing amount (negative for heals in some implementations)</summary>
        public int Damage { get; set; }

        /// <summary>Range or knockback distance in feet</summary>
        public int Distance { get; set; }

        // ===== Navigation Properties =====
        /// <summary>Players who have learned this ability (many-to-many)</summary>
        public virtual ICollection<Player> Players { get; set; }

        /// <summary>
        /// Protected constructor - initializes empty players collection
        /// </summary>
        protected Ability()
        {
            Players = new List<Player>();
        }

        /// <summary>
        /// Abstract method to execute the ability's effect.
        /// Each concrete ability type implements its own activation logic.
        /// </summary>
        /// <param name="user">The player using the ability</param>
        /// <param name="target">The target receiving the ability's effect</param>
        /// <returns>Combat log message describing the ability's effect</returns>
        public abstract string Activate(IPlayer user, ITargetable target);
    }
}
