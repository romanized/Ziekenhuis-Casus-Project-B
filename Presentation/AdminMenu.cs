static class AdminMenu
{
    private static UserLogic userLogic = new UserLogic();

    public static void Start(UserModel admin)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n==== Admin Menu ====");
            Console.WriteLine("1. Add a new doctor");
            Console.WriteLine("2. Add a new planner");
            Console.WriteLine("0. Log out");

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
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }
    }

    private static void CreateEmployee(string role)
    {
        Console.WriteLine($"\n-- Add new {role} --");

        Console.WriteLine("Enter email:");
        string? email = Console.ReadLine();
        Console.WriteLine("Enter password:");
        string? password = Console.ReadLine();
        Console.WriteLine("Enter full name:");
        string? fullname = Console.ReadLine();

        // only doctors (role "specialty") have a specialty field
        string specialty = "";
        if (role == "specialty")
        {
            Console.WriteLine("Enter specialty:");
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
            Console.WriteLine($"New {role} '{fullname}' created successfully.");
        }
        else
        {
            Console.WriteLine("An account with this email already exists.");
        }
    }
}
