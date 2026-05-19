using Microsoft.Data.Sqlite;
using Dapper;

public class TemplateAccess
{
    private SqliteConnection _connection;

    public TemplateAccess() : this("Data Source=DataSources/project.db;Foreign Keys=False") { }

    // this overload is useful for tests — just pass ":memory:"
    public TemplateAccess(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        // keep connection open so in-memory SQLite doesn't lose data between queries
        _connection.Open();
        // table gets created if it doesn't exist yet, no manual setup needed
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS AppointmentTemplate (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL,
                Notes TEXT NOT NULL DEFAULT ''
            )");
    }

    public void AddTemplate(TemplateModel template)
    {
        string sql = "INSERT INTO AppointmentTemplate (Name, Type, Notes) VALUES (@Name, @Type, @Notes)";
        _connection.Execute(sql, template);
    }

    // sorted by name so the planner can find them easily
    public List<TemplateModel> GetAll()
    {
        string sql = "SELECT ID as Id, Name, Type, Notes FROM AppointmentTemplate ORDER BY Name";
        return _connection.Query<TemplateModel>(sql).ToList();
    }

    public void Delete(long id)
    {
        _connection.Execute("DELETE FROM AppointmentTemplate WHERE ID = @Id", new { Id = id });
    }
}
