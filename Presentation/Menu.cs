static class Menu
{
    public static void Start()
    {
        bool check = true ;
        while (check)
        {
            Console.WriteLine("\n=== Main Menu ===");
            Console.WriteLine("Enter 1 to login");
            Console.WriteLine("Enter 2 to make account");
            Console.WriteLine("Enter Q to quit");

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