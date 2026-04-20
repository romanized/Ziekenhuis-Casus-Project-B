public class ReservationModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? SpecialistId { get; set; }
    public long RoomId { get; set; }

    public string Date { get; set; } = "";
    public string Time { get; set; } = "";
    public string Status { get; set; } = "";
    public string Type { get; set; } = "";

    public string RoomNumber { get; set; } = "";
    public string PatientName { get; set; } = "";
    public string DoctorName { get; set; } = "";
}