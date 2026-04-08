static class AdminMenu
{
    static private RoomLogic roomLogic = new RoomLogic();

    public static void Start()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- Admin Menu ---");
            Console.WriteLine("1. Add a room");
            Console.WriteLine("2. View all rooms");
            Console.WriteLine("0. Logout");
            Console.Write("Choose an option: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    AddRoom();
                    break;
                case "2":
                    ViewAllRooms();
                    break;
                case "0":
                    running = false;
                    Menu.Start();
                    break;
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }
    }

    private static void AddRoom()
    {
        Console.Write("Enter room number: ");
        string? roomNumber = Console.ReadLine();

        Console.Write("Enter room type (e.g. consultation, operation): ");
        string? type = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(roomNumber) || string.IsNullOrWhiteSpace(type))
        {
            Console.WriteLine("Room number and type cannot be empty.");
            return;
        }

        bool success = roomLogic.AddRoom(roomNumber, type);
        if (success)
        {
            Console.WriteLine($"Room '{roomNumber}' has been added successfully.");
        }
        else
        {
            Console.WriteLine($"A room with number '{roomNumber}' already exists.");
        }
    }

    private static void ViewAllRooms()
    {
        List<RoomModel> rooms = roomLogic.GetAllRooms();
        if (rooms.Count == 0)
        {
            Console.WriteLine("No rooms found.");
            return;
        }

        Console.WriteLine("\n--- All Rooms ---");
        foreach (RoomModel room in rooms)
        {
            Console.WriteLine($"ID: {room.Id} | Room Number: {room.RoomNumber} | Type: {room.Type}");
        }
    }
}
