using Xunit;

public class AdminCreateEmployeeTests
{
    [Fact]
    public void CreateEmployee_dokter_met_specialty()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        bool ok = logic.CreateEmployee("dokter@ziekenhuis.nl", "doc1", "Dr Test", "specialty", "Kindergeneeskunde");

        Assert.True(ok);
        var saved = access.GetByEmail("dokter@ziekenhuis.nl");
        Assert.Equal("specialty", saved.Role);
        Assert.Equal("Kindergeneeskunde", saved.Specialty);
    }
}
