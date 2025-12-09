using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public abstract class Monster : IMonster, ITargetable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int AggressionLevel { get; set; }
        public string MonsterType { get; set; }
        public int ArmorClass { get; set; }

        // Foreign key
        public int? RoomId { get; set; }

        // Navigation property
        public virtual Room Room { get; set; }

        protected Monster()
        {

        }
        public int ReceiveAttack(int damage)
        {
            int actualDamage = Math.Max(0, damage - ArmorClass);
            Health -= actualDamage;
            return actualDamage;
        }

        public abstract string Attack(ITargetable target);

        public int? LootItemId { get; set; }
        public virtual Item LootItem { get; set; }

    }
}
