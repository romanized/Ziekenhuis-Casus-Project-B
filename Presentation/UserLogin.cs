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
        Console.WriteLine("Welcome to the login page");

        Console.WriteLine("Please enter your email address");
        string? email = Console.ReadLine();
        Console.WriteLine("Please enter your password");
        string password = ReadMaskedPassword();

        UserModel acc = userLogic.CheckLogin(email!, password!);
        if (acc == null)
        {
            Console.WriteLine("No account found with that email and password");
            return;
        }

        Console.WriteLine("Welcome back " + acc.FullName);
        switch (acc.Role)
        {
            case "admin":
                AdminMenu.Start(acc);
                break;
            case "specialty":
                DoctorMenu.Start(acc);
                break;
            case "planner":
                PlannerMenu.Start(acc);
                break;
            case "ouder":
                ParentMenu.Start(acc);
                break;
        }
    }
}