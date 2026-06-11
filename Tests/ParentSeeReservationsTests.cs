using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ParentSeeReservationsTests
{
    [TestMethod]
    public void Ouder_ziet_alleen_eigen_afspraken()
    {
        string conn = "Data Source=file:parentres?mode=memory&cache=shared";
        var users = new UserAccess(conn);
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);

        users.Write(new UserModel
        {
            Email = "z@test.nl", Password = "x", FullName = "Ziet Reservering",
            BirthDate = "1980-01-01", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        long uid = users.GetByEmail("z@test.nl").Id;
        rooms.AddRoom(new RoomModel { Name = "K1", Type = "x", Location = "1" });
        long rid = rooms.GetAllRooms()[0].Id;

        reservations.CreateReservation(uid, rid, null, "2026-06-15 10:00", "Controle");
        reservations.CreateReservation(uid, rid, null, "2026-07-20 14:00", "Consult");
        // van iemand anders, mag er niet bij
        reservations.CreateReservation(uid + 99, rid, null, "2026-06-16 10:00", "Operatie");

        var mine = reservations.GetAllReservationsByUserId(uid);

        Assert.AreEqual(2, mine.Count);
    }
}
