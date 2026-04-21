using Microsoft.Data.Sqlite;
using Dapper;

public class DoctorAccess
{
    private SqliteConnection _connection = new SqliteConnection("Data Source=DataSources/project.db;Foreign Keys=False");

    public ReservationModel? GetNextActiveReservationByDoctorId(long doctorId)
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
            WHERE r.Specialist_ID = @DoctorId
              AND r.Status = 'gepland'
              AND datetime(r.Date) >= datetime('now')
            ORDER BY datetime(r.Date) ASC
            LIMIT 1";

        return _connection.QueryFirstOrDefault<ReservationModel>(sql, new { DoctorId = doctorId });
    }

    public List<ReservationModel> GetAllReservationsByDoctorId(long doctorId)
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
            WHERE r.Specialist_ID = @DoctorId
            ORDER BY datetime(r.Date) ASC";

        return _connection.Query<ReservationModel>(sql, new { DoctorId = doctorId }).ToList();
    }
    public List<ReservationModel> GetAllReservationsByDoctorIdByDate(long doctorId, DateTime date)
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
            WHERE r.Specialist_ID = @DoctorId
            AND strftime('%Y-%m-%d', r.Date) = @Date
            ORDER BY datetime(r.Date) ASC";

        return _connection.Query<ReservationModel>(sql, new 
        { 
            DoctorId = doctorId,
            Date = date.ToString("yyyy-MM-dd")
        }).ToList();
    }
}
