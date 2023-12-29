using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using NuosHelpBot.Commands;
using NuosHelpBot.Callbacks;
using NuosHelpBot.Controllers;
using NuosHelpBot.Parser;

namespace NuosHelpBot;

public class Bot
{
    public TelegramBotClient Client;
    public BotConfiguration Configuration;
    public BotContext Context;
    public BotTimeManager TimeManager;
    public BotMessageFormatter MessageFormatter;
    public KeyboardController KeyboardController;

    public Downloader Downloader;

    private List<Command> _commands;
    private List<Callback> _callbacks;

    public Bot()
    {
        Configuration = new();

        Client = new TelegramBotClient(Configuration.Get("botToken"));

        _commands = new List<Command>
        {
            new StartCommand(),
            new ScheduleTodayCommand(),
            new ScheduleWeekCommand(),
            new ScheduleGroupCommand(),
            new InfoCommand(),
            new SettingsCommand(),
            new NotificationsSettingsCommand(),
            new ChangeGroupCommand(),
            new ChangeSubgroupCommand(),
            new MainMenuCommand()
        };
        _callbacks = new List<Callback>
        {
            new ChooseGroupCallback(),
            new SetGroupCallback(),
            new SetSubgroupCallback(),
            new SetNotificationsCallback(),
            new GetScheduleGroupCallback(),
            new GetScheduleWeekCallback()
        };

        KeyboardController = new();
        MessageFormatter = new(this);
        Context = new(Configuration.Get("dbConnectionString"));
        TimeManager = new(this);

        Downloader = new Downloader(this);
    }

    public void Start()
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

        TimeManager.Start();

        Console.WriteLine($"Bot started: {Client.GetMeAsync().Result.FirstName}");

        Downloader.Download();
    }

    public async void NotifyStudents(int week, int day, int timeNumber)
    {
        var students = await Context.GetNotifiedStudents();

        var schedule = await Context.GetSchedule(week, day, timeNumber);

        foreach (var student in students)
        {
            try
            {
                var studentClass = 
                    from c in schedule
                    where c.Group.Code == student.Group.Code &&
                    (c.Subgroup.Type == 0 || c.Subgroup.Type == student.Subgroup.Type)
                    select c;

                if (studentClass.Count() > 0)
                {
                    await Client.SendTextMessageAsync(
                        student.TelegramId,
                        MessageFormatter.ScheduleToNotify(studentClass),
                        parseMode: ParseMode.Html);
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Message for {student.TelegramName} was not sent.");
            }
        }
    }

    private Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var additionalDebugInfo = bool.Parse(Configuration.Get("additionalDebugInfo"));
        if (additionalDebugInfo)
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
