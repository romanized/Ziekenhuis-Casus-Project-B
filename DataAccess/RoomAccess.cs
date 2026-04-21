using Microsoft.Data.Sqlite;
using Dapper;

public class RoomAccess
{
    private SqliteConnection _connection = new SqliteConnection("Data Source=DataSources/project.db;Foreign Keys=False");

    public void AddRoom(RoomModel room)
    {
        string sql = "INSERT INTO Room (Name, Type, Location) VALUES (@Name, @Type, @Location)";
        _connection.Execute(sql, room);
    }

    public List<RoomModel> GetAllRooms()
    {
        string sql = "SELECT ID as Id, Name, Type, Location FROM Room ORDER BY Name";
        return _connection.Query<RoomModel>(sql).ToList();
    }

    public List<RoomModel> GetAvailableRooms(string dateTime)
    {
        string sql = @"
            SELECT ID as Id, Name, Type, Location FROM Room
            WHERE ID NOT IN (
                SELECT Room_ID FROM Reservation
                WHERE datetime(Date) = datetime(@DateTime)
                AND Status = 'gepland'
            )
            ORDER BY Name";
        return _connection.Query<RoomModel>(sql, new { DateTime = dateTime }).ToList();
    }
    public string GetRoomNameById(long id)
    {
        string sql = "SELECT Name FROM Room WHERE ID = @Id";

        return _connection.QueryFirstOrDefault<string>(sql, new { Id = id });
    }
}
