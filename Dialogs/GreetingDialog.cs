using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class GreetingDialog : IDialog
    {
        public bool _isFromOtherSource;
        public GreetingDialog(bool isFromOtherSource)
        {
            _isFromOtherSource = isFromOtherSource;
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageRecievedGreeting);
        }

        private async Task MessageRecievedGreeting(IDialogContext context, IAwaitable<object> result)
        {
            if (_isFromOtherSource)
            {
                _isFromOtherSource = false;
                context.Wait(MessageRecievedGreeting);
            }
            else
            {
                var activity = (Activity)await result;
                var name = activity.Text;
                context.Done<string>(name);
            }
        }
    }
}