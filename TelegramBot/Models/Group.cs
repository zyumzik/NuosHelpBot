namespace NuosHelpBot.Models;

public class Group
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public int Course { get; set; }
    public string EducationForm { get; set; } = "";
    public string EducationLevel { get; set; } = "";

    public ICollection<User>? Users { get; set; }
    public ICollection<Class>? Classes { get; set; }

    public override string ToString() =>
        $"[id: {Id}, code: {Code}, course: {Course}, educationForm: {EducationForm}, educationLevel: {EducationLevel}]";
}
