public class ReservationModel
{
    public Int64 Id { get; set; }
    public Int64 User_Id { get; set; }
    public Int64 Room_Id { get; set; }
    public Int64 Specialist_Id { get; set; }
    public Int64 Infofolder_Id { get; set; }
    public string Date { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public string Sleutel1 { get; set; }
    public string Sleutel2 { get; set; }
    public ReservationModel() { }

    public ReservationModel(
        Int64 id,
        Int64 userId,
        Int64 roomId,
        Int64 specialistId,
        Int64 infofolderId,
        string date,
        string type,
        string doel,
        string status,
        string sleutel1,
        string sleutel2)
    {
        Id = id;
        User_Id = userId;
        Room_Id = roomId;
        Specialist_Id = specialistId;
        Infofolder_Id = infofolderId;
        Date = date;
        Type = type;
        Status = status;
        Sleutel1 = sleutel1;
        Sleutel2 = sleutel2;
    }
}