using Npgsql;
using Dapper;
using NuosHelpBot.Models;

namespace NuosHelpBot;

public class BotContext : IDisposable
{
    public NpgsqlConnection Connection;

    public BotContext(string connectionString)
    {
        Connection = new(connectionString);
        try
        {
            Connection.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Context initialization error: " + ex);
        }
    }

    public async Task<IEnumerable<T>> GetRawTable<T>(string table)
    {
        try
        {
            string commandText =
                $"SELECT * " +
                @$"FROM public.""{table}"" " +
                @"ORDER BY ""Id"" ASC";
            return await Connection.QueryAsync<T>(commandText);
        } catch (Exception ex) 
        { 
            Console.WriteLine("GetRowTable error: " + ex); 
            return Enumerable.Empty<T>(); 
        }
    }

    public async Task AddStudent(string telegramName, long telegramId)
    {
        string sql = 
            @"INSERT INTO public.""Students""" +
            @"(""TelegramName"", ""TelegramId"") " +
            @"VALUES (@telegramName, @telegramId)";
        var args = new { telegramName, telegramId };
        await Connection.ExecuteAsync(sql, args);
    }

    public async Task<bool> StudentExists(long telegramId)
    {
        string sql = 
            @"SELECT EXISTS (" +
                @"SELECT * " +
                @"FROM public.""Students"" " +
                @"WHERE ""TelegramId"" = @telegramId)";
        var args = new { telegramId };
        return await Connection.ExecuteScalarAsync<bool>(sql, args);
    }

    public async Task<Student> GetStudent(long telegramId)
    {
        string sql =
            @"SELECT * " +
            @"FROM public.""Students"" s " +
                @"LEFT JOIN public.""Groups"" g " +
                @"ON s.""GroupId"" = g.""Id"" " +
                @"LEFT JOIN public.""Subgroups"" sg " +
                @"ON s.""SubgroupId"" = sg.""Id"" " +
            @"WHERE s.""TelegramId"" = @telegramId " +
            @"ORDER BY s.""Id"" ASC";
        var data = await Connection.QueryAsync<Student, Group, Subgroup, Student>(
            sql, 
            (student, group, subgroup) => 
            { 
                student.Group = group;
                student.Subgroup = subgroup;
                return student; 
            },
            new { telegramId });

        return data.First();
    }

    public async Task<IEnumerable<Student>> GetNotifiedStudents()
    {
            /* SELECT s."Id", s."GroupId", s."Notify", s."SubgroupId", s."TelegramId", s."TelegramName", c."DisciplineId"
FROM public."Classes" AS c
INNER JOIN public."Groups" AS g ON c."GroupId" = g."Id"
INNER JOIN public."Students" AS s ON g."Id" = s."GroupId"
WHERE c."TimeId" = 2 */
        try
        {
            string sql =
                @"SELECT * " + 
                @"FROM public.""Students"" s " + 
                    @"LEFT JOIN public.""Groups"" g " + 
                    @"ON s.""GroupId"" = g.""Id"" " +
                    @"LEFT JOIN public.""Subgroups"" sg " + 
                    @"ON s.""SubgroupId"" = sg.""Id"" " +
                @"WHERE s.""Notify"" = true " + 
                @"ORDER BY s.""Id"" ASC";
            return await Connection.QueryAsync<Student, Group, Subgroup, Student>(
                sql,
                (student, group, subgroup) =>
                {
                    student.Group = group;
                    student.Subgroup = subgroup;
                    return student;
                });

        } catch
        {
            return Enumerable.Empty<Student>();
        }
    }

    public async Task SetStudentGroup(long telegramId, string groupCode)
    {
        string sql =
            @"UPDATE public.""Students"" " +
            @"SET ""GroupId"" = (" +
                @"SELECT ""Id"" " +
                @"FROM public.""Groups"" " +
                @"WHERE ""Code"" = @groupCode) " +
            @"WHERE ""TelegramId"" = @telegramId";
        var args = new { telegramId, groupCode };
        await Connection.ExecuteAsync(sql, args);
    }

    public async Task SetStudentSubgroup(long telegramId, int subgroupType)
    {
        string sql =
            @"UPDATE public.""Students"" " +
            @"SET ""SubgroupId"" = (" +
                @"SELECT ""Id"" " +
                @"FROM public.""Subgroups"" " +
                @"WHERE ""Type"" = @subgroupType) " +
            @"WHERE ""TelegramId"" = @telegramId";
        var args = new { telegramId, subgroupType };
        await Connection.ExecuteAsync(sql, args);
    }

    public async Task SetStudentsNotifications(long telegramId, bool notify)
    {
        string sql =
            @"UPDATE public.""Students"" " +
            @"SET ""Notify"" = @notify " +
            @"WHERE ""TelegramId"" = @telegramId";
        var args = new { telegramId, notify };
        await Connection.ExecuteAsync(sql, args);
    }

    public async Task<bool> ScheduleExists(long telegramId, int week, int day)
    {
        string sql =
            @"SELECT EXISTS (" +
            @"SELECT * " +
            @"FROM public.""Classes"" c " +
            @"WHERE c.""GroupId"" = ( " + 
                @"SELECT ""GroupId"" " + 
                @"FROM public.""Students"" " + 
                @"WHERE ""TelegramId"" = @telegramId) " + 
            @"AND c.""SubgroupId"" = ( " + 
                @"SELECT ""SubgroupId"" " + 
                @"FROM public.""Students"" " +
                @"WHERE ""TelegramId"" = @telegramId) " + 
            @"AND c.""Week"" = @week " + 
            @"AND c.""Day"" = @day)";
        var args = new { telegramId, week, day };
        return await Connection.ExecuteScalarAsync<bool>(sql, args);
    }

    public async Task<IEnumerable<Class>> GetSchedule(long telegramId, int week, int day, bool subgroupSearch = true)
    {
        //if (!await ScheduleExists(telegramId, week, day)) return Enumerable.Empty<Class>();

        try
        {
            string sql =
                @"SELECT * " +
                @"FROM public.""Classes"" c " +
                    @"LEFT JOIN public.""Times"" t " +
                    @"ON c.""TimeId"" = t.""Id"" " +
                    @"LEFT JOIN public.""ClassTypes"" ct " +
                    @"ON c.""ClassTypeId"" = ct.""Id"" " +
                    @"LEFT JOIN public.""Disciplines"" d " +
                    @"ON c.""DisciplineId"" = d.""Id"" " +
                    @"LEFT JOIN public.""Teachers"" tch " +
                    @"ON c.""TeacherId"" = tch.""Id"" " +
                    @"LEFT JOIN public.""Positions"" p " +
                    @"ON p.""Id"" = tch.""PositionId"" " +
                    @"LEFT JOIN public.""Groups"" g " +
                    @"ON c.""GroupId"" = g.""Id"" " +
                    @"LEFT JOIN public.""Subgroups"" sg " +
                    @"ON c.""SubgroupId"" = sg.""Id"" " +
                @"WHERE c.""GroupId"" = (" +
                    @"SELECT ""GroupId"" " +
                    @"FROM public.""Students"" " +
                    @"WHERE ""TelegramId"" = @telegramId) ";
            if (subgroupSearch) sql +=
                @"AND (c.""SubgroupId"" = (" +
                    @"SELECT ""SubgroupId"" " +
                    @"FROM public.""Students"" " +
                    @"WHERE ""TelegramId"" = @telegramId) " +
                    @"OR c.""SubgroupId"" = 1) ";
            sql +=
                @"AND c.""Week"" = @week " +
                @"AND c.""Day"" = @day " +
                @"ORDER BY t.""Number"" ASC";
            var types = new Type[]
            {
            typeof(Class),
            typeof(Time),
            typeof(ClassType),
            typeof(Discipline),
            typeof(Teacher),
            typeof(Position),
            typeof(Group),
            typeof(Subgroup)
            };
            Func<object[], Class> map = (obj) =>
            {
                Class m_class = obj[0] as Class;
                m_class.Time = obj[1] as Time;
                m_class.ClassType = obj[2] as ClassType;
                m_class.Discipline = obj[3] as Discipline;
                m_class.Teacher = obj[4] as Teacher;
                m_class.Teacher.Position = obj[5] as Position;
                m_class.Group = obj[6] as Group;
                m_class.Subgroup = obj[7] as Subgroup;
                return m_class;
            };
            var args = new { telegramId, week, day };

            return await Connection.QueryAsync<Class>(sql, types, map, args);
        } catch (Exception ex)
        {
            return Enumerable.Empty<Class>();
        }
    }

    public async Task<IEnumerable<Class>> GetSchedule(int week, int day, int time)
    {
        try
        {
            string sql =
                @"SELECT * " + 
                @"FROM public.""Classes"" c " +
                    @"LEFT JOIN public.""Times"" t " +
                    @"ON c.""TimeId"" = t.""Id"" " +
                    @"LEFT JOIN public.""ClassTypes"" ct " +
                    @"ON c.""ClassTypeId"" = ct.""Id"" " +
                    @"LEFT JOIN public.""Disciplines"" d " +
                    @"ON c.""DisciplineId"" = d.""Id"" " +
                    @"LEFT JOIN public.""Teachers"" tch " +
                    @"ON c.""TeacherId"" = tch.""Id"" " +
                    @"LEFT JOIN public.""Positions"" p " +
                    @"ON p.""Id"" = tch.""PositionId"" " +
                    @"LEFT JOIN public.""Groups"" g " +
                    @"ON c.""GroupId"" = g.""Id"" " +
                    @"LEFT JOIN public.""Subgroups"" sg " +
                    @"ON c.""SubgroupId"" = sg.""Id"" " + 
                @"WHERE c.""Week"" = @week " + 
                @"AND c.""Day"" = @day " + 
                @"AND t.""Number"" = @time";
            var types = new Type[]
            {
                typeof(Class),
                typeof(Time),
                typeof(ClassType),
                typeof(Discipline),
                typeof(Teacher),
                typeof(Position),
                typeof(Group),
                typeof(Subgroup)
            };
            Func<object[], Class> map = (obj) =>
            {
                Class m_class = obj[0] as Class;
                m_class.Time = obj[1] as Time;
                m_class.ClassType = obj[2] as ClassType;
                m_class.Discipline = obj[3] as Discipline;
                m_class.Teacher = obj[4] as Teacher;
                m_class.Teacher.Position = obj[5] as Position;
                m_class.Group = obj[6] as Group;
                m_class.Subgroup = obj[7] as Subgroup;
                return m_class;
            };
            var args = new { week, day, time };

            return await Connection.QueryAsync<Class>(sql, types, map, args);
        } catch (Exception e)
        {
            return Enumerable.Empty<Class>();
        }
    }

    public void Dispose()
    {
        Connection.Close();
    }
}
