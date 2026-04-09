static class PlannerMenu
{
    private static ReservationAccess reservationAccess = new ReservationAccess();
    private static RoomAccess roomAccess = new RoomAccess();
    private static UserAccess userAccess = new UserAccess();

    public static void Start(UserModel planner)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("\n==== Planner Menu ====");
            Console.WriteLine("1. View all appointments");
            Console.WriteLine("2. Create new appointment");
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
                    ShowAllAppointments();
                    break;
                case "2":
                    CreateAppointment();
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

    private static void ShowAllAppointments()
    {
        List<ReservationModel> list = reservationAccess.GetAllReservations();
        Console.WriteLine("\n-- All appointments --");
        if (list.Count == 0)
        {
            Console.WriteLine("No appointments found.");
            return;
        }
        foreach (ReservationModel r in list)
        {
            Console.WriteLine($"{r.Date} {r.Time} | Room: {r.RoomNumber} | Status: {r.Status}");
        }
    }

    private static void CreateAppointment()
    {
        Console.WriteLine("\n-- Create new appointment --");

        // Select patient
        List<UserModel> patients = userAccess.GetAllByRole("ouder");
        if (patients.Count == 0)
        {
            Console.WriteLine("No patients found.");
            return;
        }
        Console.WriteLine("\nSelect patient:");
        for (int i = 0; i < patients.Count; i++)
            Console.WriteLine($"{i + 1}. {patients[i].FullName} ({patients[i].Email})");
        if (!TryPickIndex(patients.Count, out int patientIdx)) return;
        UserModel patient = patients[patientIdx];

        // Select room
        List<RoomModel> rooms = roomAccess.GetAllRooms();
        if (rooms.Count == 0)
        {
            Console.WriteLine("No rooms available. Add a room first.");
            return;
        }
        Console.WriteLine("\nSelect room:");
        for (int i = 0; i < rooms.Count; i++)
            Console.WriteLine($"{i + 1}. {rooms[i].Name} ({rooms[i].Type}, {rooms[i].Location})");
        if (!TryPickIndex(rooms.Count, out int roomIdx)) return;
        RoomModel room = rooms[roomIdx];

        // Select doctor (optional)
        List<UserModel> doctors = userAccess.GetAllByRole("specialty");
        long? specialistId = null;
        if (doctors.Count > 0)
        {
            Console.WriteLine("\nSelect doctor (or press Enter to skip):");
            for (int i = 0; i < doctors.Count; i++)
                Console.WriteLine($"{i + 1}. {doctors[i].FullName}");
            Console.Write("Choice: ");
            string? docInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(docInput) && int.TryParse(docInput, out int docChoice)
                && docChoice >= 1 && docChoice <= doctors.Count)
            {
                specialistId = doctors[docChoice - 1].Id;
            }
        }

        // Date and time
        Console.WriteLine("\nEnter date (YYYY-MM-DD):");
        string? date = Console.ReadLine();
        Console.WriteLine("Enter time (HH:MM):");
        string? time = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(time))
        {
            Console.WriteLine("Date and time are required.");
            return;
        }

        // Type
        Console.WriteLine("Enter appointment type:");
        string? type = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(type)) type = "algemeen";

        string dateTime = $"{date} {time}";
        reservationAccess.CreateReservation(patient.Id, room.Id, specialistId, dateTime, type);
        Console.WriteLine($"Appointment created for {patient.FullName} on {date} at {time} in room {room.Name}.");
    }

    private static bool TryPickIndex(int count, out int index)
    {
        Console.Write("Choice: ");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int choice) && choice >= 1 && choice <= count)
        {
            index = choice - 1;
            return true;
        }
        Console.WriteLine("Invalid choice.");
        index = -1;
        return false;
    }
}
