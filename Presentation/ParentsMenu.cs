static class ParentMenu
{
    private static ReservationAccess reservationAccess = new ReservationAccess();

    public static void Start(UserModel user)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n==== Parent Main Menu ====");
            Console.WriteLine("1. View appointment overview");
            Console.ForegroundColor = ConsoleColor.Red;
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
                    ShowAppointmentOverview(user);
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

    private static void ShowAppointmentOverview(UserModel user)
    {
        List<ReservationModel> allAppointments = reservationAccess.GetAllReservationsByUserId(user.Id);

        List<ReservationModel> upcomingAppointments = new List<ReservationModel>();
        List<ReservationModel> pastAppointments = new List<ReservationModel>();

        DateTime now = DateTime.Now;

        foreach (ReservationModel appointment in allAppointments)
        {
            DateTime appointmentDateTime;
            bool validDate = DateTime.TryParse($"{appointment.Date} {appointment.Time}", out appointmentDateTime);

            if (!validDate)
            {
                continue;
            }

            if (appointmentDateTime >= now)
            {
                upcomingAppointments.Add(appointment);
            }
            else
            {
                pastAppointments.Add(appointment);
            }
        }

        Console.WriteLine("\n================ APPOINTMENT OVERVIEW ================\n");
        PrintSideBySideTables(upcomingAppointments, pastAppointments);

        Console.WriteLine();
        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    private static void PrintSideBySideTables(List<ReservationModel> upcoming, List<ReservationModel> past)
    {
        List<string> leftTable = BuildTableLines("Upcoming appointments", upcoming);
        List<string> rightTable = BuildTableLines("Past appointments", past);

        int leftWidth = 45;
        int maxLines = Math.Max(leftTable.Count, rightTable.Count);

        for (int i = 0; i < maxLines; i++)
        {
            string leftLine = i < leftTable.Count ? leftTable[i] : "";
            string rightLine = i < rightTable.Count ? rightTable[i] : "";

            Console.WriteLine(leftLine.PadRight(leftWidth) + "   " + rightLine);
        }
    }

    private static List<string> BuildTableLines(string title, List<ReservationModel> appointments)
    {
        List<string> lines = new List<string>();

        lines.Add(title);
        lines.Add("----------------------------------------");
        lines.Add(string.Format("{0,-12} {1,-6} {2,-12}", "Date", "Time", "Room"));
        lines.Add("----------------------------------------");

        if (appointments.Count == 0)
        {
            lines.Add("No appointments");
            return lines;
        }

        foreach (ReservationModel appointment in appointments)
        {
            lines.Add(string.Format("{0,-12} {1,-6} {2,-12}",
                appointment.Date,
                appointment.Time,
                appointment.RoomNumber));
        }

        return lines;
    }
}