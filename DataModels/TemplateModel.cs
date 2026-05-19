// een template is een naam + afspraaktype + optionele notitie voor de planner
public class TemplateModel
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    // mag leeg zijn, maar helpt de planner soms even herinneren wat er moet gebeuren
    public string Notes { get; set; } = "";
}
