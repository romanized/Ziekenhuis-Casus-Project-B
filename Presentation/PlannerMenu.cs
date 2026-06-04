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
        Console.WriteLine("\n-- Nieuwe afspraak aanmaken --");

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
                Console.WriteLine("Enter a valid future date (YYYY-MM-DD).");
                continue;
            }
            break;
        }

        (string time, RoomModel room)? picked = ShowDayGridAndPick(selectedDate);
        if (picked == null) return;

        string selectedTime = picked.Value.time;
        RoomModel selectedRoom = picked.Value.room;
        string dateTimeStr = $"{selectedDate:yyyy-MM-dd} {selectedTime}";

        string[] types = { "Controle", "Consult", "Operatie", "Spoedgeval", "Algemeen" };
        Console.WriteLine("\nStap 2 - Selecteer type afspraak:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");
        if (!TryPickIndex(types.Length, out int typeIdx)) return;
        string appointmentType = types[typeIdx];

        string[] slots = GenerateTimeSlots();
        Console.WriteLine("\nStap 3 - Selecteer tijdstip:");
        for (int i = 0; i < slots.Length; i++)
            Console.WriteLine($"  {i + 1}. {slots[i]}");
        if (!TryPickIndex(slots.Length, out int slotIdx)) return;
        selectedTime = slots[slotIdx];
        dateTimeStr = $"{selectedDate:yyyy-MM-dd} {selectedTime}";

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
        selectedRoom = availableRooms[roomIdx];

        List<UserModel> availableDoctors = userAccess.GetAvailableDoctors(dateTimeStr);
        long? specialistId = null;

        if (availableDoctors.Count > 0)
        {
            Console.WriteLine($"\nStap 4 - Beschikbare hulpverleners op {selectedDate:dd-MM-yyyy} om {selectedTime} (Enter om over te slaan):");

            for (int i = 0; i < availableDoctors.Count; i++)
                Console.WriteLine($"  {i + 1}. {availableDoctors[i].FullName} ({availableDoctors[i].Specialty})");
        }

        Console.Write("Keuze: ");
        string? docInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(docInput) && int.TryParse(docInput, out int docChoice)
            && docChoice >= 1 && docChoice <= availableDoctors.Count)
            specialistId = availableDoctors[docChoice - 1].Id;

        Console.WriteLine("");

        // Step 5 - patient hotbar search
        List<UserModel> patients = userAccess.GetAllByRole("ouder");
        if (patients.Count == 0) { Console.WriteLine("Geen patienten gevonden."); return; }

        Console.WriteLine("\nStap 5 - Selecteer patient (hotbar zoeken):");
        Console.WriteLine("Typ een naam (voornaam, achternaam of volledige naam). Spaties worden genegeerd.");
        Console.WriteLine("Resultaten worden alfabetisch oplopend getoond.");
        Console.WriteLine("Druk op Enter zonder tekst om te annuleren.");

        UserModel selectedPatient;
        while (true)
        {
            Console.Write("\nZoek: ");
            string? queryRaw = Console.ReadLine();
            if (queryRaw == null) return;
            if (string.IsNullOrWhiteSpace(queryRaw)) return;

            string q = NormalizeHotbarQuery(queryRaw);

            List<UserModel> matches = patients
                .Where(p =>
                {
                    var parts = SplitNameParts(p.FullName);
                    string firstNorm = NormalizeHotbarQuery(parts.first);
                    string lastNorm = NormalizeHotbarQuery(parts.last);
                    string fullNorm = NormalizeHotbarQuery(p.FullName);

                    return fullNorm.Contains(q) || firstNorm.Contains(q) || lastNorm.Contains(q);
                })
                .OrderBy(p => p.FullName)
                .ThenBy(p => p.Id)
                .ToList();

            if (matches.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Geen patiënten gevonden. Probeer een andere zoekterm.");
                Console.ResetColor();
                continue;
            }

            for (int i = 0; i < matches.Count; i++)
                Console.WriteLine($"  {i + 1}. {matches[i].FullName} ({matches[i].Email})");

            //selection uses EXACT name from the results.
            Console.Write("\nSelecteer door exact één van de getoonde namen te typen (Enter = annuleren): ");
            string? selectionRaw = Console.ReadLine();
            if (selectionRaw == null) return;
            if (string.IsNullOrWhiteSpace(selectionRaw)) return;

            string s = selectionRaw.Trim();
            string sNorm = NormalizeHotbarQuery(s);

            selectedPatient = matches.FirstOrDefault(p =>
            {
                string patientFullNorm = NormalizeHotbarQuery(p.FullName);
                return patientFullNorm.Equals(sNorm, StringComparison.OrdinalIgnoreCase);
            });

            if (selectedPatient != null) break;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ongeldige selectie. Typ exact één van de namen uit de resultaten.");
            Console.ResetColor();
        }

        string doctorName = specialistId.HasValue
            ? availableDoctors.FirstOrDefault(d => d.Id == specialistId)?.FullName ?? "-"
            : "-";

        Console.WriteLine("\n-- Bevestig afspraak --");
        Console.WriteLine($"  Datum:    {selectedDate:dddd dd MMMM yyyy}");
        Console.WriteLine($"  Tijd:     {selectedTime}");
        Console.WriteLine($"  Type:     {appointmentType}");
        Console.WriteLine($"  Kamer:    {selectedRoom.Name} ({selectedRoom.Type})");
        Console.WriteLine($"  Hulpverlener: {doctorName}");
        Console.WriteLine($"  Patient:  {selectedPatient.FullName}");
        Console.Write("\nBevestigen? (J/N): ");
        string? confirm = Console.ReadLine();
        if (confirm?.Trim().ToUpper() != "J") { Console.WriteLine("Geannuleerd."); return; }

        reservationAccess.CreateReservation(selectedPatient.Id, selectedRoom.Id, specialistId, dateTimeStr, appointmentType);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nAfspraak aangemaakt voor {selectedPatient.FullName} op {selectedDate:dd-MM-yyyy} om {selectedTime} in kamer {selectedRoom.Name}.");
        Console.ResetColor();
        Console.WriteLine("Druk op een toets...");
        Console.ReadKey();
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
                Console.Write($"| {r.Name,-12}");
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
                        && slotDt >= start && slotDt < start.AddMinutes(30));

                    if (res != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        string label = res.Type?.Length > 9 ? res.Type[..9] : (res.Type ?? "BEZET");
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

            Console.WriteLine();
            Console.WriteLine("Tijd invoeren (bijv. 09:30) | 0 = terug");
            Console.Write("Tijd: ");
            string? timeInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(timeInput) || timeInput == "0") return null;
            if (!slots.Contains(timeInput)) { Console.WriteLine("Ongeldig tijdstip."); Console.ReadKey(); continue; }

            Console.Write("Kamer: ");
            string? roomInput = Console.ReadLine()?.Trim();
            RoomModel? chosenRoom = rooms.FirstOrDefault(r => r.Name.Equals(roomInput, StringComparison.OrdinalIgnoreCase));
            if (chosenRoom == null) { Console.WriteLine("Kamer niet gevonden."); Console.ReadKey(); continue; }

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

    private static string NormalizeHotbarQuery(string input)
    {
        if (input == null) return string.Empty;
        // Hotbar ignores whitespace
        var noSpaces = new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
        return noSpaces.ToUpperInvariant();
    }

    private static (string first, string last) SplitNameParts(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return ("", "");
        var parts = fullName
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        if (parts.Count == 1) return (parts[0], "");
        // assume first token is first name, last token is last name
        return (parts.First(), parts.Last());
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

