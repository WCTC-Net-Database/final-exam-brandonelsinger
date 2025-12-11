using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    /// <summary>
    /// Bandit monster type - humanoid enemies wielding rusty weapons.
    /// Deals bonus damage (+3) compared to base aggression level.
    /// </summary>
    public class Bandit : Monster
    {
        /// <summary>
        /// Bandit attack - rusty weapon strike with +3 bonus damage.
        /// </summary>
        /// <param name="target">The entity being attacked</param>
        /// <returns>Combat log message describing the weapon attack</returns>
        public override string Attack(ITargetable target)
        {
            // Bandits deal aggression + 3 bonus damage from their weapons
            int damage = AggressionLevel + 3;
            int actualDamage = target.ReceiveAttack(damage);
            return $"{Name} attacks {target.Name} with a rusty weapon dealing {actualDamage} damage!";
        }
    }
}
