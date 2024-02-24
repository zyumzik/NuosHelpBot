using Telegram.Bot.Types;
using Telegram.Bot;

namespace NuosHelpBot.Commands;

public class ChangeGroupCommand : Command
{
    public override string Name => "Змінити групу 🎓";

    public override async Task Execute(Bot bot, Message message)
    {
        var keyboard = Keyboards.EducationFormKb;

        await bot.Client.SendTextMessageAsync(
            message.From.Id,
            "Оберіть форму та рівень освіти: ",
            replyMarkup: keyboard);
    }
}
