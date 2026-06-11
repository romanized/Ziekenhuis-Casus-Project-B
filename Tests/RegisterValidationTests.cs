using Microsoft.VisualStudio.TestTools.UnitTesting;

// als email al bestaat mag register niet door
[TestClass]
public class RegisterValidationTests
{
    [TestMethod]
    public void Register_dubbele_email_false()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        bool first = logic.Register("dub@test.nl", "wachtwoord1", "Eerste",
            "1990-01-01", "0611111111", "", "");
        bool second = logic.Register("dub@test.nl", "anders2", "Tweede",
            "1991-01-01", "0622222222", "", "");

        Assert.IsTrue(first);
        Assert.IsFalse(second);
    }
}
