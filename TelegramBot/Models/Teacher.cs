namespace NuosHelpBot.Models;

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<Class>? Classes { get; set; }
}
