using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NuosHelpBot.Callbacks;

public class SetSubgroupCallback : Callback
{
    public override string Name => "/setSubgroup";  // arg[0] - subgroupType, arg[1] - setNotifications

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);
        var subgroupType = int.Parse(args[0]);
        bool setNotifications = bool.Parse(args[1]);

        await bot.Context.SetStudentSubgroup(query.From.Id, subgroupType);

        IReplyMarkup keyboard = setNotifications 
            ? bot.KeyboardController.SetNotifying
            : bot.KeyboardController.MainMenu;
        var text = $"Обрано підгрупу {subgroupType}. ";
        text += setNotifications ? "Чи бажаєте ви отримувати повідомлення про початок пар?" : "";

        await bot.Client.SendTextMessageAsync(
            query.Message.Chat.Id,
            text,
            replyMarkup: keyboard);
    }
}
