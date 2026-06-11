// tests voor TemplateAccess, gebruiken in-memory SQLite zodat de echte DB niet wordt aangeraakt
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TemplateAccessTests
{
    private TemplateAccess MakeAccess()
    {
        // elke test krijgt zijn eigen lege database
        return new TemplateAccess("Data Source=:memory:");
    }

    [TestMethod]
    public void AddTemplate_ShouldSaveToDb()
    {
        var access = MakeAccess();

        access.AddTemplate(new TemplateModel
        {
            Name = "Zwangerschapscontrole",
            Type = "Controle",
            Notes = "week 20"
        });

        List<TemplateModel> result = access.GetAll();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Zwangerschapscontrole", result[0].Name);
        Assert.AreEqual("Controle", result[0].Type);
        Assert.AreEqual("week 20", result[0].Notes);
    }

    [TestMethod]
    public void GetAll_EmptyDb_ReturnsEmptyList()
    {
        var access = MakeAccess();

        List<TemplateModel> result = access.GetAll();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void DeleteTemplate_ShouldRemoveFromDb()
    {
        var access = MakeAccess();

        access.AddTemplate(new TemplateModel { Name = "Te verwijderen", Type = "Consult", Notes = "" });

        List<TemplateModel> before = access.GetAll();
        Assert.AreEqual(1, before.Count);

        access.Delete(before[0].Id);

        List<TemplateModel> after = access.GetAll();
        Assert.AreEqual(0, after.Count);
    }
}
