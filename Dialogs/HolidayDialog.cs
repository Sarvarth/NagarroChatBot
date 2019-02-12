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
    public class HolidayDialog : IDialog
    {
        private DateRangeDTO _dateRange;
        private HolidayService _holidayService;
        public HolidayDialog(DateRangeDTO dateRange, HolidayService holidayService)
        {
            this._dateRange = dateRange;
            _holidayService = holidayService;
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ShowFlexibleHolidays);
        }

        private async Task ShowFlexibleHolidays(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                List<Holiday> holidays = _holidayService.GetHolidays(false, _dateRange);

                await context.PostAsync($"There are a total of {holidays.Count} holidays");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                resultMessage.Attachments = AdaptiveCardJsonManager.CreateCards(holidays).ToList();

                await context.PostAsync(resultMessage);

            }
            catch (FormCanceledException ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}