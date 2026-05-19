static class Menu
{
    public static void Start()
    {
        bool check = true;
        while (check)
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
            Console.WriteLine("Welcome to the hospital appointment system.");
            Console.WriteLine("Log in or create an account to manage your appointments.");
            Console.WriteLine("\n=== Main Menu ===");
            Console.WriteLine("Enter 1 to login");
            Console.WriteLine("Enter 2 to make account");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Enter Q to quit");
            Console.ResetColor();

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a valid option.");
                continue;
            }

            input = input.Trim().ToUpper();

            if (input == "1")
            {
                UserLogin.Start();
            }
            else if (input == "2")
            {
                UserRegister.Start();
            }
            else if (input == "Q")
            {

                break;
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }
}