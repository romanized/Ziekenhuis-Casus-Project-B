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
}
