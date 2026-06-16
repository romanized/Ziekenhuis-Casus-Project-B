public class TemplateLogic
{
    private TemplateAccess _access;

    public TemplateLogic() { _access = new TemplateAccess(); }

    public void AddTemplate(TemplateModel template)
    {
        _access.AddTemplate(template);
    }

    public List<TemplateModel> GetAll()
    {
        return _access.GetAll();
    }

    public void Delete(long id)
    {
        _access.Delete(id);
    }
}
