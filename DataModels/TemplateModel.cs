// a template is a name + appointment type + optional note for the planner
public class TemplateModel
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    // can be empty, but helps remind the planner what needs to happen
    public string Notes { get; set; } = "";
}
