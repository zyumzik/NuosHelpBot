﻿namespace NuosHelpBot.Models;

public class ClassType
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Class>? Classes { get; set; }
}
