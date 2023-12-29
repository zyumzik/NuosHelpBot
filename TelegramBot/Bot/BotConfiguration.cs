using System.Xml;

namespace NuosHelpBot;

public class BotConfiguration
{
    private Dictionary<string, string> _configuration = new();

    public BotConfiguration() 
    {
        XmlDocument document = new XmlDocument();
        document.Load("configuration.xml");

        var nodes = document.DocumentElement?.ChildNodes;

        foreach (XmlNode item in nodes)
        {
            if (item.Name != "#comment") _configuration.Add(item.Name, item.InnerText);
        }

    }

    public string Get(string key)
    {
        return _configuration[key];
    }
}
