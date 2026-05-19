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
            Console.WriteLine("Ongeldig e-mailadres. Een geldig adres bevat '@' en een '.' (bijv. naam@mail.nl).");
        }
    }

    private static string AskPassword()
    {
        while (true)
        {
            Console.Write("Enter your Password (min 6 chars, minstens 1 cijfer): ");
            string password = ReadMaskedPassword();
            if (IsValidPassword(password)) return password;
            Console.WriteLine("Wachtwoord moet minimaal 6 tekens zijn en minimaal 1 cijfer bevatten.");
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
        Console.Clear();
        UserLogic userLogic = new();
        Console.WriteLine("=== Registratie patiënt ===");
        Console.WriteLine();
        Console.WriteLine("Welkom bij de registratie van het ziekenhuis.");
        Console.WriteLine("Tijdens deze registratie vult u uw gegevens in, zodat het ziekenhuis uw zorg en afspraken goed kan voorbereiden.");
        Console.WriteLine("Alle informatie wordt vertrouwelijk behandeld en alleen gebruikt voor uw zorgtraject.");
        Console.WriteLine();

        string email = AskEmail();
        string password = AskPassword();

        Console.Write("Enter your Full Name: ");
        string? fullname = Console.ReadLine();

        Console.Write("Enter your BirthDate (yyyy-mm-dd): ");
        string? birthdateInput = Console.ReadLine();

        if (!DateTime.TryParse(birthdateInput, out DateTime birthdate))
        {
            Console.WriteLine("Invalid birthdate format.");
            return;
        }

        Console.Write("Enter your Phone number: ");
        string? phonenumber = Console.ReadLine();

        Console.Write("Enter your Pregnancy start date (yyyy-mm-dd): ");
        string? startdateInput = Console.ReadLine();

        if (!DateTime.TryParse(startdateInput, out DateTime startdate))
        {
            Console.WriteLine("Invalid start date.");
            return;
        }

        if (string.IsNullOrWhiteSpace(fullname) || string.IsNullOrWhiteSpace(phonenumber))
        {
            Console.WriteLine("Name and phone number cannot be empty.");
            return;
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

        Console.WriteLine(ok
            ? "Account created! You can now log in."
            : "An account with this email already exists.");
    }
}