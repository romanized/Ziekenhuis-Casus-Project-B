using Xunit;

// de gebruiker krijgt een tweede kans bij een fout telefoonnummer of een foute notitie
public class RegisterSecondChanceTests
{
    [Theory]
    [InlineData("0612345678")]
    [InlineData("06 12 34 56 78")]
    [InlineData("+31612345678")]
    [InlineData("010-1234567")]
    public void Telefoonnummer_met_cijfers_is_geldig(string phone)
    {
        Assert.True(UserRegister.IsValidPhoneNumber(phone));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abcdefg")]
    [InlineData("06abcdef")]
    [InlineData("geen nummer")]
    public void Telefoonnummer_met_letters_of_leeg_is_ongeldig(string phone)
    {
        Assert.False(UserRegister.IsValidPhoneNumber(phone));
    }

    [Theory]
    [InlineData("Allergisch voor penicilline")]
    [InlineData("Kamer 12 graag")]
    [InlineData("")]
    [InlineData("   ")]
    public void Notitie_met_tekst_of_leeg_is_geldig(string note)
    {
        Assert.True(UserRegister.IsValidNote(note));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("0")]
    [InlineData("99999999")]
    public void Notitie_met_alleen_cijfers_is_ongeldig(string note)
    {
        Assert.False(UserRegister.IsValidNote(note));
    }
}
