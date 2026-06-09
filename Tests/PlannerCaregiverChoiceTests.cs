using Xunit;

// de planner moet per afspraak een hulpverlener kunnen kiezen
public class PlannerCaregiverChoiceTests
{
    [Fact]
    public void Beschikbare_hulpverlener_verschijnt_in_keuzelijst()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        logic.CreateEmployee("arts@ziekenhuis.nl", "zorg123", "Dr Anne", "specialty", "Verloskunde");

        var available = access.GetAvailableDoctors("2026-12-01 09:00");

        Assert.Single(available);
        Assert.Equal("Dr Anne", available[0].FullName);
    }

    [Fact]
    public void Bezette_hulpverlener_valt_weg_op_dat_tijdstip()
    {
        // shared zodat user-, room- en reservation-access dezelfde db zien
        string conn = "Data Source=file:caretest?mode=memory&cache=shared";
        var users = new UserAccess(conn);
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);
        var logic = new UserLogic(users);

        logic.CreateEmployee("arts@ziekenhuis.nl", "zorg123", "Dr Anne", "specialty", "Verloskunde");
        var doctor = users.GetByEmail("arts@ziekenhuis.nl");

        users.Write(new UserModel
        {
            Email = "p@test.nl", Password = "x", FullName = "Patient Een",
            BirthDate = "", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        var patient = users.GetByEmail("p@test.nl");
        rooms.AddRoom(new RoomModel { Name = "Kamer A", Type = "Onderzoek", Location = "1" });
        long roomId = rooms.GetAllRooms()[0].Id;

        reservations.CreateReservation(patient.Id, roomId, doctor.Id, "2026-12-01 09:00", "Controle");

        var bezet = users.GetAvailableDoctors("2026-12-01 09:00");
        var later = users.GetAvailableDoctors("2026-12-01 11:00");

        Assert.Empty(bezet);
        Assert.Single(later);
    }
}
