// vult een lege database met de standaardgegevens die nodig zijn om de app
// te gebruiken (admin, planner, hulpverleners en kamers). Wordt bij opstart
// aangeroepen. project.db zit niet meer in git, dus een verse clone bouwt
// de database hiermee zelf op. Alles is idempotent: bestaat het al, dan
// gebeurt er niets.
static class Seeder
{
    public static void SeedAll()
    {
        UserAccess users = new UserAccess();
        UserLogic logic = new UserLogic(users);
        RoomAccess rooms = new RoomAccess();

        if (users.GetAllByRole("admin").Count == 0)
        {
            logic.CreateEmployee("admin@hospital.com", "admin123", "admin account", "admin", "");
        }

        if (users.GetAllByRole("planner").Count == 0)
        {
            logic.CreateEmployee("planner", "planner", "planner", "planner", "");
        }

        if (users.GetAllByRole("specialty").Count == 0)
        {
            logic.CreateEmployee("anne.bakker@ziekenhuis.nl", "zorg123", "Anne Bakker", "specialty", "Gynaecologie");
            logic.CreateEmployee("mark.devries@ziekenhuis.nl", "zorg123", "Mark de Vries", "specialty", "Verloskunde");
            logic.CreateEmployee("sophie.jansen@ziekenhuis.nl", "zorg123", "Sophie Jansen", "specialty", "Echoscopie");
        }

        if (rooms.GetAllRooms().Count == 0)
        {
            rooms.AddRoom(new RoomModel { Name = "Echokamer 1", Type = "Echo", Location = "Verdieping 1" });
            rooms.AddRoom(new RoomModel { Name = "Onderzoekskamer 1", Type = "Onderzoek", Location = "Verdieping 1" });
            rooms.AddRoom(new RoomModel { Name = "Operatiekamer 1", Type = "Operatie", Location = "Verdieping 2" });
        }
    }
}
