using Xunit;

public class ParentNextAppointmentTests
{
    [Fact]
    public void Eerstvolgende_afspraak_wordt_gepakt()
    {
        string conn = "Data Source=file:nextapp?mode=memory&cache=shared";
        var users = new UserAccess(conn);
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);

        users.Write(new UserModel
        {
            Email = "n@test.nl", Password = "x", FullName = "Patient",
            BirthDate = "1980-01-01", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        long uid = users.GetByEmail("n@test.nl").Id;
        rooms.AddRoom(new RoomModel { Name = "K1", Type = "x", Location = "1" });
        long rid = rooms.GetAllRooms()[0].Id;

        // 2 toekomstige, de eerstvolgende moet terugkomen
        reservations.CreateReservation(uid, rid, null, "2099-12-01 10:00", "Consult");
        reservations.CreateReservation(uid, rid, null, "2099-06-15 09:00", "Controle");

        var next = reservations.GetNextActiveReservationByUserId(uid);

        Assert.NotNull(next);
        Assert.Equal("2099-06-15", next!.Date);
    }
}
