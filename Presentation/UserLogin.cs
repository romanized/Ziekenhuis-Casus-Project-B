static class UserLogin
{
    static private UserLogic userLogic = new UserLogic();

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
        Console.WriteLine("Welkom op de inlogpagina");

        Console.WriteLine("Voer uw e-mailadres in");
        string? email = Console.ReadLine();

        Console.WriteLine("Voer uw wachtwoord in (invoer wordt verborgen)");
        string password = ReadMaskedPassword();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("E-mail en wachtwoord mogen niet leeg zijn.");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        UserModel? acc = userLogic.CheckLogin(email, password);

        if (acc == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Geen account gevonden met dat e-mailadres en wachtwoord");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Welkom terug " + acc.FullName);

        switch (acc.Role.ToLower())
        {
            case "admin":
                Console.Clear();
                AdminMenu.Start(acc);
                break;
            case "specialty":
                Console.Clear();
                DoctorMenu.Start(acc);
                break;
            case "planner":
                Console.Clear();
                PlannerMenu.Start(acc);
                break;
            case "ouder":
                Console.Clear();
                ParentMenu.Start(acc);
                break;
            default:
                Console.WriteLine("Onbekende rol.");
                break;
        }
    }
}