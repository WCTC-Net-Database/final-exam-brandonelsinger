using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    /// <summary>
    /// Physical ability that pushes enemies back while dealing damage.
    /// Uses the Damage and Distance properties from the base Ability class.
    /// </summary>
    public class ShoveAbility : Ability
    {
        /// <summary>
        /// Executes a shove attack - deals damage and pushes the target back.
        /// </summary>
        /// <param name="user">The player performing the shove</param>
        /// <param name="target">The entity being shoved</param>
        /// <returns>Combat log message with damage dealt and knockback distance</returns>
        public override string Activate(IPlayer user, ITargetable target)
        {
            // Apply damage through target's defense calculation
            int actualDamage = target.ReceiveAttack(Damage);

            return $"{user.Name} shoves {target.Name} back {Distance} feet, dealing {actualDamage} damage!";
        }
    }
}
