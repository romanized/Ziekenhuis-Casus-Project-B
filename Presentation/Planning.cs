using System.Threading.Tasks;

static class Planning
{
    static private ReservationLogic userLogic = new ReservationLogic();


    public static void Start()
    {
        Console.WriteLine("Welcome to the Reservation page");
        
        Console.WriteLine("Please enter your User_ID");
        int? User_ID = int.Parse(Console.ReadLine());

        Console.WriteLine("Please enter your Room_ID");
        int? Room_ID = int.Parse(Console.ReadLine());

        Console.WriteLine("Please enter your Specialist_ID");
        int? Specialist_ID = int.Parse(Console.ReadLine());

        Console.WriteLine("Please enter your InfoFolder_ID");
        int? InfoFolder_ID = int.Parse(Console.ReadLine());

        Console.WriteLine("Please enter your Date");
        string? Date = Console.ReadLine();

        Console.WriteLine("Please enter your Type");
        string? Type = Console.ReadLine();

        Console.WriteLine("Please enter your Doel");
        string? Doel = Console.ReadLine();

        Console.WriteLine("Please enter your Status");
        string? Status = Console.ReadLine();

        Console.WriteLine("Please enter your Sleutel 1");
        string? Sleutel1 = Console.ReadLine();

        Console.WriteLine("Please enter your Sleutel 2");
        string? Sleutel2 = Console.ReadLine();

        ReservationLogic reservationlogic = new();
        reservationlogic.Make(
            (Int64)User_ID,
            (Int64)Room_ID,
            (Int64)Specialist_ID,
            (Int64)InfoFolder_ID,
            Date,
            Type,
            Doel,
            Status,
            Sleutel1,
            Sleutel2
        );
        


    }
}