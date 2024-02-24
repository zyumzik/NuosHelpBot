using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using NuosHelpBot.Commands;
using NuosHelpBot.Callbacks;
using NuosHelpBot.Parser;

using System.Configuration;
using NuosHelpBot.Extensions;

namespace NuosHelpBot;

public class Bot
{
    public TelegramBotClient Client;
    public BotTimeManager TimeManager;

    private List<Command> _commands;
    private List<Callback> _callbacks;

    public Bot()
    {
        var token = ConfigurationManager.AppSettings["botToken"];
        Client = new TelegramBotClient(token);

        _commands = new List<Command>
        {
            new ChangeGroupCommand(),
            new InfoCommand(),
            new MainMenuCommand(),
            new NotificationsSettingsCommand(),
            new ScheduleGroupCommand(),
            new ScheduleTodayCommand(),
            new ScheduleWeekCommand(),
            new SettingsCommand(),
            new StartCommand()
        };
        _callbacks = new List<Callback>
        {
            new ChooseGroupCallback(),
            new GetScheduleGroupCallback(),
            new GetScheduleWeekCallback(),
            new SetCourseCallback(),
            new SetEducationFormCallback(),
            new SetGroupCallback(),
            new SetNotificationsCallback()
        };

        TimeManager = new(this);
    }

    public async void Start()
    {

        var cancellationToken = new CancellationTokenSource().Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { },
        };

        Client.StartReceiving(
            HandleUpdate,
            HandleError,
            receiverOptions,
            cancellationToken);

        Console.WriteLine($"Bot started: {Client.GetMeAsync().Result.FirstName}");

        if (ConfigurationManager.AppSettings["downloadSchedules"] == "true") await Downloader.DownloadSchedules();
        if (ConfigurationManager.AppSettings["parseSchedules"] == "true") SchedulesParser.ParseSchedules();

        TimeManager.Start();
    }

    public async void NotifyStudents(int week, int day, int timeNumber)
    {
        List<Models.User> users;
        List<Models.Class> schedule;
        using (var context = new BotContext())
        {
            users = context.Users.ToList();
            schedule = context.Classes.ToList();
        }

        foreach (var user in users)
        {
            try
            {
                var studentClass = 
                    from c in schedule
                    //where c.Group.Code == student.Group.Code &&
                    //(c.Subgroup.Type == 0 || c.Subgroup.Type == student.Subgroup.Type)
                    select c;

                if (studentClass.Count() > 0)
                {
                    await Client.SendTextMessageAsync(
                        user.TelegramId,
                        studentClass.ToNotify(),
                        parseMode: ParseMode.Html);
                }
            } catch (Exception e)
            {
                Console.WriteLine($"Message for {user.TelegramId} was not sent. {e.Message}");
            }
        }
    }

    private Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (ConfigurationManager.AppSettings["additionalDebugInfo"] == "true")
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

        switch (update.Type)
        {
            case UpdateType.Message:
                if (update.Message != null) HandleMessage(update.Message);
                break;
            case UpdateType.CallbackQuery:
                if (update.CallbackQuery != null) HandleCallback(update.CallbackQuery);
                break;
            default:
                break;
        }

        return Task.CompletedTask;
    }

    private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));

        return Task.CompletedTask;
    }

    private async void HandleMessage(Message message)
    {
        Console.WriteLine($"<< Message from {message.From.Username}: {message.Text}");
        foreach (var command in _commands)
        {
            if (command.Contains(message)) await command.Execute(this, message);
        }
    }

    private async void HandleCallback(CallbackQuery callbackQuery)
    {
        Console.WriteLine($"<< Callback from {callbackQuery.From.Username}: {callbackQuery.Data}");
        foreach (var callback in _callbacks)
        {
            if (callback.Contains(callbackQuery)) 
                await callback.Execute(this, callbackQuery);
        }
    }
}
