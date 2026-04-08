public class ReservationLogic
{
    private ReservationAcesss _access = new();

    public ReservationLogic(){}

    public void Make(
        Int64 userid,
        Int64 roomid,
        Int64 specialistid,
        Int64 infoFolderid,
        string date,
        string type,
        string doel,
        string status,
        string sleutel1,
        string sleutel2)
    {
        var reservation = new ReservationModel(
            0, userid,roomid,specialistid,infoFolderid,date,type,doel,status,sleutel1,sleutel2
        );
        _access.Write(reservation);

    }



}