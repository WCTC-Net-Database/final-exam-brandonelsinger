using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    /// <summary>
    /// Beast monster type - wild animals that attack with feral rage.
    /// Deals bonus damage (+2) compared to base aggression level.
    /// </summary>
    public class Beast : Monster
    {
        /// <summary>
        /// Beast attack - fierce lunging attack with +2 bonus damage.
        /// </summary>
        /// <param name="target">The entity being attacked</param>
        /// <returns>Combat log message describing the feral attack</returns>
        public override string Attack(ITargetable target)
        {
            // Beasts deal aggression + 2 bonus damage
            int damage = AggressionLevel + 2;
            int actualDamage = target.ReceiveAttack(damage);
            return $"The {Name} lunges at {target.Name} with feral rage for {actualDamage} damage!";
        }
    }
}