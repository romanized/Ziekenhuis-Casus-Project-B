using Microsoft.Data.Sqlite;
using Dapper;

public class ReservationAccess
{
    private SqliteConnection _connection = new SqliteConnection("Data Source=DataSources/project.db");

    public ReservationModel? GetNextActiveReservationByUserId(long userId)
    {
        string sql = @"
            SELECT
                r.ID as Id,
                r.User_ID as UserId,
                r.Doctor_ID as DoctorId,
                r.Room_ID as RoomId,
                r.Date as Date,
                r.Time as Time,
                r.Status as Status,
                room.Room_number as RoomNumber
            FROM Reservation r
            INNER JOIN Room room ON r.Room_ID = room.ID
            WHERE r.User_ID = @UserId
              AND r.Status = 'active'
              AND datetime(r.Date || ' ' || r.Time) >= datetime('now')
            ORDER BY datetime(r.Date || ' ' || r.Time) ASC
            LIMIT 1";

        return _connection.QueryFirstOrDefault<ReservationModel>(sql, new { UserId = userId });
    }

    public List<ReservationModel> GetAllReservationsByUserId(long userId)
    {
        string sql = @"
            SELECT
                r.ID as Id,
                r.User_ID as UserId,
                r.Doctor_ID as DoctorId,
                r.Room_ID as RoomId,
                r.Date as Date,
                r.Time as Time,
                r.Status as Status,
                room.Room_number as RoomNumber
            FROM Reservation r
            INNER JOIN Room room ON r.Room_ID = room.ID
            WHERE r.User_ID = @UserId
            ORDER BY datetime(r.Date || ' ' || r.Time) ASC";

        return _connection.Query<ReservationModel>(sql, new { UserId = userId }).ToList();
    }
}