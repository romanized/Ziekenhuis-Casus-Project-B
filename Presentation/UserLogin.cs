static class UserLogin
{
    static private UserLogic userLogic = new UserLogic();

    public static void Start()
    {
        Console.WriteLine("Welcome to the login page");
        Console.WriteLine("Please enter your email address");
        string? email = Console.ReadLine();
        Console.WriteLine("Please enter your password");
        string? password = Console.ReadLine();

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
                Console.WriteLine($"Welcome doctor {acc.FullName}");
                break;
            case "planner":
                Console.WriteLine($"Welcome planner {acc.FullName}");
                break;
            case "ouder":
                ParentMenu.Start(acc);
                break;
        }
    }
}