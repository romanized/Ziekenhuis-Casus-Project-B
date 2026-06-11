using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AdminCreateEmployeeTests
{
    [TestMethod]
    public void CreateEmployee_dokter_met_specialty()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        bool ok = logic.CreateEmployee("dokter@ziekenhuis.nl", "doc1", "Dr Test", "specialty", "Kindergeneeskunde");

        Assert.IsTrue(ok);
        var saved = access.GetByEmail("dokter@ziekenhuis.nl");
        Assert.AreEqual("specialty", saved.Role);
        Assert.AreEqual("Kindergeneeskunde", saved.Specialty);
    }
}

