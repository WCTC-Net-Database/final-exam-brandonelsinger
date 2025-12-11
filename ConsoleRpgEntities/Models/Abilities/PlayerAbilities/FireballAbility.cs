using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    /// <summary>
    /// Magical ability that hurls a ball of fire at enemies.
    /// Deals fire damage based on the Damage property.
    /// </summary>
    public class FireballAbility : Ability
    {
        /// <summary>
        /// Casts a fireball at the target, dealing fire damage.
        /// </summary>
        /// <param name="user">The player casting the spell</param>
        /// <param name="target">The entity being burned</param>
        /// <returns>Combat log message describing the fiery attack</returns>
        public override string Activate(IPlayer user, ITargetable target)
        {
            // Apply fire damage through target's defense calculation
            int actualDamage = target.ReceiveAttack(Damage);
            return $"{user.Name} casts Fireball! {target.Name} takes {actualDamage} fire damage and is singed!";
        }
    }
}
