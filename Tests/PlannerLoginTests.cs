using Xunit;

// planner wordt door admin aangemaakt en kan dan inloggen
public class PlannerLoginTests
{
    [Fact]
    public void Planner_kan_inloggen_na_aanmaken()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        logic.CreateEmployee("planner@ziekenhuis.nl", "plan1", "Planner Een", "planner", "");
        var result = logic.CheckLogin("planner@ziekenhuis.nl", "plan1");

        Assert.NotNull(result);
        Assert.Equal("planner", result.Role);
    }
}
