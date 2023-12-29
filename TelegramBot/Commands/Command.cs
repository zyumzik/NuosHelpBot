using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NuosHelpBot.Commands;

public abstract class Command
{
    public abstract string Name { get; }

    public abstract Task Execute(Bot bot, Message message);

    public virtual bool Contains(Message message) 
    {
        return message.Type == MessageType.Text && message.Text.Contains(Name);
    }
}
