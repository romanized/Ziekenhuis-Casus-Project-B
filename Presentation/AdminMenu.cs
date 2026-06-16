static class AdminMenu
{
    private static UserLogic userLogic = new UserLogic();
    private static RoomAccess roomAccess = new RoomAccess();
    private static TemplateAccess templateAccess = new TemplateAccess();

    public static void Start(UserModel admin)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
        ______ _      _              _           _
        |___  /(_)    | |            | |         (_)
            / /  _  ___| | _____ _ __ | |__  _   _ _ ___
        / /  | |/ _ \ |/ / _ \ '_ \| '_ \| | | | / __|
        / /__ | |  __/   <  __/ | | | | | | |_| | \__ \
        /_____||_|\___|_|\_\___|_| |_|_| |_|\__,_|_|___/

        ");
            Console.ResetColor();
            Console.WriteLine("\n==== Beheerdersmenu ====");
            Console.WriteLine("1. Nieuwe zorgverlener toevoegen");
            Console.WriteLine("2. Nieuwe planner toevoegen");
            Console.WriteLine("3. Nieuwe kamer toevoegen");
            Console.WriteLine("4. Afsprakentemplates beheren");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. Uitloggen");
            Console.ResetColor();

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Voer een geldige optie in.");
                continue;
            }

            switch (input.Trim())
            {
                case "1":
                    Console.Clear();
                    CreateEmployee("specialty");
                    break;
                case "2":
                    Console.Clear();
                    CreateEmployee("planner");
                    break;
                case "3":
                    Console.Clear();
                    CreateRoom();
                    break;
                case "4":
                    Console.Clear();
                    ManageTemplates();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Voer een geldige optie in.");
                    break;
            }
        }

        UserLogic.Logout();
        Console.WriteLine("\nDruk op een toets om door te gaan...");
        Console.ReadKey();
    }
    private static string ReadMaskedPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password;
    }


    private static void CreateEmployee(string role)
    {
        Console.Clear();
        string displayRole = role == "specialty" ? "caregiver" : role;
        Console.WriteLine($"\n-- Nieuwe {displayRole} toevoegen --");
        Console.WriteLine("Voer een e-mailadres in:");
        string? email = Console.ReadLine();
        Console.WriteLine("Voer een wachtwoord in:");
        string? password = ReadMaskedPassword();
        Console.WriteLine("Voer een volledige naam in:");
        string? fullname = Console.ReadLine();

        string specialty = "";
        if (role == "specialty")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Voer specialisatie in:");
            Console.ResetColor();
            specialty = Console.ReadLine() ?? "";
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullname))
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Alle velden zijn verplicht.");
            Console.ResetColor();
            return;
        }

        bool ok = userLogic.CreateEmployee(email, password, fullname, role, specialty);
        if (ok)
            Console.WriteLine($"Nieuwe {displayRole} '{fullname}' succesvol aangemaakt.");
        else
            Console.WriteLine("Er bestaat al een account met dit e-mailadres.");
    }

    private static void CreateRoom()
    {
        Console.Clear();
        Console.WriteLine("\n-- Nieuwe kamer toevoegen --");

        Console.WriteLine("Voer kamernaam in:");
        string? name = Console.ReadLine();

        Console.WriteLine("Voer kamertype in:");
        string? type = Console.ReadLine();

        Console.WriteLine("Voer kamerlokatie in:");
        string? location = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Kamernaam is verplicht.");
            return;
        }

        roomAccess.AddRoom(new RoomModel
        {
            Name = name,
            Type = type ?? "",
            Location = location ?? ""
        });

        Console.WriteLine($"Kamer '{name}' succesvol toegevoegd.");
    }

    // template management submenu, admin gets here via option 4
    private static void ManageTemplates()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n==== Afspraken Templates ====");
            Console.WriteLine("1. Nieuw Template maken");
            Console.WriteLine("2. Alle Templates bekijken");
            Console.WriteLine("3. Templates verwijderen");
            Console.WriteLine("0. Terug");

            string? input = Console.ReadLine()?.Trim();
            switch (input)
            {
                case "1":
                    CreateTemplate();
                    break;
                case "2":
                    ShowTemplates();
                    break;
                case "3":
                    DeleteTemplate();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Voer een geldige optie in.");
                    break;
            }
        }
    }

    // asks for name, type and note then saves it
    private static void CreateTemplate()
    {
        Console.Clear();
        Console.WriteLine("\n-- Nieuwe Template --");

        Console.Write("Naam van de template (bijv. 'Zwangerschapscontrole week 20'): ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Naam is verplicht.");
            return;
        }

        string[] types = { "Checkup", "Consultation", "Surgery", "Emergency", "General" };
        Console.WriteLine("Selecteer een afspraaktype:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");

        Console.Write("Choice: ");
        string? typeInput = Console.ReadLine();
        if (!int.TryParse(typeInput, out int typeIdx) || typeIdx < 1 || typeIdx > types.Length)
        {
            Console.WriteLine("Voer een geldige optie in.");
            return;
        }
        string selectedType = types[typeIdx - 1];

        Console.WriteLine("Template infortmatie over de afspraak (optioneel):");
        string notes = Console.ReadLine() ?? "";

        templateAccess.AddTemplate(new TemplateModel
        {
            Name = name,
            Type = selectedType,
            Notes = notes
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Template '{name}' aangemaakt.");
        Console.ResetColor();
        Console.ReadKey();
    }

    private static void ShowTemplates()
    {
        Console.Clear();
        List<TemplateModel> templates = templateAccess.GetAll();
        Console.WriteLine("\n-- Alle Templates --");

        if (templates.Count == 0)
        {
            Console.WriteLine("Nog geen Templates.");
            Console.ReadKey();
            return;
        }

        foreach (TemplateModel t in templates)
        {
            Console.WriteLine($"  [{t.Id}] {t.Name} | Type: {t.Type}");
            if (!string.IsNullOrWhiteSpace(t.Notes))
                Console.WriteLine($"       Notities: {t.Notes}");
        }

        Console.WriteLine("\nDruk op een toets om terug te gaan...");
        Console.ReadKey();
    }

    private static void DeleteTemplate()
    {
        Console.Clear();
        List<TemplateModel> templates = templateAccess.GetAll();
        if (templates.Count == 0)
        {
            Console.WriteLine("Geen Templates om te verwijderen.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\n-- Verwijder Template --");
        for (int i = 0; i < templates.Count; i++)
            Console.WriteLine($"  {i + 1}. {templates[i].Name} ({templates[i].Type})");

        Console.Write("Choice (0 = cancel): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice == 0) return;
        if (choice < 1 || choice > templates.Count)
        {
            Console.WriteLine("Voer een geldige optie in.");
            return;
        }

        TemplateModel toDelete = templates[choice - 1];
        templateAccess.Delete(toDelete.Id);
        Console.WriteLine($"Template '{toDelete.Name}' verwijdererd.");
        Console.ReadKey();
    }
}
