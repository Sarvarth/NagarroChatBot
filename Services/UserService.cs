using Microsoft.Bot.Builder.Dialogs;
using SimpleEchoBot.AzureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleEchoBot.Services
{
    [Serializable]
    public class UserService
    {
        private const string usernameKey = "username";
        public static string GetUsername(IDialogContext context)
        {
            return context.UserData.GetValue<string>(usernameKey);
        }
    }
}