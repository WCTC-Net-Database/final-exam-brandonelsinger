namespace ConsoleRpgEntities.Models.Attributes;

public interface ITargetable
{
    string Name { get; set; }
    int Health { get; set; }
    int ReceiveAttack(int damage);
}
