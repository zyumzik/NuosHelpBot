//using System.Timers;
using System.Configuration;
using Microsoft.Win32;
using NuosHelpBot.Models;

namespace NuosHelpBot;

public class BotTimeManager
{
    public event EventHandler<OnTimerElapsedEventArgs> OnTimerElapsed;

    private List<Time> _times;
    private Timer _timer;
    private int _timerInterval;
    private int _timerOffset;

    public static int CurrentSemester
    {
        get
        {
            var nowTime = DateTime.Now;

            if (nowTime.Month >= 6 && nowTime.Month <= 12) return 1;
            else return 2;
        }
    }

    public static int CurrentWeek
    {
        get
        {
            var autumnTerm = DateTime.Parse(ConfigurationManager.AppSettings["autumnTermDate"]);
            var springTerm = DateTime.Parse(ConfigurationManager.AppSettings["springTermDate"]);

            DateTime startDate = DateTime.Now;
            if (startDate <= springTerm) startDate = autumnTerm;
            else startDate = autumnTerm;

            var weeks = (DateTime.Now.DayOfYear - startDate.DayOfYear) / 7;

            return weeks % 2 == 0 ? 1 : 2;
        }
    }

    public static int CurrentDay
    {
        get => (int)DateTime.Now.DayOfWeek;
    }

    public class OnTimerElapsedEventArgs : EventArgs
    {
        public int Semester;
        public int Week;
        public int Day;
        public int Time;
    }

    public BotTimeManager()
    {
        _timerOffset = int.Parse(ConfigurationManager.AppSettings["minutesBeforeClassNotify"]);
    }

    public void Start()
    {
        using (var context = new BotContext())
        {
            _times = context.GetTimes().ToList();
        }

        _timerInterval = TimeBeforeNext5Mins();
        _timer = new(TimerCallback, null, _timerInterval, Timeout.Infinite);

        // testing 
        _times.Add(new() 
        { 
            Number = 3, 
            StartTime = new(12, 50, 00),
            EndTime = new(13, 00, 00) 
        });
    }

    private void TimerCallback(object state)
    {
        var currentTime = DateTime.Now;
        var time = (from t in _times where
                   t.StartTime.Hours == currentTime.Hour &&
                   t.StartTime.Minutes == currentTime.Minute + _timerOffset
                    select t).FirstOrDefault();
        if (time != null)
        {
            OnTimerElapsed?.Invoke(this, new()
            {
                Semester = CurrentSemester,
                Week = CurrentWeek,
                Day = CurrentDay,
                Time = time.Number
            });
            Console.WriteLine($"Timer elapsed: {time.Number} - {time.StartTime}");
        }
        _timerInterval = TimeBeforeNext5Mins();
        _timer.Change(_timerInterval, Timeout.Infinite);
    }

    private int TimeBeforeNext5Mins()
    {
        DateTime now = DateTime.Now;

        DateTime next5Minute = new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / 5) * 5, 0);

        if (next5Minute < now)
        {
            next5Minute = next5Minute.AddMinutes(5);
        }

        TimeSpan timeUntilNext5Minute = next5Minute - now;

        return (int)timeUntilNext5Minute.TotalMilliseconds;
    }
}
