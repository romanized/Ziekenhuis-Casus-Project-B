using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class LogoutTests
{
    [TestMethod]
    public void Logout_zet_currentaccount_op_null()
    {
        var access = new UserAccess("Data Source=:memory:");
        access.Write(new UserModel
        {
            Email = "uit@test.nl",
            Password = "geheim1",
            FullName = "Uitlog Test",
            BirthDate = "", PhoneNumber = "", StartDate = "",
            Role = "ouder", Specialty = "", Notes = ""
        });
        var logic = new UserLogic(access);
        logic.CheckLogin("uit@test.nl", "geheim1");

        UserLogic.Logout();

        Assert.IsNull(UserLogic.CurrentAccount);
    }
}

