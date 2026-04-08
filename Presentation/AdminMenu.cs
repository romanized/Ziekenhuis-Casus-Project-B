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
        Console.Write("Enter room name: ");
        string? name = Console.ReadLine();

        Console.Write("Enter room type (e.g. consultation, operation): ");
        string? type = Console.ReadLine();

        Console.Write("Enter room location (e.g. Floor 2, Building A): ");
        string? location = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(location))
        {
            Console.WriteLine("Name, type and location cannot be empty.");
            return;
        }

        bool success = roomLogic.AddRoom(name, type, location);
        if (success)
        {
            Console.WriteLine($"Room '{name}' has been added successfully.");
        }
        else
        {
            Console.WriteLine($"A room named '{name}' already exists.");
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
            Console.WriteLine($"ID: {room.Id} | Name: {room.Name} | Type: {room.Type} | Location: {room.Location}");
        }
    }
}
