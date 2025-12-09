using ConsoleRpgEntities.Models.Attributes;
using ConsoleRpgEntities.Models.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRpgEntities.Models.Abilities.PlayerAbilities
{
    public class HealAbility : Ability
    {
        public override void Activate(IPlayer user, ITargetable target)
        {
            int healAmount = Math.Abs(Damage);
            Console.WriteLine($"[Magic] {user.Name} casts a holy light on {target.Name}.");

            if (target is Player p)
            {
                p.Health = Math.Min(p.MaxHealth, p.Health + healAmount);
            }
            else
            {
                target.Health += healAmount;
            }

            Console.WriteLine($"{target.Name} recovers {healAmount} HP!");
        }
    }
}
