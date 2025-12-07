using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    public class CombatAbility : Ability
    {
        public override void Activate(IPlayer user, ITargetable target)
        {
            int weaponDamage = 0;
            string weaponName = "Fists";

            if (user is Player player && player.Equipment?.Weapon != null)
            {
                weaponDamage = player.Equipment.Weapon.Attack;
                weaponName = player.Equipment.Weapon.Name;
            }

            int totalDamage = weaponDamage + Damage;

            Console.WriteLine($"[Skill] {user.Name} uses {Name} with their {weaponName}!");

            int actualDamage = target.ReceiveAttack(totalDamage);

            Console.WriteLine($"A brutal strike dealing {actualDamage} physical damage!");

        }
    }
}
