public class RoomModel
{
    public Int64 Id { get; set; }
    public string RoomNumber { get; set; }
    public string Type { get; set; }

    public RoomModel() { }

    public RoomModel(Int64 id, string roomNumber, string type)
    {
        Id = id;
        RoomNumber = roomNumber;
        Type = type;
    }
}
