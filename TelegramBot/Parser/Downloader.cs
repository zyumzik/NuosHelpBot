using System.Configuration;

namespace NuosHelpBot.Parser;

public class Downloader
{
    public static async Task DownloadSchedules()
    {
        //         path,   url
        Dictionary<string, string> documents = new() 
        { 
            { "FT_AUT_1_B", ConfigurationManager.AppSettings["FullTimeAutumn1"] },
            { "FT_AUT_2_B", ConfigurationManager.AppSettings["FullTimeAutumn2"] },
            { "FT_AUT_3_B", ConfigurationManager.AppSettings["FullTimeAutumn3"] },
            { "FT_AUT_4_B", ConfigurationManager.AppSettings["FullTimeAutumn4"] },
            { "FT_AUT_5_M", ConfigurationManager.AppSettings["FullTimeAutumn5"] },
            { "FT_AUT_6_M", ConfigurationManager.AppSettings["FullTimeAutumn6"] },
            
            { "FT_SPR_1_B", ConfigurationManager.AppSettings["FullTimeSpring1"] },
            { "FT_SPR_2_B", ConfigurationManager.AppSettings["FullTimeSpring2"] },
            { "FT_SPR_3_B", ConfigurationManager.AppSettings["FullTimeSpring3"] },
            { "FT_SPR_4_B", ConfigurationManager.AppSettings["FullTimeSpring4"] },
            { "FT_SPR_5_B", ConfigurationManager.AppSettings["FullTimeSpring5"] },
            
            { "PT_AUT_1_B", ConfigurationManager.AppSettings["PartTimeAutumn1"] },
            { "PT_AUT_2_B", ConfigurationManager.AppSettings["PartTimeAutumn2"] },
            { "PT_AUT_3_B", ConfigurationManager.AppSettings["PartTimeAutumn3"] },
            { "PT_AUT_4_B", ConfigurationManager.AppSettings["PartTimeAutumn4"] },
            { "PT_AUT_5_B", ConfigurationManager.AppSettings["PartTimeAutumn5"] },
            { "PT_AUT_5_M", ConfigurationManager.AppSettings["PartTimeAutumn5M"] },
            { "PT_AUT_6_M", ConfigurationManager.AppSettings["PartTimeAutumn6M"] },
            
            { "PT_SPR_1_B", ConfigurationManager.AppSettings["PartTimeSpring1"] },
            { "PT_SPR_2_B", ConfigurationManager.AppSettings["PartTimeSpring2"] },
            { "PT_SPR_3_B", ConfigurationManager.AppSettings["PartTimeSpring3"] },
            { "PT_SPR_4_B", ConfigurationManager.AppSettings["PartTimeSpring4"] },
            { "PT_SPR_5_B", ConfigurationManager.AppSettings["PartTimeSpring5"] },
            { "PT_SPR_5_M", ConfigurationManager.AppSettings["PartTimeSpring5M"] }
        };
        if (!Directory.Exists("schedules")) Directory.CreateDirectory("schedules");

        Console.WriteLine("Downloading started");
        using (HttpClient client = new())
        {
            int counter = 0;
            foreach (var document in documents)
            {
                string url = $"https://docs.google.com/spreadsheets/d/{document.Value}/export?format=xlsx";
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (FileStream fileStream = File.Create("schedules/" + document.Key + ".xlsx"))
                            {
                                await stream.CopyToAsync(fileStream);
                                Console.WriteLine($"File {document.Key} was downloaded successfully");
                            }
                        }
                        counter++;
                    }
                    else
                    {
                        Console.WriteLine("Error occured during file downloading: " +
                            $"{response.StatusCode} - {response.ReasonPhrase} - {document.Key}");
                    }
                }
            }
            Console.WriteLine($"Downloading finished: [{counter} / {documents.Count}]");
        }
    }
}
