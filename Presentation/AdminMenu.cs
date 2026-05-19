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
            Console.WriteLine("\n==== Admin Menu ====");
            Console.WriteLine("1. Add a new hulpverlener");
            Console.WriteLine("2. Add a new planner");
            Console.WriteLine("3. Add a new room");
            Console.WriteLine("4. Beheer afspraak templates");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("0. Log out");
            Console.ResetColor();

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a valid option.");
                continue;
            }

            switch (input.Trim())
            {
                case "1":
                    CreateEmployee("specialty");
                    break;
                case "2":
                    CreateEmployee("planner");
                    break;
                case "3":
                    CreateRoom();
                    break;
                case "4":
                    ManageTemplates();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }

        UserLogic.Logout();
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void CreateEmployee(string role)
    {
        string displayRole = role == "specialty" ? "hulpverlener" : role;
        Console.WriteLine($"\n-- Add new {displayRole} --");

        Console.WriteLine("Enter email:");
        string? email = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string? password = Console.ReadLine();
        Console.WriteLine("Enter full name:");
        string? fullname = Console.ReadLine();

        string specialty = "";
        if (role == "specialty")
        {
            Console.WriteLine("Enter specialization:");
            specialty = Console.ReadLine() ?? "";
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullname))
        {
            Console.WriteLine("All fields are required.");
            return;
        }

        bool ok = userLogic.CreateEmployee(email, password, fullname, role, specialty);
        if (ok)
        {
            Console.WriteLine($"New {displayRole} '{fullname}' created successfully.");
        }
        else
        {
            Console.WriteLine("An account with this email already exists.");
        }
    }

    private static void CreateRoom()
    {
        Console.WriteLine("\n-- Add new room --");

        Console.WriteLine("Enter room name:");
        string? name = Console.ReadLine();
        Console.WriteLine("Enter room type:");
        string? type = Console.ReadLine();
        Console.WriteLine("Enter room location:");
        string? location = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Room name is required.");
            return;
        }

        roomAccess.AddRoom(new RoomModel
        {
            Name = name,
            Type = type ?? "",
            Location = location ?? ""
        });

        Console.WriteLine($"Room '{name}' added successfully.");
    }

    // submenu voor template beheer, admin komt hier via optie 4
    private static void ManageTemplates()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("\n==== Afspraak Templates ====");
            Console.WriteLine("1. Nieuw template aanmaken");
            Console.WriteLine("2. Alle templates bekijken");
            Console.WriteLine("3. Template verwijderen");
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
                    Console.WriteLine("Ongeldige keuze.");
                    break;
            }
        }
    }

    // vraagt naam, type en notitie en slaat het op
    private static void CreateTemplate()
    {
        Console.WriteLine("\n-- Nieuw template --");

        Console.Write("Naam van het template (bijv. 'Zwangerschapscontrole week 20'): ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Naam is verplicht.");
            return;
        }

        string[] types = { "Controle", "Consult", "Operatie", "Spoedgeval", "Algemeen" };
        Console.WriteLine("Selecteer afspraaktype:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");

        Console.Write("Keuze: ");
        string? typeInput = Console.ReadLine();
        if (!int.TryParse(typeInput, out int typeIdx) || typeIdx < 1 || typeIdx > types.Length)
        {
            Console.WriteLine("Ongeldige keuze.");
            return;
        }
        string selectedType = types[typeIdx - 1];

        Console.Write("Extra notities voor de planner (mag leeg): ");
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
        List<TemplateModel> templates = templateAccess.GetAll();
        Console.WriteLine("\n-- Alle templates --");

        if (templates.Count == 0)
        {
            Console.WriteLine("Nog geen templates aangemaakt.");
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
        List<TemplateModel> templates = templateAccess.GetAll();
        if (templates.Count == 0)
        {
            Console.WriteLine("Geen templates om te verwijderen.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\n-- Template verwijderen --");
        for (int i = 0; i < templates.Count; i++)
            Console.WriteLine($"  {i + 1}. {templates[i].Name} ({templates[i].Type})");

        Console.Write("Keuze (0 = annuleren): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice == 0) return;
        if (choice < 1 || choice > templates.Count)
        {
            Console.WriteLine("Ongeldige keuze.");
            return;
        }

        TemplateModel toDelete = templates[choice - 1];
        templateAccess.Delete(toDelete.Id);
        Console.WriteLine($"Template '{toDelete.Name}' verwijderd.");
        Console.ReadKey();
    }
}
