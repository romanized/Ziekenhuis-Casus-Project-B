static class UserRegister
{
    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains('@') && email.EndsWith(".com");
    }

    private static bool IsValidPassword(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Length >= 6;
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
    Console.WriteLine("Welcome to Register Page");

    Console.Write("Enter your Email: ");
    string? email = Console.ReadLine();

    if (!IsValidEmail(email ?? ""))
    {
        Console.WriteLine("Invalid email format.");
        return;
    }

    Console.Write("Enter your Password (min 6 chars): ");
    string password = ReadMaskedPassword();

    if (!IsValidPassword(password))
    {
        Console.WriteLine("Password must be at least 6 characters.");
        return;
    }

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

    bool ok = userLogic.Register(
        email!,
        password,
        fullname,
        birthdate.ToString("yyyy-MM-dd"),
        phonenumber,
        startdate.ToString("yyyy-MM-dd")
    );

    Console.WriteLine(ok
        ? "Account created! You can now log in."
        : "An account with this email already exists.");
}
}