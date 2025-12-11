namespace ConsoleRpgEntities.Models.Attributes
{
    /// <summary>
    /// Interface for entities that can be targeted by attacks and abilities.
    /// Implemented by both Player and Monster classes.
    /// </summary>
    public interface ITargetable
    {
        /// <summary>Name displayed in combat messages</summary>
        string Name { get; set; }

        /// <summary>Current health points</summary>
        int Health { get; set; }

        /// <summary>
        /// Process incoming damage, applying any defense modifiers.
        /// </summary>
        /// <param name="damage">Raw damage before defense calculation</param>
        /// <returns>Actual damage dealt after defense reduction</returns>
        int ReceiveAttack(int damage);
    }
}