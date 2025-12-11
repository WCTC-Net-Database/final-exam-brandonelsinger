using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    /// <summary>
    /// Undead monster type - reanimated creatures dealing necrotic damage.
    /// Deals standard aggression-based damage with a chilling touch.
    /// </summary>
    public class Undead : Monster
    {
        /// <summary>
        /// Undead attack - bone-chilling necrotic touch attack.
        /// </summary>
        /// <param name="target">The entity being attacked</param>
        /// <returns>Combat log message describing the necrotic attack</returns>
        public override string Attack(ITargetable target)
        {
            int damage = AggressionLevel;
            int actualDamage = target.ReceiveAttack(damage);
            return $"{Name} strikes {target.Name} with a bone-chilling touch for {actualDamage} necrotic damage!";
        }
    }
}