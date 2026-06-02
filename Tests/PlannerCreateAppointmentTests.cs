using Xunit;

public class PlannerCreateAppointmentTests
{
    [Fact]
    public void Afspraak_aanmaken_en_terugzien_op_datum()
    {
        // shared zodat alle 3 de access klassen dezelfde db zien
        string conn = "Data Source=file:plantest?mode=memory&cache=shared";
        var users = new UserAccess(conn);
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);

        users.Write(new UserModel
        {
            Email = "p@test.nl", Password = "x", FullName = "Patient Een",
            BirthDate = "1980-01-01", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        var patient = users.GetByEmail("p@test.nl");
        rooms.AddRoom(new RoomModel { Name = "Kamer A", Type = "Onderzoek", Location = "1" });
        long roomId = rooms.GetAllRooms()[0].Id;

        reservations.CreateReservation(patient.Id, roomId, null, "2026-06-15 10:00", "Controle");

        var result = reservations.GetReservationsForDate("2026-06-15");
        Assert.Single(result);
        Assert.Equal("gepland", result[0].Status);
        Assert.Equal("Controle", result[0].Type);
    }
}
