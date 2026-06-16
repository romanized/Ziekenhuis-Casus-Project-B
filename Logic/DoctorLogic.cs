public class DoctorLogic
{
    private DoctorAccess _access;

    public DoctorLogic() { _access = new DoctorAccess(); }

    public ReservationModel? GetNextActiveReservationByDoctorId(long doctorId)
    {
        return _access.GetNextActiveReservationByDoctorId(doctorId);
    }

    public List<ReservationModel> GetAllReservationsByDoctorId(long doctorId)
    {
        return _access.GetAllReservationsByDoctorId(doctorId);
    }

    public List<ReservationModel> GetAllReservationsByDoctorIdByDate(long doctorId, DateTime date)
    {
        return _access.GetAllReservationsByDoctorIdByDate(doctorId, date);
    }
}
