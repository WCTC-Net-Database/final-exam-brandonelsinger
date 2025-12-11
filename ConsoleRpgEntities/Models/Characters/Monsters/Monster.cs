using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    /// <summary>
    /// Abstract base class for all monster types in the game.
    /// Uses Table-Per-Hierarchy (TPH) inheritance with MonsterType as the discriminator.
    /// Implements IMonster for monster behavior and ITargetable to receive attacks.
    /// </summary>
    public abstract class Monster : IMonster, ITargetable
    {
        // ===== Primary Key =====
        public int Id { get; set; }

        // ===== Monster Attributes =====
        /// <summary>Display name of the monster</summary>
        public string Name { get; set; }

        /// <summary>Current health points - monster dies when this reaches 0</summary>
        public int Health { get; set; }

        /// <summary>Determines attack damage and AI behavior intensity</summary>
        public int AggressionLevel { get; set; }

        /// <summary>TPH discriminator - identifies the concrete monster type (Goblin, Beast, etc.)</summary>
        public string MonsterType { get; set; }

        /// <summary>Damage reduction applied to incoming attacks</summary>
        public int ArmorClass { get; set; }

        // ===== Foreign Keys =====
        /// <summary>The room where this monster is located (nullable)</summary>
        public int? RoomId { get; set; }

        /// <summary>Item dropped when monster is defeated (nullable)</summary>
        public int? LootItemId { get; set; }

        // ===== Navigation Properties =====
        /// <summary>The room containing this monster</summary>
        public virtual Room Room { get; set; }

        /// <summary>The item this monster drops on death</summary>
        public virtual Item LootItem { get; set; }

        /// <summary>
        /// Protected constructor for inheritance
        /// </summary>
        protected Monster()
        {
        }

        /// <summary>
        /// Calculates and applies damage to the monster, factoring in armor class.
        /// Part of the ITargetable interface implementation.
        /// </summary>
        /// <param name="damage">Raw incoming damage before armor reduction</param>
        /// <returns>Actual damage dealt after armor reduction</returns>
        public int ReceiveAttack(int damage)
        {
            // Apply armor class as flat damage reduction
            int actualDamage = Math.Max(0, damage - ArmorClass);
            Health -= actualDamage;
            return actualDamage;
        }

        /// <summary>
        /// Abstract attack method - each monster type implements unique attack behavior.
        /// </summary>
        /// <param name="target">The entity being attacked</param>
        /// <returns>Combat log message describing the attack</returns>
        public abstract string Attack(ITargetable target);
    }
}