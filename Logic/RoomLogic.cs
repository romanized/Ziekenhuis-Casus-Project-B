public class RoomLogic
{
    private RoomAccess _access;

    public RoomLogic() { _access = new RoomAccess(); }

    public void AddRoom(RoomModel room)
    {
        _access.AddRoom(room);
    }

    public List<RoomModel> GetAllRooms()
    {
        return _access.GetAllRooms();
    }

    public List<RoomModel> GetAvailableRooms(string dateTime)
    {
        return _access.GetAvailableRooms(dateTime);
    }

    public string GetRoomNameById(long id)
    {
        return _access.GetRoomNameById(id);
    }

    public string GetRoomLocationById(long id)
    {
        return _access.GetRoomLocationById(id);
    }
}
