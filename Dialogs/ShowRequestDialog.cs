using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SimpleEchoBot.Models;
using SimpleEchoBot.Models.Enums;
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
    public class ShowRequestDialog : IDialog
    {
        private DateRangeDTO _dateRange;
        private ShowRequest _showRequest;

        private FlexibleHolidayService flexibleHolidayService;
        private LeaveService leaveService;

        public ShowRequestDialog(DateRangeDTO dateRange, ShowRequest showRequest)
        {
            this._dateRange = dateRange;
            this._showRequest = showRequest;
            this.leaveService = new LeaveService();
            this.flexibleHolidayService = new FlexibleHolidayService();
        }
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ShowSubmittedRequests);
        }

        private async Task ShowSubmittedRequests(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var username = UserService.GetUsername(context);
                switch (_showRequest)
                {
                    case ShowRequest.Flexible:
                        {
                            var flexibleHolidays = await flexibleHolidayService.GetOptedInHolidays(username, _dateRange);

                            await context.PostAsync($"There are a total of {flexibleHolidays.Count} OptedIn Flexible Holidays");

                            var resultMessage = context.MakeMessage();
                            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            resultMessage.Attachments = new List<Attachment>();

                            resultMessage.Attachments = AdaptiveCardJsonManager.CreateCards(flexibleHolidays).ToList();

                            await context.PostAsync(resultMessage);
                            break;
                        }
                    case ShowRequest.Leave:
                        {
                            var leaves = await leaveService.GetLeaveAsync(username, _dateRange);

                            await context.PostAsync($"There are a total of {leaves.Count} Leaves");

                            var resultMessage = context.MakeMessage();
                            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            resultMessage.Attachments = new List<Attachment>();

                            resultMessage.Attachments = AdaptiveCardJsonManager.CreateCards(leaves).ToList();

                            await context.PostAsync(resultMessage);
                            break;
                        }
                    case ShowRequest.All:
                        {
                            var flexibleHolidays =await flexibleHolidayService.GetOptedInHolidays(username, _dateRange);

                            await context.PostAsync($"There are a total of {flexibleHolidays.Count} OptedIn Flexible Holidays");

                            var resultFlexibleHolidayMessage = context.MakeMessage();
                            resultFlexibleHolidayMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            resultFlexibleHolidayMessage.Attachments = new List<Attachment>();

                            resultFlexibleHolidayMessage.Attachments = AdaptiveCardJsonManager.CreateCards(flexibleHolidays).ToList();

                            await context.PostAsync(resultFlexibleHolidayMessage);


                            var leaves = await leaveService.GetLeaveAsync(username, _dateRange);

                            await context.PostAsync($"There are a total of {leaves.Count} Leaves");

                            var resultMessage = context.MakeMessage();
                            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            resultMessage.Attachments = new List<Attachment>();

                            resultMessage.Attachments = AdaptiveCardJsonManager.CreateCards(leaves).ToList();

                            await context.PostAsync(resultMessage);

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "There was some error. Please try again later";
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