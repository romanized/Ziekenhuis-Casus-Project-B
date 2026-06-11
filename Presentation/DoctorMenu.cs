using System.Net.Mail;

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

            Console.WriteLine("\n==== Zorgverlener Hoofdmenu ====");
            Console.WriteLine($"Ingelogd als: {doctor.FullName}" +
                              (string.IsNullOrWhiteSpace(doctor.Specialty) ? "" : $" ({doctor.Specialty})"));

            Console.WriteLine("1. Mijn volgende afspraak bekijken");
            Console.WriteLine("2. Al mijn afspraken bekijken");
            Console.WriteLine("3. Dagagenda bekijken");
            Console.WriteLine("0. Uitloggen");

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Voer een geldige optie in.");
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
                    Console.WriteLine("Ongeldige invoer.");
                    break;
            }
        }

        UserLogic.Logout();
        Console.WriteLine("\nDruk op een toets om door te gaan...");
        Console.ReadKey();
    }

    private static void ShowNextAppointment(UserModel doctor)
    {
        Console.Clear();
        ReservationModel? next = doctorAccess.GetNextActiveReservationByDoctorId(doctor.Id);

        Console.WriteLine("\n-- Volgende afspraak --");

        if (next == null)
        {
            Console.WriteLine("Je hebt geen aankomende afspraken.");
            Console.WriteLine("\nDruk op een toets...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Datum: " + next.Date);
        Console.WriteLine("Tijd: " + next.Time);
        Console.WriteLine("Kamer: " + next.RoomNumber);

        Console.WriteLine("\nDruk op een toets...");
        Console.ReadKey();
    }

    private static void ShowAllAppointments(UserModel doctor)
    {
        Console.Clear();
        List<ReservationModel> list = doctorAccess.GetAllReservationsByDoctorId(doctor.Id);

        Console.WriteLine("\n-- Alle afspraken --");

        if (list.Count == 0)
        {
            Console.WriteLine("Je hebt geen afspraken.");
            Console.WriteLine("\nDruk op een toets...");
            Console.ReadKey();
            return;
        }

        foreach (ReservationModel r in list)
        {
            Console.WriteLine($"{r.Date} {r.Time} | Kamer {r.RoomNumber}");
        }

        Console.WriteLine("\nDruk op een toets...");
        Console.ReadKey();
    }

    public static void ShowAgenda(UserModel doctor)
    {
        Console.Clear();

        DateTime date = DateTime.Now;

        while (true)
        {
            List<ReservationModel> list =
                doctorAccess.GetAllReservationsByDoctorIdByDate(doctor.Id, date);

            AgendaTemplate(doctor, date, list);

            Console.WriteLine("\nN = Volgende dag | P = Vorige dag | Type nummer om afspraak te bekijken 1-19 | X = Exit");
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

                            Console.WriteLine("=== Afspraak details ===");

                            Console.WriteLine($"Datum: {selectedAppointment.Date}");
                            Console.WriteLine($"Tijd: {selectedAppointment.Time}");
                            Console.WriteLine($"Kamer: {selectedAppointment.RoomNumber}");

                            string patientName = userAccess.GetFullNameById(selectedAppointment.UserId);

                            if (patientName == null)
                            {
                                patientName = "Onbekend";
                            }

                            Console.WriteLine($"Patiënt: {patientName}");

                            Console.WriteLine("\nDruk op een toets...");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Geen afspraak in dit tijdslot.");
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

        Console.Write("\n-- Agenda voor ");

        if (date.Date == DateTime.Now.Date)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{date:yyyy-MM-dd} Vandaag");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{date:yyyy-MM-dd}");
            Console.ResetColor();
        }

        Console.WriteLine(" --");

        DateTime time = date.Date.AddHours(8);

        DateTime now = DateTime.Now;

        int minutes = now.Minute;
        int removeMinutes;

        if (minutes < 30)
        {
            removeMinutes = minutes;
        }
        else
        {
            removeMinutes = minutes - 30;
        }

        DateTime nextSlot = now.AddMinutes(-removeMinutes);

        for (int i = 0; i < 19; i++)
        {
            bool found = false;
            ReservationModel appointment = null;

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
                string name = userAccess.GetFullNameById(appointment.UserId);

                if (name == null)
                {
                    name = "Onbekend";
                }

                string roomname = roomAccess.GetRoomNameById(appointment.RoomId);

                if (roomname == null)
                {
                    roomname = "";
                }

                Console.ForegroundColor = ConsoleColor.Yellow;

                if (i + 1 < 10)
                {
                    Console.WriteLine($"{i + 1}.  {time:HH:mm} | Patiënt: {name} | Kamer: {roomname}");
                }
                else
                {
                    Console.WriteLine($"{i + 1}. {time:HH:mm} | Patiënt: {name} | Kamer: {roomname}");
                }

                Console.ResetColor();
            }
            else
            {
                if (i + 1 < 10)
                {
                    Console.WriteLine($"{i + 1}.  {time:HH:mm} | Beschikbaar");
                }
                else
                {
                    Console.WriteLine($"{i + 1}. {time:HH:mm} | Beschikbaar");
                }
            }

            if (date.Date == DateTime.Now.Date &&
                time.ToString("HH:mm") == nextSlot.ToString("HH:mm"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{now:HH:mm}   ─────────────────────────");
                Console.ResetColor();
            }

            time = time.AddMinutes(30);
        }
    }
}