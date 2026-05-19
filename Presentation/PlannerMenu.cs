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
            if (string.IsNullOrWhiteSpace(input)) { Console.WriteLine("Please enter a valid option."); continue; }

            switch (input.Trim())
            {
                case "1": ShowAgenda(); break;
                case "2": CreateAppointment(); break;
                case "3": ShowRoomStatus(); break;
                case "0": running = false; break;
                default: Console.WriteLine("Invalid input."); break;
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
            Console.WriteLine($"  {i + 1}. {slots[i]}");
        if (!TryPickIndex(slots.Length, out int slotIdx)) return;
        string selectedTime = slots[slotIdx];

        Console.WriteLine($"\n-- Room status on {date:dd-MM-yyyy} at {selectedTime} --");
        List<RoomModel> rooms = roomAccess.GetAllRooms();
        List<ReservationModel> todays = reservationAccess.GetReservationsForDate(date.ToString("yyyy-MM-dd"));

        foreach (RoomModel room in rooms)
        {
            ReservationModel? atSlot = todays.FirstOrDefault(r => r.RoomId == room.Id && r.Time == selectedTime);

            Console.Write($"Room {room.Name} ({room.Type}, {room.Location}) - Status: ");
            if (atSlot == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("FREE");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"OCCUPIED ({atSlot.Type}, patient: {atSlot.PatientName})");
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

            if (string.IsNullOrEmpty(input) || input == "0") return;
            if (input.ToUpper() == "N") { viewMonth = viewMonth.AddMonths(1); continue; }
            if (input.ToUpper() == "P") { viewMonth = viewMonth.AddMonths(-1); continue; }

            if (DateTime.TryParse(input, out DateTime selectedDate))
                ShowAppointmentsForDate(selectedDate);
            else
                Console.WriteLine("Invalid input.");
        }
    }

    private static void DrawCalendar(DateTime month)
    {
        Console.Clear();
        List<string> reservedDates = reservationAccess.GetReservedDatesForMonth(month.ToString("yyyy-MM"));

        Console.WriteLine($"\n========== {month:MMMM yyyy} ==========");
        Console.WriteLine(" Mo   Tu   We   Th   Fr   Sa   Su");

        int startDay = (int)new DateTime(month.Year, month.Month, 1).DayOfWeek;
        if (startDay == 0) startDay = 7;
        int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);

        Console.Write(new string(' ', (startDay - 1) * 5));

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime date = new DateTime(month.Year, month.Month, day);
            string dateStr = date.ToString("yyyy-MM-dd");
            bool hasReservation = reservedDates.Contains(dateStr);

            if (hasReservation)
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Write($"{day,3}  ");
            Console.ResetColor();

            int col = ((startDay - 1) + (day - 1)) % 7;
            if (col == 6) Console.WriteLine();
        }

        Console.WriteLine("\n");
        Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("[Yellow=Has appointments]");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void ShowAppointmentsForDate(DateTime date)
    {
        Console.Clear();
        List<ReservationModel> appointments = reservationAccess.GetReservationsForDate(date.ToString("yyyy-MM-dd"));
        Console.WriteLine($"\n-- Appointments on {date:dddd dd MMMM yyyy} --");
        if (appointments.Count == 0) { Console.WriteLine("No appointments on this date."); return; }

        foreach (ReservationModel r in appointments)
        {
            Console.ForegroundColor = r.Status == "gepland" ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
            string caregiver = string.IsNullOrEmpty(r.DoctorName) ? "-" : r.DoctorName;
            Console.WriteLine($"  {r.Time} | {r.Type,-15} | Room: {r.RoomNumber,-10} | Patient: {r.PatientName,-20} | Caregiver: {caregiver,-20} | Status: {r.Status}");
            Console.ResetColor();
        }
    }

    private static void CreateAppointment()
    {
        Console.Clear();
        Console.WriteLine("\n-- Create new appointment --");

        DateTime viewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime selectedDate;
        while (true)
        {
            DrawCalendar(viewMonth);
            Console.WriteLine("\nN = next month | P = previous month | enter date (YYYY-MM-DD) to select");
            Console.Write("Date: ");
            string? dateInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(dateInput)) { Console.WriteLine("Cancelled."); return; }
            if (dateInput.ToUpper() == "N") { viewMonth = viewMonth.AddMonths(1); continue; }
            if (dateInput.ToUpper() == "P") { viewMonth = viewMonth.AddMonths(-1); continue; }

            if (!DateTime.TryParse(dateInput, out selectedDate) || selectedDate.Date < DateTime.Today)
            {
                Console.WriteLine("Enter a valid future date (YYYY-MM-DD).");
                continue;
            }
            break;
        }

        List<UserModel> patients = userAccess.GetAllByRole("ouder");
        if (patients.Count == 0) { Console.WriteLine("No patients found."); return; }

        // oldest patients at the top so the planner doesn't have to scroll
        patients = patients
            .OrderBy(p => DateTime.TryParse(p.BirthDate, out DateTime bd) ? bd : DateTime.MaxValue)
            .ToList();

        Console.WriteLine("\nStep 2 - Select patient (oldest first):");
        for (int i = 0; i < patients.Count; i++)
        {
            string age = DateTime.TryParse(patients[i].BirthDate, out DateTime bd)
                ? $"{(int)((DateTime.Today - bd).TotalDays / 365.25)} years"
                : "age unknown";
            Console.WriteLine($"  {i + 1}. {patients[i].FullName} ({age}) - {patients[i].Email}");
        }
        if (!TryPickIndex(patients.Count, out int patientIdx)) return;
        UserModel selectedPatient = patients[patientIdx];

        string appointmentType = PickAppointmentType();
        if (appointmentType == "") return;

        string[] slots = GenerateTimeSlots();
        Console.WriteLine("\nStep 4 - Select time slot:");
        for (int i = 0; i < slots.Length; i++)
            Console.WriteLine($"  {i + 1}. {slots[i]}");
        if (!TryPickIndex(slots.Length, out int slotIdx)) return;
        string selectedTime = slots[slotIdx];
        string dateTimeStr = $"{selectedDate:yyyy-MM-dd} {selectedTime}";

        List<RoomModel> availableRooms = roomAccess.GetAvailableRooms(dateTimeStr);
        if (availableRooms.Count == 0)
        {
            Console.WriteLine("No rooms available at this time.");
            return;
        }
        Console.WriteLine($"\nStep 5 - Available rooms on {selectedDate:dd-MM-yyyy} at {selectedTime}:");
        for (int i = 0; i < availableRooms.Count; i++)
            Console.WriteLine($"  {i + 1}. {availableRooms[i].Name} ({availableRooms[i].Type}, {availableRooms[i].Location})");
        if (!TryPickIndex(availableRooms.Count, out int roomIdx)) return;
        RoomModel selectedRoom = availableRooms[roomIdx];

        List<UserModel> availableDoctors = userAccess.GetAvailableDoctors(dateTimeStr);
        long? specialistId = null;
        if (availableDoctors.Count > 0)
        {
            Console.WriteLine($"\nStep 6 - Available caregivers on {selectedDate:dd-MM-yyyy} at {selectedTime} (Enter to skip):");

            for (int i = 0; i < availableDoctors.Count; i++)
                Console.WriteLine($"  {i + 1}. {availableDoctors[i].FullName} ({availableDoctors[i].Specialty})");

            Console.Write("Choice: ");
            string? docInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(docInput) && int.TryParse(docInput, out int docChoice)
                && docChoice >= 1 && docChoice <= availableDoctors.Count)
                specialistId = availableDoctors[docChoice - 1].Id;
        }
        else
        {
            Console.WriteLine("No caregivers available at this time.");
        }

        string doctorName = specialistId.HasValue
            ? availableDoctors.FirstOrDefault(d => d.Id == specialistId)?.FullName ?? "-"
            : "-";

        Console.WriteLine("\n-- Confirm appointment --");
        Console.WriteLine($"  Date:      {selectedDate:dddd dd MMMM yyyy}");
        Console.WriteLine($"  Time:      {selectedTime}");
        Console.WriteLine($"  Type:      {appointmentType}");
        Console.WriteLine($"  Room:      {selectedRoom.Name} ({selectedRoom.Type})");
        Console.WriteLine($"  Caregiver: {doctorName}");
        Console.WriteLine($"  Patient:   {selectedPatient.FullName}");
        Console.Write("\nConfirm? (Y/N): ");
        string? confirm = Console.ReadLine();
        if (confirm?.Trim().ToUpper() != "Y") { Console.WriteLine("Cancelled."); return; }

        reservationAccess.CreateReservation(selectedPatient.Id, selectedRoom.Id, specialistId, dateTimeStr, appointmentType);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nAppointment created for {selectedPatient.FullName} on {selectedDate:dd-MM-yyyy} at {selectedTime} in room {selectedRoom.Name}.");
        Console.ResetColor();
    }

    // if templates exist, planner gets the option to pick one first, otherwise it's manual
    private static string PickAppointmentType()
    {
        Console.Clear();
        List<TemplateModel> templates = templateAccess.GetAll();

        if (templates.Count > 0)
        {
            Console.WriteLine("\nStep 3 - Do you want to use a template? (Y/N)");
            string? useTemplate = Console.ReadLine()?.Trim().ToUpper();

            if (useTemplate == "Y")
            {
                Console.WriteLine("\nAvailable templates:");
                for (int i = 0; i < templates.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {templates[i].Name} (type: {templates[i].Type})");
                    if (!string.IsNullOrWhiteSpace(templates[i].Notes))
                        Console.WriteLine($"       -> {templates[i].Notes}");
                }

                Console.Write("Choice: ");
                string? tInput = Console.ReadLine();
                if (int.TryParse(tInput, out int tChoice) && tChoice >= 1 && tChoice <= templates.Count)
                {
                    TemplateModel picked = templates[tChoice - 1];
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Template '{picked.Name}' selected, type: {picked.Type}");
                    Console.ResetColor();
                    // type comes from the template, step 3 done
                    return picked.Type;
                }

                Console.WriteLine("Invalid choice, selecting manually.");
            }
        }

        string[] types = { "Checkup", "Consultation", "Surgery", "Emergency", "General" };
        Console.WriteLine("\nStep 3 - Select appointment type:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");
        if (!TryPickIndex(types.Length, out int typeIdx)) return "";
        return types[typeIdx];
    }

    private static string[] GenerateTimeSlots()
    {
        List<string> slots = new List<string>();
        for (int h = 8; h <= 17; h++)
        {
            slots.Add($"{h:D2}:00");
            if (h < 17) slots.Add($"{h:D2}:30");
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
