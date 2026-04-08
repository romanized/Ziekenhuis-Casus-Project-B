static class ParentMenu
{
    public static void Start(UserModel user)
    {
        ReservationAccess reservationAccess = new ReservationAccess();
        ReservationModel? nextReservation = reservationAccess.GetNextActiveReservationByUserId(user.Id);

        Console.WriteLine("==== Parent Main Menu ====");

        if (nextReservation != null)
        {
            Console.WriteLine("Your next active appointment:");
            Console.WriteLine("Date: " + nextReservation.Date);
            Console.WriteLine("Time: " + nextReservation.Time);
            Console.WriteLine("Room: " + nextReservation.RoomNumber);
        }
        else
        {
            Console.WriteLine("You have no active upcoming appointments.");
        }

        Console.WriteLine();
        Console.WriteLine("Press 0 to go back");

        string? input = Console.ReadLine();
        if (input == "0")
        {
            Menu.Start();
        }
    }
}