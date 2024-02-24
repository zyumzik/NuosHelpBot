using NuosHelpBot.Models;
using IronXL;
using System.Text.RegularExpressions;

namespace NuosHelpBot.Parser;

public class SchedulesParser
{
    public static async void ParseSchedules()
    {
        Console.WriteLine("Start parsing documents...");

        // parsing documents
        var files = Directory.GetFiles("schedules/", "*.xlsx");

        foreach (var file in files)
        {
            Console.WriteLine($"Start parsing file: {file}");

            // <education form FT/PT>_<semester AUT/SPR>_<course>_<education level B/M>
            var fileInfo = file.Replace("schedules/", "").Replace(".xlsx", "").Split('_');

            WorkBook wb = WorkBook.Load(file);
            WorkSheet ws = wb.DefaultWorkSheet;

            // parsing groups columns
            var groupCells =
                (from cells in ws["C5:AZ5"].ToList()
                 where !string.IsNullOrEmpty(cells.Text)
                 select cells).ToList();

            // parsing schedule for each group
            foreach (var cell in groupCells)
            {
                var column = Regex.Replace(cell.AddressString, @"\d", "");
                var cells = ws[$"{column}6:{column}250"].ToList();

                var group = new Models.Group()
                {
                    Code = cell.Text.Replace(" ", ""),
                    Course = int.Parse(fileInfo[2]),
                    EducationForm = fileInfo[0],
                    EducationLevel = fileInfo[3]
                };

                using (var context = new BotContext())
                {
                    context.AddGroup(group);
                }
                
                var classes = ParseClasses(cells, group, fileInfo[1]);

                using (var context = new BotContext())
                {
                    context.AddClasses(classes);
                }

                Console.WriteLine($"\tAdded {classes.Count} classes for group {group.Code}");
            }

            Console.WriteLine($"Document {file} parsed");
        }

        Console.WriteLine("All documents parsed");
    }

    private static List<Class> ParseClasses(List<Cell> cells, Models.Group group, string semester)
    {
        if (group.Code == "3381")
        {

        }

        var result = new List<Class>();
        cells.RemoveRange(120, 5);  // removing empty cells

        using (var context = new BotContext())
        {
            for (int i = 0; i < cells.Count; i += 4)
            {
                if (string.IsNullOrEmpty(cells[i].Text)) continue;  // skip empty class nodes

                var timeIterator = i / 4;
                var dayIterator = timeIterator / 6;
                var weekIterator = dayIterator / 5;

                var parsedClass = new Class()
                {
                    TimeId = context.GetTime((timeIterator % 6) + 1).Id,
                    ClassTypeId = context.GetClassType(cells[i].Text).Id,
                    DisciplineId = context.GetOrAddDiscipline(cells[i + 1].Text).Id,
                    TeacherId = context.GetOrAddTeacher(cells[i + 2].Text).Id,
                    GroupId = context.GetGroup(group.Code, group.Course, group.EducationForm, group.EducationLevel).Id,
                    Day = (dayIterator % 5) + 1,
                    Week = (weekIterator % 5) + 1,
                    Audience = cells[i + 3].Text,
                    Semester = semester == "AUT" ? 1 : 2
                };

                result.Add(parsedClass);
            }
        }

        return result;
    }
}
