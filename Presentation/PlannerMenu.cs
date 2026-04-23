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
            Console.WriteLine("1. View agenda");
            Console.WriteLine("2. Create new appointment");
            Console.WriteLine("3. Kamerstatus bekijken");
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
        Console.WriteLine($"\n-- Kamerstatus op {DateTime.Today:dd-MM-yyyy} --");
        List<RoomModel> rooms = roomAccess.GetAllRooms();
        List<ReservationModel> todays = reservationAccess.GetReservationsForDate(DateTime.Today.ToString("yyyy-MM-dd"));

        foreach (RoomModel room in rooms)
        {
            List<ReservationModel> inRoom = todays.Where(r => r.RoomId == room.Id).ToList();

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
                Console.WriteLine($"BEZET: {inRoom.Count} afspraken vandaag");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (ReservationModel r in inRoom)
                    Console.WriteLine($"    {r.Time} | {r.Type} | Patient: {r.PatientName}");
                Console.ResetColor();
            }
        }

        Console.WriteLine("\nDruk op een toets om terug te gaan...");
        Console.ReadKey();
    }

    private static void ShowAgenda()
    {
        DateTime viewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        while (true)
        {
            DrawCalendar(viewMonth);
            Console.WriteLine("\nN = volgende maand | P = vorige maand | datum invoeren (YYYY-MM-DD) | 0 = terug");
            Console.Write("Keuze: ");
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input) || input == "0") return;
            if (input.ToUpper() == "N") { viewMonth = viewMonth.AddMonths(1); continue; }
            if (input.ToUpper() == "P") { viewMonth = viewMonth.AddMonths(-1); continue; }

            if (DateTime.TryParse(input, out DateTime selectedDate))
                ShowAppointmentsForDate(selectedDate);
            else
                Console.WriteLine("Ongeldige invoer.");
        }
    }

    private static void DrawCalendar(DateTime month)
    {
        List<string> reservedDates = reservationAccess.GetReservedDatesForMonth(month.ToString("yyyy-MM"));

        Console.WriteLine($"\n========== {month:MMMM yyyy} ==========");
        Console.WriteLine(" Ma   Di   Wo   Do   Vr   Za   Zo");

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
        Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("[Geel=Heeft afspraken]");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void ShowAppointmentsForDate(DateTime date)
    {
        List<ReservationModel> appointments = reservationAccess.GetReservationsForDate(date.ToString("yyyy-MM-dd"));
        Console.WriteLine($"\n-- Afspraken op {date:dddd dd MMMM yyyy} --");
        if (appointments.Count == 0) { Console.WriteLine("Geen afspraken op deze datum."); return; }

        foreach (ReservationModel r in appointments)
        {
            Console.ForegroundColor = r.Status == "gepland" ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
            string doctor = string.IsNullOrEmpty(r.DoctorName) ? "-" : r.DoctorName;
            Console.WriteLine($"  {r.Time} | {r.Type,-15} | Kamer: {r.RoomNumber,-10} | Patient: {r.PatientName,-20} | Arts: {doctor,-20} | Status: {r.Status}");
            Console.ResetColor();
        }
    }

    private static void CreateAppointment()
    {
        Console.WriteLine("\n-- Nieuwe afspraak aanmaken --");

        // Stap 1: Datum selecteren
        DateTime viewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime selectedDate;
        while (true)
        {
            DrawCalendar(viewMonth);
            Console.WriteLine("\nN = volgende maand | P = vorige maand | datum invoeren (YYYY-MM-DD) om te selecteren");
            Console.Write("Datum: ");
            string? dateInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(dateInput)) { Console.WriteLine("Geannuleerd."); return; }
            if (dateInput.ToUpper() == "N") { viewMonth = viewMonth.AddMonths(1); continue; }
            if (dateInput.ToUpper() == "P") { viewMonth = viewMonth.AddMonths(-1); continue; }

            if (!DateTime.TryParse(dateInput, out selectedDate) || selectedDate.Date < DateTime.Today)
            {
                Console.WriteLine("Voer een geldige toekomstige datum in (YYYY-MM-DD).");
                continue;
            }
            break;
        }

        // Stap 2: Type afspraak
        string[] types = { "Controle", "Consult", "Operatie", "Spoedgeval", "Algemeen" };
        Console.WriteLine("\nStap 2 - Selecteer type afspraak:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");
        if (!TryPickIndex(types.Length, out int typeIdx)) return;
        string appointmentType = types[typeIdx];

        // Stap 3: Tijdstip selecteren
        string[] slots = GenerateTimeSlots();
        Console.WriteLine("\nStap 3 - Selecteer tijdstip:");
        for (int i = 0; i < slots.Length; i++)
            Console.WriteLine($"  {i + 1}. {slots[i]}");
        if (!TryPickIndex(slots.Length, out int slotIdx)) return;
        string selectedTime = slots[slotIdx];
        string dateTimeStr = $"{selectedDate:yyyy-MM-dd} {selectedTime}";

        // Stap 4a: Beschikbare kamers
        List<RoomModel> availableRooms = roomAccess.GetAvailableRooms(dateTimeStr);
        if (availableRooms.Count == 0)
        {
            Console.WriteLine("Geen kamers beschikbaar op dit tijdstip.");
            return;
        }
        Console.WriteLine($"\nStap 4 - Beschikbare kamers op {selectedDate:dd-MM-yyyy} om {selectedTime}:");
        for (int i = 0; i < availableRooms.Count; i++)
            Console.WriteLine($"  {i + 1}. {availableRooms[i].Name} ({availableRooms[i].Type}, {availableRooms[i].Location})");
        if (!TryPickIndex(availableRooms.Count, out int roomIdx)) return;
        RoomModel selectedRoom = availableRooms[roomIdx];

        // Stap 4b: Beschikbare artsen
        List<UserModel> availableDoctors = userAccess.GetAvailableDoctors(dateTimeStr);
        long? specialistId = null;
        if (availableDoctors.Count > 0)
        {
            Console.WriteLine($"\nStap 4 - Beschikbare artsen op {selectedDate:dd-MM-yyyy} om {selectedTime} (Enter om over te slaan):");
            for (int i = 0; i < availableDoctors.Count; i++)
                Console.WriteLine($"  {i + 1}. {availableDoctors[i].FullName} ({availableDoctors[i].Specialty})");
            Console.Write("Keuze: ");
            string? docInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(docInput) && int.TryParse(docInput, out int docChoice)
                && docChoice >= 1 && docChoice <= availableDoctors.Count)
                specialistId = availableDoctors[docChoice - 1].Id;
        }
        else
        {
            Console.WriteLine("Geen artsen beschikbaar op dit tijdstip.");
        }

        // Stap 5: Patient selecteren
        List<UserModel> patients = userAccess.GetAllByRole("ouder");
        if (patients.Count == 0) { Console.WriteLine("Geen patienten gevonden."); return; }
        Console.WriteLine("\nStap 5 - Selecteer patient:");
        for (int i = 0; i < patients.Count; i++)
            Console.WriteLine($"  {i + 1}. {patients[i].FullName} ({patients[i].Email})");
        if (!TryPickIndex(patients.Count, out int patientIdx)) return;
        UserModel selectedPatient = patients[patientIdx];

        // Bevestiging
        string doctorName = specialistId.HasValue
            ? availableDoctors.FirstOrDefault(d => d.Id == specialistId)?.FullName ?? "-"
            : "-";

        Console.WriteLine("\n-- Bevestig afspraak --");
        Console.WriteLine($"  Datum:    {selectedDate:dddd dd MMMM yyyy}");
        Console.WriteLine($"  Tijd:     {selectedTime}");
        Console.WriteLine($"  Type:     {appointmentType}");
        Console.WriteLine($"  Kamer:    {selectedRoom.Name} ({selectedRoom.Type})");
        Console.WriteLine($"  Arts:     {doctorName}");
        Console.WriteLine($"  Patient:  {selectedPatient.FullName}");
        Console.Write("\nBevestigen? (J/N): ");
        string? confirm = Console.ReadLine();
        if (confirm?.Trim().ToUpper() != "J") { Console.WriteLine("Geannuleerd."); return; }

        reservationAccess.CreateReservation(selectedPatient.Id, selectedRoom.Id, specialistId, dateTimeStr, appointmentType);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nAfspraak aangemaakt voor {selectedPatient.FullName} op {selectedDate:dd-MM-yyyy} om {selectedTime} in kamer {selectedRoom.Name}.");
        Console.ResetColor();
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
        Console.Write("Keuze: ");
        string? input = Console.ReadLine();
        if (int.TryParse(input, out int choice) && choice >= 1 && choice <= count)
        {
            index = choice - 1;
            return true;
        }
        Console.WriteLine("Ongeldige keuze.");
        index = -1;
        return false;
    }
}
