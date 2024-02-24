using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Callbacks;

public class SetEducationFormCallback : Callback
{
    public override string Name => "/setEducationForm";  // args[0]: education form; args[1]: education level

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);

        var keyboard = Keyboards.CoursesKb(args[0], args[1], BotTimeManager.CurrentSemester);
        var text = "Оберть курс:";

        await bot.Client.SendTextMessageAsync(
            query.From.Id,
            text,
            replyMarkup: keyboard);
    }
}
