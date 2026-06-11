using Microsoft.VisualStudio.TestTools.UnitTesting;

// de gebruiker krijgt een tweede kans bij een fout telefoonnummer of een foute notitie
[TestClass]
public class RegisterSecondChanceTests
{
    [DataTestMethod]
    [DataRow("0612345678")]
    [DataRow("06 12 34 56 78")]
    [DataRow("+31612345678")]
    [DataRow("010-1234567")]
    public void Telefoonnummer_met_cijfers_is_geldig(string phone)
    {
        Assert.IsTrue(UserRegister.IsValidPhoneNumber(phone));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("abcdefg")]
    [DataRow("06abcdef")]
    [DataRow("geen nummer")]
    public void Telefoonnummer_met_letters_of_leeg_is_ongeldig(string phone)
    {
        Assert.IsFalse(UserRegister.IsValidPhoneNumber(phone));
    }

    [DataTestMethod]
    [DataRow("Allergisch voor penicilline")]
    [DataRow("Kamer 12 graag")]
    [DataRow("")]
    [DataRow("   ")]
    public void Notitie_met_tekst_of_leeg_is_geldig(string note)
    {
        Assert.IsTrue(UserRegister.IsValidNote(note));
    }

    [DataTestMethod]
    [DataRow("12345")]
    [DataRow("0")]
    [DataRow("99999999")]
    public void Notitie_met_alleen_cijfers_is_ongeldig(string note)
    {
        Assert.IsFalse(UserRegister.IsValidNote(note));
    }
}

