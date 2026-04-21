static class DoctorMenu
{
    private static DoctorAccess doctorAccess = new DoctorAccess();

    public static void Start(UserModel doctor)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n==== Doctor Main Menu ====");
            Console.WriteLine($"Logged in as: Dr. {doctor.FullName}" +
                              (string.IsNullOrWhiteSpace(doctor.Specialty) ? "" : $" ({doctor.Specialty})"));
            Console.WriteLine("1. View my next appointment");
            Console.WriteLine("2. View all my appointments");
            Console.WriteLine("3. View the Agenda for the day");
            Console.WriteLine("0. Log out");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a valid option.");
                continue;
            }

            switch (input.Trim())
            {
                case "1":
                    ShowNextAppointment(doctor);
                    break;
                case "2":
                    ShowAllAppointments(doctor);
                    break;
                case "3":
                    ShowAgenda(doctor);
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

    private static void ShowNextAppointment(UserModel doctor)
    {
        ReservationModel? next = doctorAccess.GetNextActiveReservationByDoctorId(doctor.Id);
        Console.WriteLine("\n-- Next appointment --");
        if (next == null)
        {
            Console.WriteLine("You have no active upcoming appointments.");
            return;
        }
        Console.WriteLine("Date: " + next.Date);
        Console.WriteLine("Time: " + next.Time);
        Console.WriteLine("Room: " + next.RoomNumber);
    }

    private static void ShowAllAppointments(UserModel doctor)
    {
        List<ReservationModel> list = doctorAccess.GetAllReservationsByDoctorId(doctor.Id);
        Console.WriteLine("\n-- All appointments --");
        if (list.Count == 0)
        {
            Console.WriteLine("You have no appointments.");
            return;
        }
        foreach (ReservationModel r in list)
        {
            Console.WriteLine($"{r.Date} {r.Time} | Room {r.RoomNumber} | Status: {r.Status}");
        }
    }
    public static void ShowAgenda(UserModel doctor)
    {

        Console.WriteLine("Please enter the date of the agenda with this format YYYY/MM/DD");
        // string date = Console.ReadLine();
        string date = "2026/04/25";
        DateTime newdate = Convert.ToDateTime(date);
        Console.WriteLine($"{newdate.Year} {newdate.Month} {newdate.Day}");
        List<ReservationModel> list = doctorAccess.GetAllReservationsByDoctorIdByDate(doctor.Id, newdate);
        Console.WriteLine("\n-- All appointments --");
        Console.WriteLine($"\n-- Agenda for {date:yyyy-MM-dd} --");

        if (list.Count == 0)
        {
            Console.WriteLine("No appointments for this day.");
            return;
        }

        foreach (var r in list)
        {
            Console.WriteLine($"{r.Time} | Room {r.RoomNumber} | Status: {r.Status}");
        }
}

        
        // foreach (ReservationModel r in list)
        // {


            
        //     Console.WriteLine($"{r.Date} {r.Time} | Room {r.RoomNumber} | Status: {r.Status}");
        // }
    }
}
