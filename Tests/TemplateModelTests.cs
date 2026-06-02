// losse tests voor het model zelf, geen DB nodig
using Xunit;

public class TemplateModelTests
{
    [Fact]
    public void TemplateModel_DefaultValues_AreEmpty()
    {
        var model = new TemplateModel();

        Assert.Equal("", model.Name);
        Assert.Equal("", model.Type);
        Assert.Equal("", model.Notes);
    }

    [Fact]
    public void TemplateModel_CanSetValues()
    {
        var model = new TemplateModel
        {
            Name = "Operatie voorbereiding",
            Type = "Operatie",
            Notes = "patiënt moet nuchter zijn"
        };

        Assert.Equal("Operatie voorbereiding", model.Name);
        Assert.Equal("Operatie", model.Type);
        Assert.Equal("patiënt moet nuchter zijn", model.Notes);
    }

    [Fact]
    public void AddMultipleTemplates_ShouldAllBeSaved()
    {
        var access = new TemplateAccess("Data Source=:memory:");

        access.AddTemplate(new TemplateModel { Name = "A", Type = "Controle", Notes = "" });
        access.AddTemplate(new TemplateModel { Name = "B", Type = "Consult", Notes = "" });
        access.AddTemplate(new TemplateModel { Name = "C", Type = "Spoedgeval", Notes = "" });

        List<TemplateModel> result = access.GetAll();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetAll_ShouldBeSortedByName()
    {
        var access = new TemplateAccess("Data Source=:memory:");

        // opgeslagen in omgekeerde volgorde, maar moeten gesorteerd terugkomen
        access.AddTemplate(new TemplateModel { Name = "Zwangerschap", Type = "Controle", Notes = "" });
        access.AddTemplate(new TemplateModel { Name = "Algemeen", Type = "Algemeen", Notes = "" });

        List<TemplateModel> result = access.GetAll();

        Assert.Equal("Algemeen", result[0].Name);
        Assert.Equal("Zwangerschap", result[1].Name);
    }

    [Fact]
    public void DeleteOnlyOne_ShouldLeaveOthersAlone()
    {
        var access = new TemplateAccess("Data Source=:memory:");

        access.AddTemplate(new TemplateModel { Name = "Bewaren", Type = "Controle", Notes = "" });
        access.AddTemplate(new TemplateModel { Name = "Weggooien", Type = "Consult", Notes = "" });

        List<TemplateModel> before = access.GetAll();
        long idToDelete = before.First(t => t.Name == "Weggooien").Id;
        access.Delete(idToDelete);

        List<TemplateModel> after = access.GetAll();

        Assert.Single(after);
        Assert.Equal("Bewaren", after[0].Name);
    }
}
