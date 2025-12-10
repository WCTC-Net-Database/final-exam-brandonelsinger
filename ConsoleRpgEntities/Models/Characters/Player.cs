using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using ConsoleRpgEntities.Models.Attributes;
using System.ComponentModel.DataAnnotations;
using ConsoleRpgEntities.Models.Equipments;
using ConsoleRpgEntities.Models.Rooms;

namespace ConsoleRpgEntities.Models.Characters
{
    public class Player : ITargetable, IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int Level { get; set; } = 1;
        public int MaxHealth { get; set; } = 100;

        // Foreign keys
        public int? EquipmentId { get; set; }
        public int? RoomId { get; set; }
        public virtual Inventory Inventory { get; set; }

        // Navigation properties
        public virtual Equipment Equipment { get; set; }
        public virtual ICollection<Ability> Abilities { get; set; }
        public virtual Room Room { get; set; }

        public Player()
        {
            Abilities = new List<Ability>();
        }
        public int ReceiveAttack(int damage)
        {
            int defense = (Equipment?.Armor?.Defense ?? 0) + (Equipment?.Weapon?.Defense ?? 0);

            int actualDamage = Math.Max(0, damage - defense);

            Health -= actualDamage;

            return actualDamage;
        }

        public string GainExperience(int amount)
        {
            Experience += amount;

            int calculatedLevel = (Experience / 100) + 1;

            if (calculatedLevel > Level)
            {
                Level = calculatedLevel;

                
                MaxHealth += 10; 
                Health = MaxHealth; 

                return $"[yellow bold]LEVEL UP![/] You are now Level {Level}! (Max HP increased to {MaxHealth})";
            }

            return $"Gained {amount} XP. ({Experience % 100}/100 to next level)";
        }
        public void Attack(ITargetable target)
        {
            Console.WriteLine($"{Name} attacks {target.Name} with a {Equipment.Weapon.Name} dealing {Equipment.Weapon.Attack} damage!");
            target.Health -= Equipment.Weapon.Attack;
            System.Console.WriteLine($"{target.Name} has {target.Health} health remaining.");

        }

        public string UseAbility(IAbility ability, ITargetable target)
        {
            if (Abilities.Contains(ability))
            {
                return ability.Activate(this, target);
            }
            else
            {
                return $"{Name} does not have the ability {ability.Name}!";
            }
        }
    }
}
