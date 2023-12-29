using System.ComponentModel;
using System.Net;
using System.Text;

namespace NuosHelpBot.Parser;

public class Downloader
{
    private Bot _bot;
    private WebClient _client;

    public Downloader(Bot bot)
    {
        _bot = bot;
        _client = new();
    }

    public void Download()
    {
        string url = _bot.Configuration.Get("FullTimeAutumnCourse1");
        string name = "FullTimeAutumnCourse1.xlsx";

        Console.WriteLine($"Downloading file: {url} to {name}");

        FileDownloader _downloader = new();
        _downloader.DownloadFileAsync(url, name);
    }
}
