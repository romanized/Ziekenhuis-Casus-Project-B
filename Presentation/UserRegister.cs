static class UserRegister
{
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        int at = email.IndexOf('@');
        if (at <= 0) return false;
        int dot = email.IndexOf('.', at);
        return dot > at + 1 && dot < email.Length - 1;
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6) return false;
        return password.Any(char.IsDigit);
    }

    private static string AskEmail()
    {
        while (true)
        {
            Console.Write("Enter your Email: ");
            string? email = Console.ReadLine();
            if (IsValidEmail(email ?? "")) return email!;
            Console.WriteLine("Invalid email. A valid address contains '@' and '.' (e.g. name@mail.com).");
        }
    }

    private static string AskPassword()
    {
        while (true)
        {
            Console.Write("Enter your Password (min 6 chars, at least 1 digit): ");
            string password = ReadMaskedPassword();
            if (IsValidPassword(password)) return password;
            Console.WriteLine("Password must be at least 6 characters and contain at least 1 digit.");
        }
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
    public static void Start()
    {
        UserLogic userLogic = new();
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== Patient registration ===");
            Console.WriteLine();
            Console.WriteLine("Welcome to the hospital registration.");
            Console.WriteLine("Fill in your details so the hospital can prepare your care and appointments.");
            Console.WriteLine();

            string email = AskEmail();
            string password = AskPassword();

            Console.Write("Enter your Full Name: ");
            string? fullname = Console.ReadLine();

            Console.Write("Enter your BirthDate (yyyy-mm-dd): ");
            string? birthdateInput = Console.ReadLine();

            if (!DateTime.TryParse(birthdateInput, out DateTime birthdate))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid birthdate format.");
                Console.ResetColor();
                Console.ReadKey();
                continue;
            }

            Console.Write("Enter your Phone number: ");
            string? phonenumber = Console.ReadLine();

            Console.Write("Enter your Pregnancy start date (yyyy-mm-dd): ");
            string? startdateInput = Console.ReadLine();

            if (!DateTime.TryParse(startdateInput, out DateTime startdate))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid start date.");
                Console.ResetColor();
                Console.ReadKey();
                continue;
            }

            if (string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(phonenumber))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Name and phone number cannot be empty.");
                Console.ResetColor();
                Console.ReadKey();
                continue;
            }

            Console.Write("Enter your Notes: ");
            string? notes = Console.ReadLine();

            bool ok = userLogic.Register(
                email,
                password,
                fullname,
                birthdate.ToString("yyyy-MM-dd"),
                phonenumber,
                startdate.ToString("yyyy-MM-dd"),
                notes
            );

            if (ok)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Account created! You can now log in.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An account with this email already exists.");
            }

            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Do you want to register another patient? (y/n)");
            string? choice = Console.ReadLine();

            if (choice?.ToLower() != "y")
            {
                running = false;
            }
        }
    }
}