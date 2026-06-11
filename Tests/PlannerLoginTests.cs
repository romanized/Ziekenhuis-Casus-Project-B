using Microsoft.VisualStudio.TestTools.UnitTesting;

// planner wordt door admin aangemaakt en kan dan inloggen
[TestClass]
public class PlannerLoginTests
{
    [TestMethod]
    public void Planner_kan_inloggen_na_aanmaken()
    {
        var access = new UserAccess("Data Source=:memory:");
        var logic = new UserLogic(access);

        logic.CreateEmployee("planner@ziekenhuis.nl", "plan1", "Planner Een", "planner", "");
        var result = logic.CheckLogin("planner@ziekenhuis.nl", "plan1");

        Assert.IsNotNull(result);
        Assert.AreEqual("planner", result.Role);
    }
}
