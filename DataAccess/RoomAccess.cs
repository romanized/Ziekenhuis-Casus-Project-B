using Microsoft.Data.Sqlite;
using Dapper;

public class RoomAccess
{
    private SqliteConnection _connection = new SqliteConnection($"Data Source=DataSources/project.db");

    private string Table = "Room";

    public void Add(RoomModel room)
    {
        string sql = $"INSERT INTO {Table} (Name, Type, Location) VALUES (@Name, @Type, @Location)";
        _connection.Execute(sql, room);
    }

    public List<RoomModel> GetAll()
    {
        string sql = $"SELECT * FROM {Table}";
        return _connection.Query<RoomModel>(sql).ToList();
    }

    public RoomModel? GetByName(string name)
    {
        string sql = $"SELECT * FROM {Table} WHERE Name = @Name";
        return _connection.QueryFirstOrDefault<RoomModel>(sql, new { Name = name });
    }
}
