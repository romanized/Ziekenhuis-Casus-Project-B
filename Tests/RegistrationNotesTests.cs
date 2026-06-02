using Xunit;

public class RegistrationNotesTests
{
    [Fact]
    public void Notities_worden_opgeslagen_bij_register()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        logic.Register("notitie@test.nl", "wachtwoord1", "Notitie Ouder",
            "1990-01-01", "0612345678", "",
            "Allergisch voor penicilline");

        var saved = access.GetByEmail("notitie@test.nl");
        Assert.Equal("Allergisch voor penicilline", saved.Notes);
    }
}
