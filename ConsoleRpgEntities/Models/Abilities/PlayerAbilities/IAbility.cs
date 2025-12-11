using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    /// <summary>
    /// Interface defining core ability behavior.
    /// All ability types must implement this interface.
    /// </summary>
    public interface IAbility
    {
        /// <summary>Unique ability identifier</summary>
        int Id { get; set; }

        /// <summary>Ability display name</summary>
        string Name { get; set; }

        /// <summary>Players who have learned this ability</summary>
        ICollection<Player> Players { get; set; }

        /// <summary>
        /// Execute the ability's effect on a target.
        /// </summary>
        /// <param name="user">The player using the ability</param>
        /// <param name="target">The target receiving the effect</param>
        /// <returns>Combat log message describing the result</returns>
        string Activate(IPlayer user, ITargetable target);
    }
}