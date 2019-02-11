using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using SimpleEchoBot.Models;
using SimpleEchoBot.Services;
using SimpleEchoBot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class FlexibleHolidayDialog : IDialog
    {
        private DateRangeDTO _dateRange;
        private HolidayService _holidayService;
        public FlexibleHolidayDialog(DateRangeDTO dateRange, HolidayService holidayService)
        {
            this._dateRange = dateRange;
            _holidayService = holidayService;
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Inside FlexibleHolidayDialog");
            context.Wait(ShowFlexibleHolidays);
        }

        private async Task ShowFlexibleHolidays(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                List<Holiday> flexibleHolidays = _holidayService.GetHolidays(true, _dateRange);

                await context.PostAsync($"There are a total of {flexibleHolidays.Count} Flexible holidays");
                await context.PostAsync("To Opt In any Flexible holiday, Press the Opt In button");
                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                resultMessage.Attachments = HeroCardManager.CreateFlexibleHolidayCards(flexibleHolidays).ToList();

                await context.PostAsync(resultMessage);
                context.Wait(OptInRequest);

            }
            catch (FormCanceledException ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation. Quitting from the Flights Search";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
        }

        private async Task OptInRequest(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (!await HandleButtonClick(context, activity))
            {
                await context.PostAsync("You did not Opt into any Flexible holiday");
                await context.Forward(new RootDialog(), AfterNoOptInRequest, activity);
            }
            else
            {
                var holidayIdString = activity.Text.TrimStart('O', 'p', 't', ' ', 'I', 'n', ' ');
                var holidayId = Convert.ToInt32(holidayIdString);
                context.Done<int>(holidayId);
            }

        }

        private Task AfterNoOptInRequest(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<string>(null);
            return Task.CompletedTask;
        }

        public async Task<bool> HandleButtonClick(IDialogContext context, Activity activity)
        {
            if (activity.Text.Contains("Opt In"))
            {

                return true;
            }
            return false;
        }
    }
}