using Microsoft.Data.Sqlite;
using Dapper;

public class RoomAccess
{
    private SqliteConnection _connection = new SqliteConnection("Data Source=DataSources/project.db");

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
}
