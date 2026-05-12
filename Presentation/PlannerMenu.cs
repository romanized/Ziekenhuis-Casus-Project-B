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
        DateTime now = DateTime.Now;
        Console.WriteLine($"\n-- Kamerstatus op {now:dd-MM-yyyy HH:mm} --");
        List<RoomModel> rooms = roomAccess.GetAllRooms();
        List<ReservationModel> todays = reservationAccess.GetReservationsForDate(now.ToString("yyyy-MM-dd"));

        foreach (RoomModel room in rooms)
        {
            List<ReservationModel> inRoom = todays
                .Where(r => r.RoomId == room.Id)
                .OrderBy(r => r.Time)
                .ToList();

            Console.Write($"Kamer {room.Name} ({room.Type}, {room.Location}) - ");

            ReservationModel? current = inRoom.FirstOrDefault(r =>
            {
                if (!DateTime.TryParse($"{now:yyyy-MM-dd} {r.Time}", out DateTime start)) return false;
                return now >= start && now < start.AddMinutes(30);
            });

            ReservationModel? next = inRoom
                .Where(r => DateTime.TryParse($"{now:yyyy-MM-dd} {r.Time}", out DateTime s) && s > now)
                .OrderBy(r => r.Time)
                .FirstOrDefault();

            if (current != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("NU BEZET");
                Console.ResetColor();
                Console.WriteLine($" — {current.Time} | {current.Type} | {current.PatientName}");
            }
            else if (next != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("VRIJ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($" (volgende: {next.Time} — {next.Type} | {next.PatientName})");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("VRIJ (geen afspraken meer vandaag)");
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
            string hulpverlener = string.IsNullOrEmpty(r.DoctorName) ? "-" : r.DoctorName;
            Console.WriteLine($"  {r.Time} | {r.Type,-15} | Kamer: {r.RoomNumber,-10} | Patient: {r.PatientName,-20} | Hulpverlener: {hulpverlener,-20} | Status: {r.Status}");
            Console.ResetColor();
        }
    }

    private static void CreateAppointment()
    {
        DateTime viewMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime selectedDate;

        while (true)
        {
            DrawCalendar(viewMonth);
            Console.WriteLine("\nN = volgende maand | P = vorige maand | datum (YYYY-MM-DD) | 0 = terug");
            Console.Write("Datum: ");
            string? dateInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(dateInput) || dateInput == "0") return;
            if (dateInput.ToUpper() == "N") { viewMonth = viewMonth.AddMonths(1); continue; }
            if (dateInput.ToUpper() == "P") { viewMonth = viewMonth.AddMonths(-1); continue; }

            if (!DateTime.TryParse(dateInput, out selectedDate) || selectedDate.Date < DateTime.Today)
            {
                Console.WriteLine("Voer een geldige toekomstige datum in (YYYY-MM-DD).");
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
        Console.WriteLine("\nType afspraak:");
        for (int i = 0; i < types.Length; i++)
            Console.WriteLine($"  {i + 1}. {types[i]}");
        if (!TryPickIndex(types.Length, out int typeIdx)) return;
        string appointmentType = types[typeIdx];

        List<UserModel> availableDoctors = userAccess.GetAvailableDoctors(dateTimeStr);
        long? specialistId = null;

        if (availableDoctors.Count > 0)
        {
            Console.WriteLine($"\nHulpverlener (Enter om over te slaan):");
            for (int i = 0; i < availableDoctors.Count; i++)
                Console.WriteLine($"  {i + 1}. {availableDoctors[i].FullName} ({availableDoctors[i].Specialty})");
            Console.Write("Keuze: ");
            string? docInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(docInput) && int.TryParse(docInput, out int docChoice)
                && docChoice >= 1 && docChoice <= availableDoctors.Count)
                specialistId = availableDoctors[docChoice - 1].Id;
        }

        List<UserModel> patients = userAccess.GetAllByRole("ouder");
        if (patients.Count == 0) { Console.WriteLine("Geen patienten gevonden."); return; }

        Console.WriteLine("\nPatient:");
        for (int i = 0; i < patients.Count; i++)
            Console.WriteLine($"  {i + 1}. {patients[i].FullName} ({patients[i].Email})");
        if (!TryPickIndex(patients.Count, out int patientIdx)) return;
        UserModel selectedPatient = patients[patientIdx];

        string doctorName = specialistId.HasValue
            ? availableDoctors.FirstOrDefault(d => d.Id == specialistId)?.FullName ?? "-"
            : "-";

        Console.WriteLine($"\n-- Bevestig --");
        Console.WriteLine($"  {selectedDate:dd-MM-yyyy} {selectedTime}  |  Kamer: {selectedRoom.Name}  |  {appointmentType}  |  {doctorName}  |  {selectedPatient.FullName}");
        Console.Write("Bevestigen? (J/N): ");
        if (Console.ReadLine()?.Trim().ToUpper() != "J") { Console.WriteLine("Geannuleerd."); return; }

        reservationAccess.CreateReservation(selectedPatient.Id, selectedRoom.Id, specialistId, dateTimeStr, appointmentType);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nAfspraak aangemaakt: {selectedPatient.FullName} — {selectedDate:dd-MM-yyyy} {selectedTime} — Kamer {selectedRoom.Name}.");
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
