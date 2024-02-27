using NuosHelpBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace NuosHelpBot;

public class BotContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassType> ClassTypes { get; set; }
    public DbSet<Discipline> Disciplines { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Time> Times { get; set; }

    public BotContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = ConfigurationManager.AppSettings["dbConnectionString"];
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>().
            HasOne(c => c.Time).
            WithMany(t => t.Classes).
            HasForeignKey(c => c.TimeId);
        modelBuilder.Entity<Class>().
            HasOne(c => c.ClassType).
            WithMany(ct => ct.Classes).
            HasForeignKey(c => c.ClassTypeId);
        modelBuilder.Entity<Class>().
            HasOne(c => c.Discipline).
            WithMany(d => d.Classes).
            HasForeignKey(c => c.DisciplineId);
        modelBuilder.Entity<Class>().
            HasOne(c => c.Teacher).
            WithMany(t => t.Classes).
            HasForeignKey(c => c.TeacherId);
        modelBuilder.Entity<Class>().
            HasOne(c => c.Group).
            WithMany(g => g.Classes).
            HasForeignKey(c => c.GroupId);

        modelBuilder.Entity<User>().
            HasOne(u => u.Group).
            WithMany(g => g.Users).
            HasForeignKey(u => u.GroupId);
    }

    public bool UserExists(long telegramId)
    {
        var query = from users in Users
                    where users.TelegramId == telegramId
                    select users;
        var student = query.FirstOrDefault();
        return student != null;
    }

    public void AddUser(User user)
    {
        Users.Add(user);
        SaveChanges();
    }

    public IEnumerable<User> GetNotifiedUsers()
    {
        var users = from u in Users
                    where u.Notify == true
                    select u;
        return users;
    }

    public IEnumerable<Group> GetGroups(int course, string educationForm, string educationLevel)
    {
        var groups = from g in Groups where 
                     g.Course == course &&
                     string.Equals(g.EducationForm, educationForm) &&
                     string.Equals(g.EducationLevel, educationLevel)
                     select g;
        return groups;
    }

    public Group? GetGroup(string code, int course, string educationForm, string educationLevel)
    {
        var group = (from g in Groups where 
                     g.Course == course &&
                     string.Equals(g.Code, code) &&
                     string.Equals(g.EducationForm, educationForm) &&
                     string.Equals(g.EducationLevel, educationLevel)
                     select g).
                     Include(g => g.Users).
                     Include(g => g.Classes).
                     FirstOrDefault();
        return group;
    }

    public string SetStudentGroup(long telegramId, int groupId)
    {
        var student = (from users in Users
                       where users.TelegramId == telegramId
                       select users).FirstOrDefault();
        var group = (from groups in Groups
                     where groups.Id == groupId
                     select groups).FirstOrDefault();

        if (student != null && group != null)
        {
            student.Group = group;
            SaveChanges();
            return group.Code;
        }

        return "";
    }

    public void SetStudentNotifications(long telegramId, bool notifications)
    {
        var student = (from users in Users
                       where users.TelegramId == telegramId
                       select users).FirstOrDefault();
        if (student != null)
        {
            student.Notify = notifications;
            SaveChanges();
        }
    }

    public IEnumerable<Time> GetTimes()
    {
        return Times;
    }

    public void AddGroup(Group group_)
    {
        var query = (from g in Groups
                     where
                     g.Course == group_.Course &&
                     string.Equals(g.Code, group_.Code) &&
                     string.Equals(g.EducationForm, group_.EducationForm) &&
                     string.Equals(g.EducationLevel, group_.EducationLevel)
                     select g)
                     .Include(g => g.Users)
                     .Include(g => g.Classes)
                     .FirstOrDefault();
        if (query == null)
        {
            Groups.Add(group_);
            SaveChanges();
        }
    }

    public Time? GetTime(int timeNumber)
    {
        var time = (from times in Times
                    where times.Number == timeNumber
                    select times).
                    Include(t => t.Classes).
                    FirstOrDefault();
        return time;
    }

    public ClassType GetClassType(string name)
    {
        var classType = (from classTypes in ClassTypes
                         where classTypes.Name == name
                         select classTypes).
                         Include(c => c.Classes).
                         FirstOrDefault();
        if (classType == null)
        {
            classType = (from classTypes in ClassTypes
                         where classTypes.Id == 1
                         select classTypes).
                         Include(c => c.Classes).
                         FirstOrDefault();
        }
        return classType;
    }

    public Discipline GetOrAddDiscipline(string name)
    {
        var discipline = (from disciplines in  Disciplines
                          where string.Equals(disciplines.Name, name)
                          select disciplines).
                          Include(d => d.Classes).
                          FirstOrDefault();
        if (discipline != null) return discipline;
        else
        {
            Disciplines.Add(new() { Name = name });
            SaveChanges();

            return GetOrAddDiscipline(name);
        }
    }

    public Teacher GetOrAddTeacher(string name)
    {
        var teacher = (from teachers in Teachers
                       where string.Equals(teachers.Name, name)
                       select teachers).
                       Include(t => t.Classes).
                       FirstOrDefault();
        if (teacher != null) return teacher;
        else
        {
            Teachers.Add(new() { Name = name }); 
            SaveChanges();

            return GetOrAddTeacher(name);
        }
    }

    public void AddClasses(List<Class> classes)
    {
        Classes.AddRange(classes);
        SaveChanges();
    }

    public IEnumerable<Class> GetClasses(long telegramId, int week, int day, int semester, int time = 0)
    {
        var user = Users
            .Where(u => u.TelegramId == telegramId)
            .Include(u => u.Group)
            .FirstOrDefault();
        var classes = Classes.Where(c => 
            c.GroupId == user.GroupId && 
            c.Semester == semester && 
            c.Week == week && 
            c.Day == day)
            .Include(c => c.Time)
            .Include(c => c.ClassType)
            .Include(c => c.Discipline)
            .Include(c => c.Teacher)
            .Include(c => c.Group)
            .OrderBy(c => c.Time.Number);
        
        if (time != 0)
        {
            return classes.Where(c => c.Time.Number == time);
        }

        return classes;
    }
}
