using ConsoleRpgEntities.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public class Bandit : Monster
    {
        public override string Attack(ITargetable target)
        {
            int damage = AggressionLevel + 3;
            int actualDamage = target.ReceiveAttack(damage);
            return $"{Name} attacks {target.Name} with a rusty weapon dealing {actualDamage} damage!";
        }
    }
}
