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
        Console.Clear();
        UserLogic userLogic = new();
        Console.WriteLine("Welcome to Register Page");    


        string? email;
        do
        {
            Console.WriteLine("Enter your Email (must contain @ and end with .com):");
            email = Console.ReadLine();
            if (!IsValidEmail(email!))
            {
                Console.WriteLine("Invalid email format!");
            }
        } while (!IsValidEmail(email!));
        string? password;
        do
        {
            Console.WriteLine("Enter your Password (min 6 characters):");
            password = ReadMaskedPassword();
            if (!IsValidPassword(password))
            {
                Console.WriteLine("Password must be at least 6 characters!");
            }
        } while (!IsValidPassword(password));
        Console.WriteLine("Enter your FullName");
        string? fullname = Console.ReadLine();
        Console.WriteLine("Enter your BirthDate");
        string? birthdate = Console.ReadLine();
        Console.WriteLine("Enter your Phonenumber");
        string? phonenumber = Console.ReadLine();
        Console.WriteLine("Enter your Pregnancy startdate");
        string? startdate = Console.ReadLine();

        bool ok = userLogic.Register(email!, password!, fullname!, birthdate!, phonenumber!, startdate!);
        if (ok)
        {
            Console.WriteLine("Account created! You can now log in.");
        }
        else
        {
            Console.WriteLine("An account with this email already exists.");
        }
    }
}