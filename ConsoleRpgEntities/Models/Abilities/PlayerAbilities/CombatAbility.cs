using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    /// <summary>
    /// Weapon-enhanced combat ability that adds bonus damage to weapon attacks.
    /// Combines the player's equipped weapon damage with the ability's bonus damage.
    /// </summary>
    public class CombatAbility : Ability
    {
        /// <summary>
        /// Executes a powerful weapon strike enhanced by the ability.
        /// Total damage = weapon attack + ability bonus damage.
        /// </summary>
        /// <param name="user">The player performing the attack</param>
        /// <param name="target">The entity being struck</param>
        /// <returns>Combat log message describing the enhanced attack</returns>
        public override string Activate(IPlayer user, ITargetable target)
        {
            int weaponDamage = 0;
            string weaponName = "Fists";

            // Get weapon stats if player has one equipped
            if (user is Player player && player.Equipment?.Weapon != null)
            {
                weaponDamage = player.Equipment.Weapon.Attack;
                weaponName = player.Equipment.Weapon.Name;
            }

            // Combine weapon damage with ability bonus
            int totalDamage = weaponDamage + Damage;
            int actualDamage = target.ReceiveAttack(totalDamage);

            return $"{user.Name} uses {Name} with their {weaponName}! A brutal strike dealing {actualDamage} physical damage!";
        }
    }
}
