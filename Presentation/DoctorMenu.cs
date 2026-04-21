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
        string date = Console.ReadLine();
        DateTime newdate = Convert.ToDateTime(date);
        List<ReservationModel> list = doctorAccess.GetAllReservationsByDoctorIdByDate(doctor.Id, newdate);
        Console.WriteLine($"\n-- Agenda for {date:yyyy-MM-dd} --");


        DateTime time = DateTime.Today.AddHours(8); // 09:00
        ReservationModel  apoint = null;

        for (int i = 0; i < 19; i++)
        {
            bool istrue = false;
            foreach (var r in list)
            {
                
            if (r.Time == time.ToString("HH:mm"))
                {
                    istrue = true;
                    apoint = r;
                }
        
            }

            if (istrue)
            {
                        UserAccess _acces = new UserAccess();
                        string name = _acces.GetFullNameById(apoint.UserId);
                        RoomAccess _access = new RoomAccess();
                        string roomname = _access.GetRoomNameById(apoint.RoomId);
                        Console.WriteLine($"{time:HH:mm} | Patient {name} Room : {apoint.RoomId} {roomname}");
            }
            else
            {
                    Console.WriteLine($"{time:HH:mm} | Available ");

            }
            
                    // Console.WriteLine("-------------------------------------------");


            time = time.AddMinutes(30);
        }
}

        
        // foreach (ReservationModel r in list)
        // {


            
        //     Console.WriteLine($"{r.Date} {r.Time} | Room {r.RoomNumber} | Status: {r.Status}");
        // }
    }
