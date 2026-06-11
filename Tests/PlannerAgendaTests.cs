using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PlannerAgendaTests
{
    [TestMethod]
    public void Maand_geeft_distinct_dates_terug()
    {
        string conn = "Data Source=file:planagenda?mode=memory&cache=shared";
        var users = new UserAccess(conn);
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);

        users.Write(new UserModel
        {
            Email = "a@test.nl", Password = "x", FullName = "A",
            BirthDate = "", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        long uid = users.GetByEmail("a@test.nl").Id;
        rooms.AddRoom(new RoomModel { Name = "K1", Type = "x", Location = "1" });
        long rid = rooms.GetAllRooms()[0].Id;

        // 2 op dezelfde dag - moet er als 1 terugkomen
        reservations.CreateReservation(uid, rid, null, "2026-06-15 10:00", "Controle");
        reservations.CreateReservation(uid, rid, null, "2026-06-15 14:00", "Consult");
        reservations.CreateReservation(uid, rid, null, "2026-06-20 11:00", "Consult");
        // andere maand
        reservations.CreateReservation(uid, rid, null, "2026-07-01 11:00", "Consult");

        var dates = reservations.GetReservedDatesForMonth("2026-06");

        Assert.AreEqual(2, dates.Count);
        Assert.IsTrue(dates.Contains("2026-06-15"));
    }
}
