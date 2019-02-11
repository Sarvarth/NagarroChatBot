using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class ApplyLeaveDialog : IDialog
    {
        private List<EntityRecommendation> _entities;
        public ApplyLeaveDialog(List<EntityRecommendation> entities)
        {
            this._entities = entities;
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Inside ApplyLeaveDialog");
            var leaveApplicationForm = new FormDialog<LeaveRequest>(new LeaveRequest(), LeaveRequest.BuildForm, FormOptions.PromptInStart, this._entities);
            context.Call(leaveApplicationForm, ApplyForLeave);
        }

        private async Task ApplyForLeave(IDialogContext context, IAwaitable<LeaveRequest> result)
        {
            var leave = await result;
            context.Done(leave);
        }
    }
}