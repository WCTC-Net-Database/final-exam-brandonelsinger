using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    public class FireballAbility : Ability
    {
        public override void Activate(IPlayer user, ITargetable target)
        {
            Console.WriteLine($"A massive fireball explodes on {target.Name}!");

            int actualDamage = target.ReceiveAttack(Damage);

            Console.WriteLine($"{target.Name} takes {actualDamage} fire damage and is singed!");
        }
    }
}
