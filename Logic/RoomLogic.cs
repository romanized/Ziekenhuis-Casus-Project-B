public class RoomLogic
{
    private RoomAccess _access = new();

    public bool AddRoom(string name, string type, string location)
    {
        RoomModel? existing = _access.GetByName(name);
        if (existing != null)
        {
            return false; // Room already exists
        }

        RoomModel newRoom = new RoomModel
        {
            Name = name,
            Type = type,
            Location = location
        };

        _access.Add(newRoom);
        return true;
    }

    public List<RoomModel> GetAllRooms()
    {
        return _access.GetAll();
    }
}
