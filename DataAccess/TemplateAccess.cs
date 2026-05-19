using Microsoft.Data.Sqlite;
using Dapper;

public class TemplateAccess
{
    private SqliteConnection _connection = new SqliteConnection("Data Source=DataSources/project.db;Foreign Keys=False");

    public TemplateAccess()
    {
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
