static class UserRegister
{
    public static void Start()
    {
        UserLogic userLogic = new();
        Console.WriteLine("Welcome to Register Page");

        Console.WriteLine("Enter your Email");
        string? email = Console.ReadLine();
        Console.WriteLine("Enter your Password");
        string? password = Console.ReadLine();
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