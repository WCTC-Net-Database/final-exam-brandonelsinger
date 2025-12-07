using ConsoleRpgEntities.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public class Beast : Monster
    {
        public override string Attack(ITargetable target)
        {
            int damage = AggressionLevel + 2;
            int actualDamage = target.ReceiveAttack(damage);
            return $"The {Name} lunges at {target.Name} with feral rage for {actualDamage} damage!";

        }
    }
}
