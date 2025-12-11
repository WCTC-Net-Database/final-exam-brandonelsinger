using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;

namespace ConsoleRpgEntities.Models.Characters;

/// <summary>
/// Interface defining core player behavior.
/// Used for type constraints in ability activation methods.
/// </summary>
public interface IPlayer
{
    // Unique identifier for the player
    int Id { get; set; }

    // Name of the player
    string Name { get; set; }

    // Collection of learned abilities
    ICollection<Ability> Abilities { get; set; }

}
