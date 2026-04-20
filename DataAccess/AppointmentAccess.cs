using Microsoft.Data.Sqlite;
using Dapper;

public class ReservationAccess
{
    private SqliteConnection _connection = new SqliteConnection("Data Source=DataSources/project.db;Foreign Keys=False");

    private const string BaseSelect = @"
        SELECT
            r.ID as Id,
            r.User_ID as UserId,
            r.Specialist_ID as SpecialistId,
            r.Room_ID as RoomId,
            strftime('%Y-%m-%d', r.Date) as Date,
            strftime('%H:%M', r.Date) as Time,
            r.Status as Status,
            r.Type as Type,
            room.Name as RoomNumber,
            u.Fullname as PatientName,
            COALESCE(d.Fullname, '') as DoctorName
        FROM Reservation r
        INNER JOIN Room room ON r.Room_ID = room.ID
        INNER JOIN User u ON r.User_ID = u.ID
        LEFT JOIN User d ON r.Specialist_ID = d.ID";

    public ReservationModel? GetNextActiveReservationByUserId(long userId)
    {
        string sql = BaseSelect + @"
            WHERE r.User_ID = @UserId
              AND r.Status = 'gepland'
              AND datetime(r.Date) >= datetime('now')
            ORDER BY datetime(r.Date) ASC
            LIMIT 1";

        return _connection.QueryFirstOrDefault<ReservationModel>(sql, new { UserId = userId });
    }

    public List<ReservationModel> GetAllReservationsByUserId(long userId)
    {
        string sql = BaseSelect + @"
            WHERE r.User_ID = @UserId
            ORDER BY datetime(r.Date) ASC";

        return _connection.Query<ReservationModel>(sql, new { UserId = userId }).ToList();
    }

    public List<ReservationModel> GetAllReservations()
    {
        string sql = BaseSelect + @"
            ORDER BY datetime(r.Date) ASC";

        return _connection.Query<ReservationModel>(sql).ToList();
    }

    public List<ReservationModel> GetReservationsForDate(string date)
    {
        string sql = BaseSelect + @"
            WHERE strftime('%Y-%m-%d', r.Date) = @Date
            ORDER BY datetime(r.Date) ASC";

        return _connection.Query<ReservationModel>(sql, new { Date = date }).ToList();
    }

    public List<string> GetReservedDatesForMonth(string yearMonth)
    {
        string sql = @"
            SELECT DISTINCT strftime('%Y-%m-%d', Date) as Date
            FROM Reservation
            WHERE strftime('%Y-%m', Date) = @YearMonth
            ORDER BY Date";

        return _connection.Query<string>(sql, new { YearMonth = yearMonth }).ToList();
    }

    public void CreateReservation(long userId, long roomId, long? specialistId, string dateTime, string type)
    {
        string sql = @"INSERT INTO Reservation (User_ID, Room_ID, Specialist_ID, Date, Type, Status)
                       VALUES (@UserId, @RoomId, @SpecialistId, @Date, @Type, 'gepland')";
        _connection.Execute(sql, new { UserId = userId, RoomId = roomId, SpecialistId = specialistId, Date = dateTime, Type = type });
    }
}
