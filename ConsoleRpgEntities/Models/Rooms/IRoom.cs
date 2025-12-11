namespace ConsoleRpgEntities.Models.Rooms
{
    /// <summary>
    /// Interface defining core room properties.
    /// Used for type constraints and polymorphic room handling.
    /// </summary>
    public interface IRoom
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        int? NorthRoomId { get; set; }
        int? SouthRoomId { get; set; }
        int? EastRoomId { get; set; }
        int? WestRoomId { get; set; }
        int X { get; set; }
        int Y { get; set; }
    }
}
