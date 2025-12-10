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
        public override string Activate(IPlayer user, ITargetable target)
        {
            int actualDamage = target.ReceiveAttack(Damage);
            return $"{user.Name} casts Fireball! {target.Name} takes {actualDamage} fire damage and is singed!";
        }
    }
}
