// tests voor TemplateAccess, gebruiken in-memory SQLite zodat de echte DB niet wordt aangeraakt
using Xunit;

public class TemplateAccessTests
{
    private TemplateAccess MakeAccess()
    {
        // elke test krijgt zijn eigen lege database
        return new TemplateAccess("Data Source=:memory:");
    }

    [Fact]
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

        Assert.Single(result);
        Assert.Equal("Zwangerschapscontrole", result[0].Name);
        Assert.Equal("Controle", result[0].Type);
        Assert.Equal("week 20", result[0].Notes);
    }

    [Fact]
    public void GetAll_EmptyDb_ReturnsEmptyList()
    {
        var access = MakeAccess();

        List<TemplateModel> result = access.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public void DeleteTemplate_ShouldRemoveFromDb()
    {
        var access = MakeAccess();

        access.AddTemplate(new TemplateModel { Name = "Te verwijderen", Type = "Consult", Notes = "" });

        List<TemplateModel> before = access.GetAll();
        Assert.Single(before);

        access.Delete(before[0].Id);

        List<TemplateModel> after = access.GetAll();
        Assert.Empty(after);
    }
}
