using Microsoft.Data.Sqlite;

using Dapper;


public class UserAccess
{
    private SqliteConnection _connection = new SqliteConnection($"Data Source=DataSources/project.db");

    private string Table = "User";

    public void Write(UserModel account)
    {
        string sql = $"INSERT INTO {Table} (Email, Password, Fullname, Phone_Number, StartDate, Role, Specialty) VALUES (@Email, @Password, @FullName, @PhoneNumber, @StartDate, @Role, @Specialty)";
        _connection.Execute(sql, account);
    }

    public UserModel GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE email = @Email";
        return _connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public void Update(UserModel account)
    {
        string sql = $"UPDATE {Table} SET email = @EmailAddress, password = @Password, fullname = @FullName WHERE id = @Id";
        _connection.Execute(sql, account);
    }

    public void Delete(UserModel account)
    {
        string sql = $"DELETE FROM {Table} WHERE id = @Id";
        _connection.Execute(sql, new { Id = account.Id });
    }



}