public class ReservationLogic
{
    private ReservationAccess _access;

    public ReservationLogic() { _access = new ReservationAccess(); }

    public List<ReservationModel> GetAllReservationsByUserId(long userId)
    {
        return _access.GetAllReservationsByUserId(userId);
    }

    public List<ReservationModel> GetReservationsForDate(string date)
    {
        return _access.GetReservationsForDate(date);
    }

    public List<string> GetReservedDatesForMonth(string yearMonth)
    {
        return _access.GetReservedDatesForMonth(yearMonth);
    }

    public void CreateReservation(long userId, long roomId, long? specialistId, string dateTime, string type, long? templateId = null)
    {
        _access.CreateReservation(userId, roomId, specialistId, dateTime, type, templateId);
    }
}
