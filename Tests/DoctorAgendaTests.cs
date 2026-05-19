using Xunit;

public class DoctorAgendaTests
{
    [Fact]
    public void Dokter_ziet_alleen_eigen_afspraken_op_die_dag()
    {
        string conn = "Data Source=file:docagenda?mode=memory&cache=shared";
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);
        var docs = new DoctorAccess(conn);

        rooms.AddRoom(new RoomModel { Name = "K1", Type = "x", Location = "1" });
        long rid = rooms.GetAllRooms()[0].Id;

        long doctorId = 5;
        long andereDoctor = 6;

        reservations.CreateReservation(1, rid, doctorId, "2026-06-15 10:00", "Controle");
        reservations.CreateReservation(2, rid, doctorId, "2026-06-15 11:00", "Consult");
        // andere dag
        reservations.CreateReservation(3, rid, doctorId, "2026-06-16 09:00", "Controle");
        // andere dokter
        reservations.CreateReservation(4, rid, andereDoctor, "2026-06-15 12:00", "Controle");

        var result = docs.GetAllReservationsByDoctorIdByDate(doctorId, new DateTime(2026, 6, 15));

        Assert.Equal(2, result.Count);
    }
}
