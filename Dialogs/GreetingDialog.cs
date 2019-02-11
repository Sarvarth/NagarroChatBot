using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageRecievedGreeting);
            return Task.CompletedTask;
        }

        private async Task MessageRecievedGreeting(IDialogContext context, IAwaitable<object> result)
        {
            var activity = (Activity)await result;
            var name = activity.Text;
            context.Done<string>(name);
        }
    }
}