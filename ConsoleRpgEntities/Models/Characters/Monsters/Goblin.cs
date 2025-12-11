using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    /// <summary>
    /// Goblin monster type - sneaky creatures that ambush players.
    /// Inherits from Monster base class using TPH inheritance.
    /// </summary>
    public class Goblin : Monster
    {
        /// <summary>
        /// Goblin-specific attribute affecting stealth and surprise attacks
        /// </summary>
        public int Sneakiness { get; set; }

        /// <summary>
        /// Goblin attack - deals damage based on aggression level with a sneaky flavor.
        /// </summary>
        /// <param name="target">The entity being attacked</param>
        /// <returns>Combat log message describing the sneak attack</returns>
        public override string Attack(ITargetable target)
        {
            int damage = AggressionLevel;
            int actualDamage = target.ReceiveAttack(damage);
            return $"{Name} sneaks up and attacks {target.Name} for {actualDamage} damage!";
        }
    }
}