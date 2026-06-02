using Xunit;

public class AdminAddRoomTests
{
    [Fact]
    public void Kamer_toevoegen_en_terugvinden()
    {
        var access = new RoomAccess("Data Source=:memory:");

        access.AddRoom(new RoomModel { Name = "Kamer 101", Type = "Onderzoek", Location = "Verdieping 1" });

        var rooms = access.GetAllRooms();
        Assert.Single(rooms);
        Assert.Equal("Kamer 101", rooms[0].Name);
    }
}
