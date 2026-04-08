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
        if (acc != null)
        {
            Console.WriteLine("Welcome back " + acc.FullName);
            Console.WriteLine("Your email number is " + acc.Email);
            switch (acc.Role)
            {
<<<<<<< HEAD
                Console.WriteLine("You are admin");
            }
            //Console.WriteLine("Your are a  " + acc.Specialty);

            Console.WriteLine("Press 0 to go back to Main menu");
            Console.WriteLine("Press 1 to go to your appointments");
=======
                case "admin":
                    Console.WriteLine("Welcome back Admin");
                    break;
                case "doctor":
                    Console.WriteLine($"Welcome back doctor {acc.FullName}");
                    break;
                case "planner":
                    Console.WriteLine($"Welcome back planner {acc.FullName}");
                    break;
                case "ouder":
                    Console.WriteLine($"Welcome back {acc.FullName}");
                    break;
                default:
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Red;;
            Console.WriteLine("Press 0 to go to Logout");
            Console.ResetColor();
>>>>>>> origin/main
            int? result = int.Parse(Console.ReadLine()!);
            if (result == 0)
            {
                Menu.Start();
            }
            else if (result == 1)
            {
                ParentMenu.Start(acc);
            }
        }
        else
        {
            Console.WriteLine("No account found with that email and password");
        }
    }
}