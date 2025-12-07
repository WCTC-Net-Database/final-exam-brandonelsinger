using ConsoleRpgEntities.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRpgEntities.Models.Characters.Monsters
{
    public class Undead : Monster
    {
        public override string Attack(ITargetable target)
        {
            int damage = AggressionLevel;
            int actualDamage = target.ReceiveAttack(damage);
            return $"{Name} strikes {target.Name} with a bone-chilling touch for {actualDamage} necrotic damage!";
        }
    }
}
