public class RoomLogic
{
    private RoomAccess _access = new();

    public bool AddRoom(string roomNumber, string type)
    {
        RoomModel? existing = _access.GetByRoomNumber(roomNumber);
        if (existing != null)
        {
            return false; // Room already exists
        }

        RoomModel newRoom = new RoomModel
        {
            RoomNumber = roomNumber,
            Type = type
        };

        _access.Add(newRoom);
        return true;
    }

    public List<RoomModel> GetAllRooms()
    {
        return _access.GetAll();
    }
}
