using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    /// <summary>
    /// Healing ability that restores health to the target.
    /// Uses the absolute value of Damage as the heal amount.
    /// Respects the player's MaxHealth cap when healing players.
    /// </summary>
    public class HealAbility : Ability
    {
        /// <summary>
        /// Casts a healing spell on the target, restoring HP.
        /// If targeting a Player, healing is capped at MaxHealth.
        /// </summary>
        /// <param name="user">The player casting the heal</param>
        /// <param name="target">The entity being healed</param>
        /// <returns>Combat log message describing the healing effect</returns>
        public override string Activate(IPlayer user, ITargetable target)
        {
            // Use absolute value to ensure positive healing regardless of how Damage is stored
            int healAmount = Math.Abs(Damage);

            // Special handling for Player targets to respect MaxHealth cap
            if (target is Player p)
            {
                p.Health = Math.Min(p.MaxHealth, p.Health + healAmount);
            }
            else
            {
                // Non-player targets get healed without a cap
                target.Health += healAmount;
            }

            return $"{user.Name} casts a holy light. {target.Name} recovers {healAmount} HP!";
        }
    }
}
