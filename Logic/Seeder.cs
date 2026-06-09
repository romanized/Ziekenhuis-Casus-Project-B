// vult de database met standaard hulpverleners als die er nog niet zijn,
// zodat de planner altijd iemand kan kiezen per afspraak
static class Seeder
{
    public static void SeedCaregivers()
    {
        UserAccess access = new UserAccess();
        UserLogic logic = new UserLogic(access);

        // al hulpverleners aanwezig? dan niets doen
        if (access.GetAllByRole("specialty").Count > 0)
        {
            return;
        }

        logic.CreateEmployee("anne.bakker@ziekenhuis.nl", "zorg123", "Anne Bakker", "specialty", "Gynaecologie");
        logic.CreateEmployee("mark.devries@ziekenhuis.nl", "zorg123", "Mark de Vries", "specialty", "Verloskunde");
        logic.CreateEmployee("sophie.jansen@ziekenhuis.nl", "zorg123", "Sophie Jansen", "specialty", "Echoscopie");
    }
}
