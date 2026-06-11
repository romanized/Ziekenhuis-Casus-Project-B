[TestClass]
public class AppointmentTemplateViewTests
{
    [TestMethod]
    public void Afspraak_met_template_toont_template_aan_patient()
    {
        string conn = "Data Source=file:tpltest1?mode=memory&cache=shared";
        var users = new UserAccess(conn);
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);
        var templates = new TemplateAccess(conn);

        users.Write(new UserModel
        {
            Email = "p@test.nl", Password = "x", FullName = "Patient Een",
            BirthDate = "", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        var patient = users.GetByEmail("p@test.nl");
        rooms.AddRoom(new RoomModel { Name = "Kamer A", Type = "Onderzoek", Location = "1" });
        long roomId = rooms.GetAllRooms()[0].Id;

        templates.AddTemplate(new TemplateModel
        {
            Name = "Zwangerschapscontrole week 20",
            Type = "Checkup",
            Notes = "Neem je zwangerschapsboekje mee."
        });
        long templateId = templates.GetAll()[0].Id;

        reservations.CreateReservation(patient.Id, roomId, null, "2026-06-15 10:00", "Checkup", templateId);

        var result = reservations.GetAllReservationsByUserId(patient.Id);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(templateId, result[0].TemplateId);
        Assert.AreEqual("Zwangerschapscontrole week 20", result[0].TemplateName);
        Assert.AreEqual("Neem je zwangerschapsboekje mee.", result[0].TemplateNotes);
    }

    [TestMethod]
    public void Afspraak_zonder_template_toont_geen_template()
    {
        string conn = "Data Source=file:tpltest2?mode=memory&cache=shared";
        var users = new UserAccess(conn);
        var rooms = new RoomAccess(conn);
        var reservations = new ReservationAccess(conn);

        users.Write(new UserModel
        {
            Email = "p@test.nl", Password = "x", FullName = "Patient Twee",
            BirthDate = "", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        var patient = users.GetByEmail("p@test.nl");
        rooms.AddRoom(new RoomModel { Name = "Kamer A", Type = "Onderzoek", Location = "1" });
        long roomId = rooms.GetAllRooms()[0].Id;

        // geen template meegegeven
        reservations.CreateReservation(patient.Id, roomId, null, "2026-06-15 10:00", "Checkup");

        var result = reservations.GetAllReservationsByUserId(patient.Id);

        Assert.AreEqual(1, result.Count);
        Assert.IsNull(result[0].TemplateId);
        Assert.AreEqual("", result[0].TemplateName);

    }
}
