static class PlannerMenu
{
    private static ReservationAccess reservationAccess = new ReservationAccess();
    private static RoomAccess roomAccess = new RoomAccess();
    private static UserAccess userAccess = new UserAccess();
    private static TemplateAccess templateAccess = new TemplateAccess();

    public static void Start(UserModel planner)
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
            Console.WriteLine("\n==== Planner Menu ====");
            Console.WriteLine("1. View agenda");
            Console.WriteLine("2. Create new appointment");
            Console.WriteLine("3. View room status");

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
                    ShowAgenda();
                    break;

                case "2":
                    CreateAppointment();
                    break;

                case "3":
                    ShowRoomStatus();
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

    private static void ShowRoomStatus()
    {
        Console.Clear();

        Console.Write("\nDate (YYYY-MM-DD, Enter = today): ");
        string? dateInput = Console.ReadLine();

        DateTime date = DateTime.Today;

        if (!string.IsNullOrWhiteSpace(dateInput) && !DateTime.TryParse(dateInput, out date))
        {
            Console.WriteLine("Invalid date.");
            Console.ReadKey();
            return;
        }

        string[] slots = GenerateTimeSlots();

        Console.WriteLine("\nSelect time slot:");
        for (int i = 0; i < slots.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {slots[i]}");
        }

        if (!TryPickIndex(slots.Length, out int slotIdx))
        {
            return;
        }

        string selectedTime = slots[slotIdx];

        Console.WriteLine($"\n-- Room status on {date:dd-MM-yyyy} at {selectedTime} --");

        List<RoomModel> rooms = roomAccess.GetAllRooms();
        List<ReservationModel> todays = reservationAccess.GetReservationsForDate(date.ToString("yyyy-MM-dd"));

        foreach (RoomModel room in rooms)
        {
            List<ReservationModel> inRoom = todays
                .Where(r => r.RoomId == room.Id && r.Time == selectedTime)
                .ToList();

            Console.Write($"Kamer {room.Name} ({room.Type}, {room.Location}) - Status: ");

            if (inRoom.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("VRIJ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"BEZET: {inRoom.Count} afspraak/afspraken");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (ReservationModel r in inRoom)
                {
                    Console.WriteLine($"    {r.Time} | {r.Type} | Patient: {r.PatientName}");
                }
                Console.ResetColor();
            }
        }

        Console.WriteLine("\nPress any key to go back...");
        Console.ReadKey();
    }

    private static void ShowAgenda()
    {
        Console.Clear();

        DateTime viewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        while (true)
        {
            DrawCalendar(viewMonth);

            Console.WriteLine("\nN = next month | P = previous month | enter date (YYYY-MM-DD) | 0 = back");
            Console.Write("Choice: ");

            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input) || input == "0")
            {
                return;
            }

            if (input.ToUpper() == "N")
            {
                viewMonth = viewMonth.AddMonths(1);
                continue;
            }

            if (input.ToUpper() == "P")
            {
                viewMonth = viewMonth.AddMonths(-1);
                continue;
            }

            if (DateTime.TryParse(input, out DateTime selectedDate))
            {
                ShowAppointmentsForDate(selectedDate);
            }
            else
            {
                Console.WriteLine("Invalid input.");
                Console.ReadKey();
            }
        }
    }

    private static void DrawCalendar(DateTime month)
    {
        Console.Clear();

        List<string> reservedDates = reservationAccess.GetReservedDatesForMonth(month.ToString("yyyy-MM"));

        Console.WriteLine($"\n========== {month:MMMM yyyy} ==========");
        Console.WriteLine(" Mo   Tu   We   Th   Fr   Sa   Su");

        int startDay = (int)new DateTime(month.Year, month.Month, 1).DayOfWeek;

        if (startDay == 0)
        {
            startDay = 7;
        }

        int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);

        Console.Write(new string(' ', (startDay - 1) * 5));

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime date = new DateTime(month.Year, month.Month, day);
            string dateStr = date.ToString("yyyy-MM-dd");

            bool hasReservation = reservedDates.Contains(dateStr);

            if (hasReservation)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            Console.Write($"{day,3}  ");
            Console.ResetColor();

            int col = ((startDay - 1) + (day - 1)) % 7;

            if (col == 6)
            {
                Console.WriteLine();
            }
        }

        Console.WriteLine("\n");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[Yellow=Has appointments]");
        Console.ResetColor();

        Console.WriteLine();
    }

    private static void ShowAppointmentsForDate(DateTime date)
    {
        Console.Clear();

        List<ReservationModel> appointments = reservationAccess.GetReservationsForDate(date.ToString("yyyy-MM-dd"));

        Console.WriteLine($"\n-- Appointments on {date:dddd dd MMMM yyyy} --");

        if (appointments.Count == 0)
        {
            Console.WriteLine("No appointments on this date.");
            Console.WriteLine("\nPress any key to go back...");
            Console.ReadKey();
            return;
        }

        foreach (ReservationModel r in appointments)
        {
            Console.ForegroundColor = r.Status == "gepland" ? ConsoleColor.Cyan : ConsoleColor.DarkGray;

            string caregiver = string.IsNullOrEmpty(r.DoctorName) ? "-" : r.DoctorName;

            Console.WriteLine($"  {r.Time} | {r.Type,-15} | Room: {r.RoomNumber,-10} | Patient: {r.PatientName,-20} | Caregiver: {caregiver,-20} | Status: {r.Status}");

            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key to go back...");
        Console.ReadKey();
    }

    private static void CreateAppointment()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n-- Nieuwe afspraak aanmaken --");

            DateTime viewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime selectedDate;

            while (true)
            {
                DrawCalendar(viewMonth);

                Console.WriteLine("\nN = volgende maand | P = vorige maand | datum invoeren (YYYY-MM-DD) om te selecteren");
                Console.Write("Datum: ");

                string? dateInput = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(dateInput))
                {
                    Console.WriteLine("Geannuleerd.");
                    return;
                }

                if (dateInput.ToUpper() == "N")
                {
                    viewMonth = viewMonth.AddMonths(1);
                    continue;
                }

                if (dateInput.ToUpper() == "P")
                {
                    viewMonth = viewMonth.AddMonths(-1);
                    continue;
                }

                if (!DateTime.TryParse(dateInput, out selectedDate) || selectedDate.Date < DateTime.Today)
                {
                    Console.WriteLine("Enter a valid future date (YYYY-MM-DD).");
                    Console.ReadKey();
                    continue;
                }

                break;
            }

            (string time, RoomModel room)? picked = ShowDayGridAndPick(selectedDate);

            if (picked == null)
            {
                continue;
            }

            string selectedTime = picked.Value.time;
            RoomModel selectedRoom = picked.Value.room;
            string dateTimeStr = $"{selectedDate:yyyy-MM-dd} {selectedTime}";

            string appointmentType = PickAppointmentType();

            if (string.IsNullOrWhiteSpace(appointmentType))
            {
                Console.WriteLine("Geen type gekozen.");
                Console.ReadKey();
                return;
            }

            List<UserModel> availableDoctors = userAccess.GetAvailableDoctors(dateTimeStr);
            long? specialistId = null;

            if (availableDoctors.Count > 0)
            {
                Console.WriteLine($"\nBeschikbare hulpverleners op {selectedDate:dd-MM-yyyy} om {selectedTime}.");
                Console.WriteLine("Druk op Enter om over te slaan.");

                for (int i = 0; i < availableDoctors.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {availableDoctors[i].FullName} ({availableDoctors[i].Specialty})");
                }

                Console.Write("Keuze: ");
                string? docInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(docInput)
                    && int.TryParse(docInput, out int docChoice)
                    && docChoice >= 1
                    && docChoice <= availableDoctors.Count)
                {
                    specialistId = availableDoctors[docChoice - 1].Id;
                }
            }
            else
            {
                Console.WriteLine("Geen hulpverleners beschikbaar op dit tijdstip.");
            }

            List<UserModel> patients = userAccess.GetAllByRole("ouder");

            if (patients.Count == 0)
            {
                Console.WriteLine("Geen patienten gevonden.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nSelecteer patient:");

            for (int i = 0; i < patients.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {patients[i].FullName} ({patients[i].Email})");
            }

            if (!TryPickIndex(patients.Count, out int patientIdx))
            {
                return;
            }

            UserModel selectedPatient = patients[patientIdx];

            string doctorName = specialistId.HasValue
                ? availableDoctors.FirstOrDefault(d => d.Id == specialistId)?.FullName ?? "-"
                : "-";

            Console.WriteLine("\n-- Bevestig afspraak --");
            Console.WriteLine($"  Datum:        {selectedDate:dddd dd MMMM yyyy}");
            Console.WriteLine($"  Tijd:         {selectedTime}");
            Console.WriteLine($"  Type:         {appointmentType}");
            Console.WriteLine($"  Kamer:        {selectedRoom.Name} ({selectedRoom.Type})");
            Console.WriteLine($"  Hulpverlener: {doctorName}");
            Console.WriteLine($"  Patient:      {selectedPatient.FullName}");

            Console.Write("\nBevestigen? (J/N): ");
            string? confirm = Console.ReadLine();

            if (confirm?.Trim().ToUpper() != "J")
            {
                Console.WriteLine("Geannuleerd.");
                Console.ReadKey();
                return;
            }

            reservationAccess.CreateReservation(selectedPatient.Id, selectedRoom.Id, specialistId, dateTimeStr, appointmentType);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nAfspraak aangemaakt voor {selectedPatient.FullName} op {selectedDate:dd-MM-yyyy} om {selectedTime} in kamer {selectedRoom.Name}.");
            Console.ResetColor();

            Console.WriteLine("Druk op een toets...");
            Console.ReadKey();
            return;
        }
    }

    private static (string time, RoomModel room)? ShowDayGridAndPick(DateTime date)
    {
        string[] slots = GenerateTimeSlots();

        List<RoomModel> rooms = roomAccess.GetAllRooms();
        List<ReservationModel> dayReservations = reservationAccess.GetReservationsForDate(date.ToString("yyyy-MM-dd"));

        while (true)
        {
            Console.Clear();

            Console.WriteLine($"\n-- Dagplanning {date:dddd dd MMMM yyyy} --");
            Console.WriteLine();

            Console.Write($"{"Tijd",-7}");

            foreach (RoomModel r in rooms)
            {
                Console.Write($"| {r.Name,-12}");
            }

            Console.WriteLine("|");
            Console.WriteLine(new string('-', 7 + rooms.Count * 15));

            foreach (string slot in slots)
            {
                Console.Write($"{slot,-7}");

                foreach (RoomModel room in rooms)
                {
                    DateTime slotDt = DateTime.Parse($"{date:yyyy-MM-dd} {slot}");

                    ReservationModel? res = dayReservations.FirstOrDefault(r =>
                        r.RoomId == room.Id
                        && DateTime.TryParse($"{date:yyyy-MM-dd} {r.Time}", out DateTime start)
                        && slotDt >= start
                        && slotDt < start.AddMinutes(30));

                    if (res != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        string label = res.Type?.Length > 9
                            ? res.Type[..9]
                            : res.Type ?? "BEZET";

                        Console.Write($"| {label,-12}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"| {"VRIJ",-12}");
                    }

                    Console.ResetColor();
                }

                Console.WriteLine("|");
            }

            bool hasFreeSlot = false;

            foreach (string slotToCheck in slots)
            {
                string checkDateTimeStr = $"{date:yyyy-MM-dd} {slotToCheck}";
                List<RoomModel> freeRooms = roomAccess.GetAvailableRooms(checkDateTimeStr);

                if (freeRooms.Count > 0)
                {
                    hasFreeSlot = true;
                    break;
                }
            }

            if (!hasFreeSlot)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAlle tijden op deze datum zitten vol.");
                Console.ResetColor();

                Console.WriteLine("0 = terug naar datum kiezen");
                Console.Write("Keuze: ");

                string? backInput = Console.ReadLine()?.Trim();

                if (backInput == "0")
                {
                    return null;
                }

                continue;
            }
            Console.WriteLine();
            Console.WriteLine("Tijd invoeren, bijvoorbeeld 09:30 | 0 = terug");
            Console.Write("Tijd: ");

            string? timeInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(timeInput) || timeInput == "0")
            {
                return null;
            }

            if (!slots.Contains(timeInput))
            {
                Console.WriteLine("Ongeldig tijdstip.");
                Console.ReadKey();
                continue;
            }

            Console.Write("Kamer: ");
            string? roomInput = Console.ReadLine()?.Trim();

            RoomModel? chosenRoom = rooms.FirstOrDefault(r =>
                r.Name.Equals(roomInput, StringComparison.OrdinalIgnoreCase));

            if (chosenRoom == null)
            {
                Console.WriteLine("Kamer niet gevonden.");
                Console.ReadKey();
                continue;
            }

            string dateTimeStr = $"{date:yyyy-MM-dd} {timeInput}";
            List<RoomModel> freeRooms = roomAccess.GetAvailableRooms(dateTimeStr);

            if (!freeRooms.Any(r => r.Id == chosenRoom.Id))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Dit slot is bezet. Kies een ander slot.");
                Console.ResetColor();

                Console.ReadKey();
                continue;
            }

            return (timeInput, chosenRoom);
        }
    }

    private static string PickAppointmentType()
    {
        Console.Clear();

        List<TemplateModel> templates = templateAccess.GetAll();

        if (templates.Count > 0)
        {
            Console.WriteLine("\nWil je een template gebruiken? (Y/N)");
            string? useTemplate = Console.ReadLine()?.Trim().ToUpper();

            if (useTemplate == "Y")
            {
                Console.WriteLine("\nAvailable templates:");

                for (int i = 0; i < templates.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {templates[i].Name} (type: {templates[i].Type})");

                    if (!string.IsNullOrWhiteSpace(templates[i].Notes))
                    {
                        Console.WriteLine($"       -> {templates[i].Notes}");
                    }
                }

                Console.Write("Choice: ");
                string? tInput = Console.ReadLine();

                if (int.TryParse(tInput, out int tChoice)
                    && tChoice >= 1
                    && tChoice <= templates.Count)
                {
                    TemplateModel picked = templates[tChoice - 1];

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Template '{picked.Name}' selected, type: {picked.Type}");
                    Console.ResetColor();

                    return picked.Type;
                }

                Console.WriteLine("Invalid choice, selecting manually.");
            }
        }

        string[] types = { "Checkup", "Consultation", "Surgery", "Emergency", "General" };

        Console.WriteLine("\nSelect appointment type:");

        for (int i = 0; i < types.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {types[i]}");
        }

        if (!TryPickIndex(types.Length, out int typeIdx))
        {
            return "";
        }

        return types[typeIdx];
    }

    private static string[] GenerateTimeSlots()
    {
        List<string> slots = new List<string>();

        for (int h = 8; h <= 17; h++)
        {
            slots.Add($"{h:D2}:00");

            if (h < 17)
            {
                slots.Add($"{h:D2}:30");
            }
        }

        return slots.ToArray();
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