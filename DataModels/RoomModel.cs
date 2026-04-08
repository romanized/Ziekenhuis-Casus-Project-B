public class RoomModel
{
    public Int64 Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Location { get; set; }

    public RoomModel() { }

    public RoomModel(Int64 id, string name, string type, string location)
    {
        Id = id;
        Name = name;
        Type = type;
        Location = location;
    }
}
