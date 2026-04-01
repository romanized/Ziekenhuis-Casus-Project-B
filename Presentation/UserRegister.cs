static class UserRegister
{
    public static void Start()
    {
        UserLogic userLogic = new();
        Console.WriteLine("Welcome to Register Page");

        Console.WriteLine("Enter your Email");
        string email = Console.ReadLine();
        Console.WriteLine("enter your Password");
        string password = Console.ReadLine();
        Console.WriteLine("enter your FullName");
        string fullname = Console.ReadLine();
        Console.WriteLine("enter your BirthDate");
        string birthdate = Console.ReadLine();
        Console.WriteLine("enter your Phonenumber");
        string phonenumber = Console.ReadLine();

        userLogic.Register(email!,password!,fullname!,birthdate!,phonenumber!);

        
    }
    
}