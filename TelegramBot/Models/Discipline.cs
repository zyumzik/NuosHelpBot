namespace NuosHelpBot.Models;

public class Discipline
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<Class>? Classes { get; set; }
}
