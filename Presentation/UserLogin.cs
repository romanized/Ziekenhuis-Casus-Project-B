using System.Threading.Tasks;

static class UserLogin
{
    static private UserLogic userLogic = new UserLogic();


    public static async Task Start()
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
            if (acc.Role == "admin")
            {
                Console.WriteLine("You are admin");   
            }
            //Console.WriteLine("Your are a  " + acc.Specialty);

            Console.WriteLine("Press 0 to go back to Main menu");
            int? result =  int.Parse(Console.ReadLine()!);
            if(result == 0)
            {         
                Menu.Start();
            }
        }
        else
        {
            Console.WriteLine("No account found with that email and password");
        }
    }
}