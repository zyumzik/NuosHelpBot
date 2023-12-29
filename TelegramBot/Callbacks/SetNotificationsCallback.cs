using Telegram.Bot;
using Telegram.Bot.Types;

namespace NuosHelpBot.Callbacks;

public class SetNotificationsCallback : Callback
{
    public override string Name => "/setNotifications";

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);
        bool notify = args[0] == "true"? true : false;

        await bot.Context.SetStudentsNotifications(query.From.Id, notify);

        var text = notify ? "Ви ввімкнули повідомлення.\n" : "Ви вимкнули повідомлення.\n";
            text += "Перед початком кожної пари ви будете отримувати відповідне до вашої групи та підгрупи повідомлення. " +
            "Ви завжди можете змінити свої дані в налаштуваннях. Приємного користування NuosHelpBot!";
        var keyboard = bot.KeyboardController.MainMenu;

        await bot.Client.SendTextMessageAsync(
            query.From.Id,
            text,
            replyMarkup: keyboard);
    }
}
