using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageRecievedGreeting);
        }

        private async Task MessageRecievedGreeting(IDialogContext context, IAwaitable<object> result)
        {
            var activity = (Activity)await result;
            var name = activity.Text;
            context.Done<string>(name);
        }
    }
}