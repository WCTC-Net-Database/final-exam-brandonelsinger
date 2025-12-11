using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    /// <summary>
    /// Interface defining core monster behavior.
    /// All monster types must implement this interface.
    /// </summary>
    public interface IMonster
    {
        /// <summary>Unique monster identifier</summary>
        int Id { get; set; }

        /// <summary>Monster display name</summary>
        string Name { get; set; }

        /// <summary>
        /// Perform an attack on a target.
        /// Each monster type has unique attack behavior.
        /// </summary>
        /// <param name="target">The entity being attacked</param>
        /// <returns>Combat log message describing the attack</returns>
        string Attack(ITargetable target);
    }
}