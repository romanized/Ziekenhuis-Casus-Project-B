static class ParentMenu
{
    private static ReservationAccess reservationAccess = new ReservationAccess();
    private static RoomAccess roomAccess = new RoomAccess();

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

        UserLogic.Logout();
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void ShowAppointmentOverview(UserModel user)
    {
        bool viewing = true;
        while (viewing)
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

            List<ReservationModel> combined = upcomingAppointments.Concat(pastAppointments).ToList();

            Console.WriteLine("\n================ APPOINTMENT OVERVIEW ================");
            PrintSideBySideTables(upcomingAppointments, pastAppointments);

            Console.WriteLine();
            Console.Write("Voer een nummer in om een afspraak te bekijken (of Enter om terug te gaan): ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                viewing = false;
                continue;
            }

            int choice;
            if (!int.TryParse(input.Trim(), out choice) || choice < 1 || choice > combined.Count)
            {
                Console.WriteLine("Ongeldig nummer.");
                continue;
            }

            ShowAppointmentDetail(combined[choice - 1]);
        }
    }

    private static void ShowAppointmentDetail(ReservationModel appointment)
    {
        string location = roomAccess.GetRoomLocationById(appointment.RoomId) ?? "";
        string doctor = string.IsNullOrWhiteSpace(appointment.DoctorName) ? "Nog niet toegewezen" : appointment.DoctorName;

        Console.WriteLine("\n================ AFSPRAAK DETAILS ================");
        Console.WriteLine($"Datum:           {appointment.Date}");
        Console.WriteLine($"Tijd:            {appointment.Time}");
        Console.WriteLine($"Type afspraak:   {appointment.Type}");
        Console.WriteLine($"Status:          {appointment.Status}");
        Console.WriteLine($"Kamer:           {appointment.RoomNumber}");
        Console.WriteLine($"Locatie:         {location}");
        Console.WriteLine($"Arts:            {doctor}");
        Console.WriteLine();
        Console.WriteLine("Druk op Enter om terug te gaan...");
        Console.ReadLine();
    }

    private static void PrintSideBySideTables(List<ReservationModel> upcoming, List<ReservationModel> past)
    {
        List<string> leftTable = BuildTableLines("Upcoming appointments", upcoming, 1);
        List<string> rightTable = BuildTableLines("Past appointments", past, upcoming.Count + 1);

        int leftWidth = 50;
        int maxLines = Math.Max(leftTable.Count, rightTable.Count);

        for (int i = 0; i < maxLines; i++)
        {
            string leftLine = i < leftTable.Count ? leftTable[i] : "";
            string rightLine = i < rightTable.Count ? rightTable[i] : "";

            Console.WriteLine(leftLine.PadRight(leftWidth) + "   " + rightLine);
        }
    }

    private static List<string> BuildTableLines(string title, List<ReservationModel> appointments, int startIndex)
    {
        List<string> lines = new List<string>();

        lines.Add(title);
        lines.Add("---------------------------------------------");
        lines.Add(string.Format("{0,-4} {1,-12} {2,-6} {3,-12}", "#", "Date", "Time", "Room"));
        lines.Add("---------------------------------------------");

        if (appointments.Count == 0)
        {
            lines.Add("No appointments");
            return lines;
        }

        int index = startIndex;
        foreach (ReservationModel appointment in appointments)
        {
            lines.Add(string.Format("{0,-4} {1,-12} {2,-6} {3,-12}",
                index,
                appointment.Date,
                appointment.Time,
                appointment.RoomNumber));
            index++;
        }

        return lines;
    }
}
