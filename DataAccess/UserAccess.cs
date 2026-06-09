using Microsoft.Data.Sqlite;

using Dapper;


public class UserAccess
{
    private SqliteConnection _connection;

    private string Table = "User";

    public UserAccess() : this("Data Source=DataSources/project.db;Foreign Keys=False") { }

    // overload voor tests: geef ":memory:" mee en de tabel wordt direct aangemaakt
    public UserAccess(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS User (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Email TEXT,
                Password TEXT,
                Fullname TEXT,
                BirthDate TEXT,
                Phone_Number TEXT,
                StartDate TEXT,
                Role TEXT,
                Specialty TEXT,
                Notes TEXT
            )");
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Reservation (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                User_ID INTEGER,
                Specialist_ID INTEGER,
                Room_ID INTEGER,
                Date TEXT,
                Type TEXT,
                Status TEXT
            )");
    }

    public void Write(UserModel account)
    {
        string sql = $"INSERT INTO {Table} (Email, Password, Fullname, BirthDate, Phone_Number, StartDate, Role, Specialty, Notes) VALUES (@Email, @Password, @FullName, @BirthDate, @PhoneNumber, @StartDate, @Role, @Specialty, @Notes)";
        _connection.Execute(sql, account);
    }

    public UserModel GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE email = @Email";
        return _connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public void Update(UserModel account)
    {
        string sql = $"UPDATE {Table} SET email = @Email, password = @Password, fullname = @FullName WHERE id = @Id";
        _connection.Execute(sql, account);
    }

    public void Delete(UserModel account)
    {
        string sql = $"DELETE FROM {Table} WHERE id = @Id";
        _connection.Execute(sql, new { Id = account.Id });
    }

    public List<UserModel> GetAllByRole(string role)
    {
        string sql = $"SELECT * FROM {Table} WHERE Role = @Role ORDER BY Fullname";
        return _connection.Query<UserModel>(sql, new { Role = role }).ToList();
    }

    public List<UserModel> GetAvailableDoctors(string dateTime)
    {
        string sql = $@"
            SELECT * FROM {Table}
            WHERE Role = 'specialty'
            AND ID NOT IN (
                SELECT Specialist_ID FROM Reservation
                WHERE Status = 'gepland'
                AND Specialist_ID IS NOT NULL
                AND datetime(Date) < datetime(@DateTime, '+30 minutes')
                AND datetime(Date, '+30 minutes') > datetime(@DateTime)
            )
            ORDER BY Fullname";

        return _connection.Query<UserModel>(sql, new { DateTime = dateTime }).ToList();
    }
    public string GetFullNameById(long id)
    {
        string sql = $"SELECT Fullname FROM {Table} WHERE ID = @Id";

        return _connection.QueryFirstOrDefault<string>(sql, new { Id = id });
    }
}