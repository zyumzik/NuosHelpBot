using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NuosHelpBot.Callbacks;

public abstract class Callback
{
    public abstract string Name { get; }

    public abstract Task Execute(Bot bot, CallbackQuery query);

    public virtual bool Contains(CallbackQuery query)
    {
        return query.Message.Type == MessageType.Text && query.Data.StartsWith(Name);
    }

    public virtual string[] ParseArgs(CallbackQuery query)
    {
        return query.Data.Remove(0, Name.Length + 1).Split(' ');
    }
}
