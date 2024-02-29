using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace NuosHelpBot.Callbacks;

internal class SetCourseCallback : Callback
{
    public override string Name => "/setCourse";  // arg[0]: course; arg[1]: educationForm; arg[2]: educationLevel

    public override async Task Execute(Bot bot, CallbackQuery query)
    {
        var args = ParseArgs(query);

        IReplyMarkup keyboard;
        
        using (var context = new BotContext())
        {
            var groups = context.GetGroups(int.Parse(args[0]), args[1], args[2]);
            keyboard = Keyboards.GroupsKb(groups);
        }
        
        var text = "Оберіть групу: ";

        await bot.Client.SendTextMessageAsync(
            query.From.Id,
            text,
            replyMarkup: keyboard);

    }
}
