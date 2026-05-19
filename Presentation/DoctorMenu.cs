static class DoctorMenu
{
    private static DoctorAccess doctorAccess = new DoctorAccess();
    private static UserAccess userAccess = new UserAccess();
    private static RoomAccess roomAccess = new RoomAccess();

    public static void Start(UserModel doctor)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
        ______ _      _              _           _     
        |___  /(_)    | |            | |         (_)    
            / /  _  ___| | _____ _ __ | |__  _   _ _ ___
        / /  | |/ _ \ |/ / _ \ '_ \| '_ \| | | | / __|
        / /__ | |  __/   <  __/ | | | | | | |_| | \__ \
        /_____||_|\___|_|\_\___|_| |_|_| |_|\__,_|_|___/

        ");
            Console.ResetColor();
            Console.WriteLine("\n==== Hulpverlener Main Menu ====");
            Console.WriteLine($"Logged in as: {doctor.FullName}" +
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
                    Console.Clear();
                    ShowNextAppointment(doctor);
                    break;
                case "2":
                    Console.Clear();
                    ShowAllAppointments(doctor);
                    break;
                case "3":
                    Console.Clear();
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

        UserLogic.Logout();
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void ShowNextAppointment(UserModel doctor)
    {
        Console.Clear();
        ReservationModel? next = doctorAccess.GetNextActiveReservationByDoctorId(doctor.Id);
        Console.WriteLine("\n-- Next appointment --");
        if (next == null)
        {
            Console.WriteLine("You have no active upcoming appointments.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Date: " + next.Date);
        Console.WriteLine("Time: " + next.Time);
        Console.WriteLine("Room: " + next.RoomNumber);
            
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private static void ShowAllAppointments(UserModel doctor)
    {
        Console.Clear();
        List<ReservationModel> list = doctorAccess.GetAllReservationsByDoctorId(doctor.Id);
        Console.WriteLine("\n-- All appointments --");
        if (list.Count == 0)
        {
            Console.WriteLine("You have no appointments.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            return;
        }
        foreach (ReservationModel r in list)
        {
            Console.WriteLine($"{r.Date} {r.Time} | Room {r.RoomNumber} | Status: {r.Status}");
        }
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    public static void ShowAgenda(UserModel doctor)
    {
        Console.Clear();
        Console.WriteLine("Enter date (YYYY/MM/DD):");

        DateTime date;
        while (true)
        {
            string? dateInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dateInput))
            {
                return;
            }
            if (DateTime.TryParse(dateInput, out date))
            {
                break;
            }
            Console.WriteLine("Ongeldige datum. Probeer opnieuw.");
        }

        while (true)
        {
            List<ReservationModel> list =
                doctorAccess.GetAllReservationsByDoctorIdByDate(doctor.Id, date);

            AgendaTemplate(doctor, date, list);

            Console.WriteLine("\nN = Next day | P = Previous day |Type number to view appointment details X = Exit");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

switch (input.Trim().ToUpper())
{
    case "N":
        date = date.AddDays(1);
        break;

    case "P":
        date = date.AddDays(-1);
        break;

    case "X":
        return;

    default:

        int number;

        if (int.TryParse(input, out number))
        {
            DateTime selectedTime = date.Date
                .AddHours(8)
                .AddMinutes((number - 1) * 30);

            ReservationModel? selectedAppointment = null;

            foreach (ReservationModel r in list)
            {
                if (r.Time == selectedTime.ToString("HH:mm"))
                {
                    selectedAppointment = r;
                    break;
                }
            }

            if (selectedAppointment != null)
            {
                Console.Clear();

                Console.WriteLine("=== Appointment Details ===");

                Console.WriteLine($"Date: {selectedAppointment.Date}");
                Console.WriteLine($"Time: {selectedAppointment.Time}");
                Console.WriteLine($"Room: {selectedAppointment.RoomNumber}");
                Console.WriteLine($"Status: {selectedAppointment.Status}");

                string patientName =
                    userAccess.GetFullNameById(selectedAppointment.UserId)
                    ?? "Unknown";

                Console.WriteLine($"Patient: {patientName}");

                Console.WriteLine("\nPress any key...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("No appointment in this slot.");
                Console.ReadKey();
            }
        }

        break;
}
        }
    }

    public static void AgendaTemplate(UserModel doctor, DateTime date, List<ReservationModel> list)
    {
        Console.Clear();
        Console.WriteLine($"\n-- Agenda for {date:yyyy-MM-dd} --");

        DateTime time = date.Date.AddHours(8);
        ReservationModel appointment = null;

        for (int i = 0; i < 19; i++)
        {
            bool found = false;
            appointment = null;

            foreach (var r in list)
            {
                if (r.Time == time.ToString("HH:mm"))
                {
                    found = true;
                    appointment = r;
                    break;
                }
            }

            if (found && appointment != null)
            {
                string name = userAccess.GetFullNameById(appointment.UserId) ?? "Onbekend";

                string roomname = roomAccess.GetRoomNameById(appointment.RoomId) ?? "";

                Console.WriteLine($"{time:HH:mm} | Patient: {name} | Room: {roomname}");
            }
            else
            {
                Console.WriteLine($"{time:HH:mm} | Available");
            }

            time = time.AddMinutes(30);
        }
    }
}
