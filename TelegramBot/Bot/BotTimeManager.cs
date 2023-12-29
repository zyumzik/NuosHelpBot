using System.Timers;
using System.Configuration;
using NuosHelpBot.Models;

namespace NuosHelpBot;

public class BotTimeManager : IDisposable
{
    private Bot _bot;
    private System.Timers.Timer _timer;
    private IEnumerable<Time> _times;

    public int CurrentWeek
    {
        get
        {
            var autumnTerm = DateTime.Parse(_bot.Configuration.Get("autumnTermDate"));
            var springTerm = DateTime.Parse(_bot.Configuration.Get("springTermDate"));

            DateTime startDate = DateTime.Now;
            if (startDate <= springTerm) startDate = autumnTerm;
            else startDate = autumnTerm;

            var weeks = (DateTime.Now.DayOfYear - startDate.DayOfYear) / 7;

            return weeks % 2 == 0 ? 1 : 2;
        }
    }

    public int CurrentDay
    {
        get => (int)DateTime.Now.DayOfWeek;
    }

    public BotTimeManager(Bot bot)
    {
        _bot = bot;
    }

    public async void Start()
    {
        _times = await _bot.Context.GetRawTable<Time>("Times");

        var minutes = int.Parse(_bot.Configuration.Get("timerTickMinutes"));
        var _tempTimer = new System.Timers.Timer(1000);
        _tempTimer.AutoReset = true;
        _tempTimer.Elapsed += (s, e) =>
        {
            if (DateTime.Now.TimeOfDay.Minutes % minutes == 0)
            {
                _timer = new(minutes * 60 * 1000);
                _timer.AutoReset = true;
                _timer.Elapsed += OnTimerTick;
                _timer.Start();

                _tempTimer.Stop();
                _tempTimer.Dispose();
                Console.WriteLine($"Timer calibrated: {DateTime.Now.TimeOfDay}, period: {minutes}");
            }
        };
        _tempTimer.Start();
    }

    private void OnTimerTick(object? sender, ElapsedEventArgs e)
    {
        var minutes = int.Parse(_bot.Configuration.Get("minutesBeforeClassNotify"));
        var currentTime = DateTime.Now.TimeOfDay;
        foreach (var time in _times)
        {
            if (currentTime.Hours == time.StartTime.Hours &&
                currentTime.Minutes == time.StartTime.Minutes - minutes)
            {
                _bot.NotifyStudents(CurrentWeek, CurrentDay, time.Number);
                Console.WriteLine("Notifying users");
            }
        }
        Console.WriteLine($"Timer tick: {DateTime.Now.TimeOfDay}");
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
