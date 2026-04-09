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
                r.Specialist_ID as SpecialistId,
                r.Room_ID as RoomId,
                strftime('%Y-%m-%d', r.Date) as Date,
                strftime('%H:%M', r.Date) as Time,
                r.Status as Status,
                room.Name as RoomNumber
            FROM Reservation r
            INNER JOIN Room room ON r.Room_ID = room.ID
            WHERE r.User_ID = @UserId
              AND r.Status = 'gepland'
              AND datetime(r.Date) >= datetime('now')
            ORDER BY datetime(r.Date) ASC
            LIMIT 1";

        return _connection.QueryFirstOrDefault<ReservationModel>(sql, new { UserId = userId });
    }

    public List<ReservationModel> GetAllReservationsByUserId(long userId)
    {
        string sql = @"
            SELECT
                r.ID as Id,
                r.User_ID as UserId,
                r.Specialist_ID as SpecialistId,
                r.Room_ID as RoomId,
                strftime('%Y-%m-%d', r.Date) as Date,
                strftime('%H:%M', r.Date) as Time,
                r.Status as Status,
                room.Name as RoomNumber
            FROM Reservation r
            INNER JOIN Room room ON r.Room_ID = room.ID
            WHERE r.User_ID = @UserId
            ORDER BY datetime(r.Date) ASC";

        return _connection.Query<ReservationModel>(sql, new { UserId = userId }).ToList();
    }
}