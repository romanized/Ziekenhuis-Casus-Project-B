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
            Console.WriteLine("1. Add a new caregiver");
            Console.WriteLine("2. Add a new planner");
            Console.WriteLine("3. Add a new room");
            Console.WriteLine("4. Manage appointment templates");
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
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }

        UserLogic.Logout();
        Console.WriteLine("\nPress any key to continue...");
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
        Console.WriteLine($"\n-- Add new {displayRole} --");

        Console.WriteLine("Enter email:");
        string? email = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string? password = ReadMaskedPassword();
        Console.WriteLine("Enter full name:");
        string? fullname = Console.ReadLine();

        string specialty = "";
        if (role == "specialty")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Enter specialization:");
            Console.ResetColor();
            specialty = Console.ReadLine() ?? "";
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullname))
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("All fields are required.");
            Console.ResetColor();
            return;
        }

        bool ok = userLogic.CreateEmployee(email, password, fullname, role, specialty);
        if (ok)
            Console.WriteLine($"New {displayRole} '{fullname}' created successfully.");
        else
            Console.WriteLine("An account with this email already exists.");
    }

    private static void CreateRoom()
    {
        Console.Clear();
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

    // template management submenu, admin gets here via option 4
    private static void ManageTemplates()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("\n==== Appointment Templates ====");
            Console.WriteLine("1. Create new template");
            Console.WriteLine("2. View all templates");
            Console.WriteLine("3. Delete template");
            Console.WriteLine("0. Back");

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
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    // asks for name, type and note then saves it
    private static void CreateTemplate()
    {
        Console.Clear();
        Console.WriteLine("\n-- New template --");

        Console.Write("Template name (e.g. 'Pregnancy check week 20'): ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name is required.");
            return;
        }

        string[] types = { "Checkup", "Consultation", "Surgery", "Emergency", "General" };
        Console.WriteLine("Select appointment type:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");

        Console.Write("Choice: ");
        string? typeInput = Console.ReadLine();
        if (!int.TryParse(typeInput, out int typeIdx) || typeIdx < 1 || typeIdx > types.Length)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }
        string selectedType = types[typeIdx - 1];

        Console.Write("Extra notes for the planner (optional): ");
        string notes = Console.ReadLine() ?? "";

        templateAccess.AddTemplate(new TemplateModel
        {
            Name = name,
            Type = selectedType,
            Notes = notes
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Template '{name}' created.");
        Console.ResetColor();
        Console.ReadKey();
    }

    private static void ShowTemplates()
    {
        Console.Clear();
        List<TemplateModel> templates = templateAccess.GetAll();
        Console.WriteLine("\n-- All templates --");

        if (templates.Count == 0)
        {
            Console.WriteLine("No templates yet.");
            Console.ReadKey();
            return;
        }

        foreach (TemplateModel t in templates)
        {
            Console.WriteLine($"  [{t.Id}] {t.Name} | Type: {t.Type}");
            if (!string.IsNullOrWhiteSpace(t.Notes))
                Console.WriteLine($"       Notes: {t.Notes}");
        }

        Console.WriteLine("\nPress any key to go back...");
        Console.ReadKey();
    }

    private static void DeleteTemplate()
    {
        Console.Clear();
        List<TemplateModel> templates = templateAccess.GetAll();
        if (templates.Count == 0)
        {
            Console.WriteLine("No templates to delete.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\n-- Delete template --");
        for (int i = 0; i < templates.Count; i++)
            Console.WriteLine($"  {i + 1}. {templates[i].Name} ({templates[i].Type})");

        Console.Write("Choice (0 = cancel): ");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int choice) || choice == 0) return;
        if (choice < 1 || choice > templates.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        TemplateModel toDelete = templates[choice - 1];
        templateAccess.Delete(toDelete.Id);
        Console.WriteLine($"Template '{toDelete.Name}' deleted.");
        Console.ReadKey();
    }
}
