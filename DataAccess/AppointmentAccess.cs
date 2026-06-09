using Microsoft.Data.Sqlite;
using Dapper;

public class ReservationAccess
{
    private SqliteConnection _connection;

    public ReservationAccess() : this("Data Source=DataSources/project.db;Foreign Keys=False") { }

    // overload voor tests met in-memory db
    public ReservationAccess(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Reservation (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                User_ID INTEGER,
                Specialist_ID INTEGER,
                Room_ID INTEGER,
                Date TEXT,
                Type TEXT,
                Status TEXT,
                Template_ID INTEGER
            )");
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Room (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Type TEXT,
                Location TEXT
            )");
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
        // join-doel voor de gekozen template; zorg dat de tabel bestaat
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS AppointmentTemplate (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL,
                Notes TEXT NOT NULL DEFAULT ''
            )");

        // migratie voor bestaande databases: voeg Template_ID toe als die nog ontbreekt
        bool hasTemplateId = _connection
            .Query("PRAGMA table_info(Reservation)")
            .Any(c => string.Equals((string)c.name, "Template_ID", StringComparison.OrdinalIgnoreCase));
        if (!hasTemplateId)
        {
            _connection.Execute("ALTER TABLE Reservation ADD COLUMN Template_ID INTEGER");
        }
    }

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
            COALESCE(d.Fullname, '') as DoctorName,
            r.Template_ID as TemplateId,
            COALESCE(t.Name, '') as TemplateName,
            COALESCE(t.Notes, '') as TemplateNotes
        FROM Reservation r
        INNER JOIN Room room ON r.Room_ID = room.ID
        INNER JOIN User u ON r.User_ID = u.ID
        LEFT JOIN User d ON r.Specialist_ID = d.ID
        LEFT JOIN AppointmentTemplate t ON r.Template_ID = t.ID";

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

    public void CreateReservation(long userId, long roomId, long? specialistId, string dateTime, string type, long? templateId = null)
    {
        string sql = @"INSERT INTO Reservation (User_ID, Room_ID, Specialist_ID, Date, Type, Status, Template_ID)
                       VALUES (@UserId, @RoomId, @SpecialistId, @Date, @Type, 'gepland', @TemplateId)";
        _connection.Execute(sql, new { UserId = userId, RoomId = roomId, SpecialistId = specialistId, Date = dateTime, Type = type, TemplateId = templateId });
    }
}
