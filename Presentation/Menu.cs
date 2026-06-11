static class Menu
{
    public static void Start()
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
            Console.WriteLine("Welkom bij het ziekenhuisafsprakensysteem.");
            Console.WriteLine("Log in of maak een account aan om uw afspraken te beheren.");
            Console.WriteLine("\n=== Hoofdmenu ===");
            Console.WriteLine("Voer 1 in om in te loggen");
            Console.WriteLine("Voer 2 in om een account aan te maken");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Voer Q in om af te sluiten");
            Console.ResetColor();

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Voer een geldige optie in.");
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
                Console.WriteLine("Voer een geldige optie in.");
            }
        }
    }
}