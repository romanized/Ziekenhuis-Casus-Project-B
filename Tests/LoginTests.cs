using Xunit;

// login test - ouder
public class LoginTests
{
    [Fact]
    public void Login_juiste_wachtwoord()
    {
        var access = new UserAccess("Data Source=:memory:");
        access.Write(new UserModel
        {
            Email = "ouder@test.nl",
            Password = "wachtwoord1",
            FullName = "Test Ouder",
            BirthDate = "",
            PhoneNumber = "",
            StartDate = "",
            Role = "ouder",
            Specialty = "",
            Notes = ""
        });
        var logic = new UserLogic(access);

        var result = logic.CheckLogin("ouder@test.nl", "wachtwoord1");

        Assert.NotNull(result);
        Assert.Equal("ouder@test.nl", result.Email);
    }
}
