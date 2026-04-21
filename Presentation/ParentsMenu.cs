static class ParentMenu
{
    private static ReservationAccess reservationAccess = new ReservationAccess();

    public static void Start(UserModel user)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("\n==== Parent Main Menu ====");
            Console.WriteLine("1. View my next appointment");
            Console.WriteLine("2. View all my appointments");
            Console.ForegroundColor = ConsoleColor.Red ;
            Console.WriteLine("0. Log out");
            Console.ResetColor();

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a valid option.");
                continue;
            }

            switch (input.Trim())
            {
                case "1":
                    ShowNextAppointment(user);
                    break;
                case "2":
                    ShowAllAppointments(user);
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }
    }

    private static void ShowNextAppointment(UserModel user)
    {
        ReservationModel? next = reservationAccess.GetNextActiveReservationByUserId(user.Id);
        Console.WriteLine("\n-- Next appointment --");
        if (next == null)
        {
            Console.WriteLine("You have no active upcoming appointments.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            return;
        }
        Console.WriteLine("Date: " + next.Date);
        Console.WriteLine("Time: " + next.Time);
        Console.WriteLine("Room: " + next.RoomNumber);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    private static void ShowAllAppointments(UserModel user)
    {
        List<ReservationModel> list = reservationAccess.GetAllReservationsByUserId(user.Id);
        Console.WriteLine("\n-- All appointments --");
        if (list.Count == 0)
        {
            Console.WriteLine("You have no appointments.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            return;
        }
        foreach (ReservationModel r in list)
        {
            Console.WriteLine($"{r.Date} {r.Time} | Room {r.RoomNumber} | Status: {r.Status}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}

