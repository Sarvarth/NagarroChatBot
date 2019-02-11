using Microsoft.Bot.Builder.Dialogs;
using SimpleEchoBot.Models;
using SimpleEchoBot.Models.Enums;
using SimpleEchoBot.Services;
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
            await context.PostAsync("Inside ShowRequestDialog");
            context.Wait(ShowSubmittedRequests);
        }

        private async Task ShowSubmittedRequests(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                switch (_showRequest)
                {
                    case ShowRequest.Flexible:
                        {
                            var flexibleHolidays = flexibleHolidayService.GetOptedInHolidays(_dateRange);

                            break;
                        }
                    case ShowRequest.Leave:
                        {
                            var leaves = leaveService.GetLeaveAsync(_dateRange);
                            break;
                        }
                    case ShowRequest.All:
                        {
                            var flexibleHolidays = flexibleHolidayService.GetOptedInHolidays(_dateRange);
                            var leaves = leaveService.GetLeaveAsync(_dateRange);

                            break;
                        }
                }




            }
            catch (Exception ex)
            {

            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}