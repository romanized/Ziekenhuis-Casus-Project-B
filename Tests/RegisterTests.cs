using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class RegisterTests
{
    [TestMethod]
    public void Register_nieuwe_user_wordt_opgeslagen()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        bool ok = logic.Register("nieuw@test.nl", "wachtwoord1", "Nieuwe Ouder",
            "1990-01-01", "0612345678", "", "geen notities");

        Assert.IsTrue(ok);
        var saved = access.GetByEmail("nieuw@test.nl");
        Assert.IsNotNull(saved);
        Assert.AreEqual("Nieuwe Ouder", saved.FullName);
    }
}
