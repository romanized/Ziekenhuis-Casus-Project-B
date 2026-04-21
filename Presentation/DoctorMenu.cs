using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

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

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
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
        Console.WriteLine("Enter date (YYYY/MM/DD):");

        DateTime date = Convert.ToDateTime(Console.ReadLine());

        while (true)
        {
            List<ReservationModel> list =
                doctorAccess.GetAllReservationsByDoctorIdByDate(doctor.Id, date);

            AgendaTemplate(doctor, date, list);

            Console.WriteLine("\n1 = Next day | 2 = Previous day | 0 = Exit");
            string input = Console.ReadLine();

            switch (input.Trim())
            {
                case "1":
                    date = date.AddDays(1);
                    break;

                case "2":
                    date = date.AddDays(-1);
                    break;

                case "0":
                    return;
            }
        }
    }

    public static void AgendaTemplate(UserModel doctor, DateTime date, List<ReservationModel> list)
    {
        Console.WriteLine($"\n-- Agenda for {date:yyyy-MM-dd} --");

        DateTime time = date.Date.AddHours(8); 
        ReservationModel apoint = null;

        for (int i = 0; i < 19; i++)
        {
            bool istrue = false;
            apoint = null;

            foreach (var r in list)
            {
                if (r.Time == time.ToString("HH:mm"))
                {
                    istrue = true;
                    apoint = r;
                    break;
                }
            }

            if (istrue && apoint != null)
            {
                UserAccess _acces = new UserAccess();
                string name = _acces.GetFullNameById(apoint.UserId);

                RoomAccess _access = new RoomAccess();
                string roomname = _access.GetRoomNameById(apoint.RoomId);

                Console.WriteLine($"{time:HH:mm} | Patient : {name} Room : {apoint.RoomId} {roomname}");
            }
            else
            {
                Console.WriteLine($"{time:HH:mm} | Available");
            }

            time = time.AddMinutes(30);
        }
    }
}
