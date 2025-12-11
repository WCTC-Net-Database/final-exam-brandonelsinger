using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleRpgEntities.Models.Equipments;

/// <summary>
/// Represents a weapon, armor, or other item in the game.
/// Items can be equipped (via Equipment) or stored in inventory.
/// The Type property determines if it's a "Weapon" or "Armor".
/// </summary>
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Weight { get; set; }

    public int Value { get; set; }
}
