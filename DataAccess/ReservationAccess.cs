using Microsoft.Data.Sqlite;

using Dapper;


public class ReservationAcesss
{
    private SqliteConnection _connection = new SqliteConnection($"Data Source=DataSources/project.db");

    private string Table = "Reservation";

    public void Write(ReservationModel reservation)
    {
        Console.WriteLine($"Executing SQL on table: {Table}");
Console.WriteLine($"Database path: {_connection.DataSource}");
        string sql = $"INSERT INTO {Table} (User_ID,Room_ID,Specialist_ID,InfoFolder_ID,Date,Type,Doel,Status,Sleutel_1,Sleutel_2) VALUES (@User_Id, @Room_Id, @Specialist_Id ,@Infofolder_Id,@Date,@Type,@Doel,@Status,@Sleutel1,@Sleutel2)";
        _connection.Execute(sql, reservation);
    }

}