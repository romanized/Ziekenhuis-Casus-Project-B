using Xunit;

public class PlannerRoomStatusTests
{
    [Fact]
    public void Bezette_kamer_komt_niet_in_lijst()
    {
        string conn = "Data Source=file:roomstatus?mode=memory&cache=shared";
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);

        rooms.AddRoom(new RoomModel { Name = "Bezet", Type = "x", Location = "1" });
        rooms.AddRoom(new RoomModel { Name = "Vrij", Type = "x", Location = "1" });
        long bezetId = rooms.GetAllRooms().First(r => r.Name == "Bezet").Id;
        reservations.CreateReservation(1, bezetId, null, "2026-06-15 10:00", "Controle");

        var available = rooms.GetAvailableRooms("2026-06-15 10:00");

        Assert.Single(available);
        Assert.Equal("Vrij", available[0].Name);
    }
}
