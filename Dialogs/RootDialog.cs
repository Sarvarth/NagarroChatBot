using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using SimpleEchoBot.Models;
using SimpleEchoBot.Models.Enums;
using SimpleEchoBot.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private const string usernameKey = "username";
        private const string flexibleEntity = "Flexible";
        private const string reasonEntity = "Reason";

        private HolidayService holidayService;
        private FlexibleHolidayService flexibleHolidayService;
        private LeaveService leaveService;


        public RootDialog() : base(new LuisService(GetLuisModelAttribute()))
        {
            this.holidayService = new HolidayService();
            this.flexibleHolidayService = new FlexibleHolidayService();
            this.leaveService = new LeaveService();
        }
        private static LuisModelAttribute GetLuisModelAttribute()
        {
            var attribute = new LuisModelAttribute(
                            ConfigurationManager.AppSettings["LuisAppId"],
                            ConfigurationManager.AppSettings["LuisAPIKey"]);
            attribute.SpellCheck = true;
            attribute.BingSpellCheckSubscriptionKey = ConfigurationManager.AppSettings["BingSpellCheckAPIKey"];
            return attribute;
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. \U0001F600 \U0001F600 Type 'help' if you need assistance.";
            //PromptDialog.Confirm(context, AfterPromptForSnoozing, "Do you want to snooze this alarm?");
            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            var hasUsername = context.UserData.ContainsKey(usernameKey);
            if (hasUsername)
            {
                var username = context.UserData.GetValue<string>(usernameKey);
                await context.PostAsync($"Hello {username}, How may i help you today \U0001F604");
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("What is your name");
                context.Call(new GreetingDialog(), SetUsername);
            }
        }

        [LuisIntent("ViewHoliday")]
        public async Task HolidayIntent(IDialogContext context, LuisResult result)
        {
            var hasUsername = context.UserData.ContainsKey(usernameKey);
            if (!hasUsername)
            {
                await context.PostAsync("First tell me your name \U0001F61C");
                context.Call(new GreetingDialog(), SetUsername);
            }

            if(result.AlteredQuery != null)
            {
                await context.PostAsync($"Searching for {result.AlteredQuery} \U0001F609");
            }

            var hasFlexibleEntity = await TryFind(result, flexibleEntity);

            DateTime startTime;
            DateTime? endTime;
            var hasDate = TryGetDateRange(result, true, out startTime, out endTime);
            var dateRange = new DateRangeDTO(startTime, endTime);

            if (hasFlexibleEntity)
            {
                await context.Forward(new FlexibleHolidayDialog(dateRange, holidayService), AfterFlexibleHolidayIntent, result, CancellationToken.None);
            }
            else
            {
                await context.Forward(new HolidayDialog(dateRange, holidayService), AfterHolidayIntent, result, CancellationToken.None);
            }
        }

        [LuisIntent("ApplyLeave")]
        public async Task ApplyLeaveIntent(IDialogContext context, LuisResult result)
        {
            var hasUsername = context.UserData.ContainsKey(usernameKey);
            if (!hasUsername)
            {
                await context.PostAsync("First tell me your name \U0001F61C");
                context.Call(new GreetingDialog(), SetUsername);
            }

            var hasFlexibleEntity = await TryFind(result, flexibleEntity);

            DateTime startTime;
            DateTime? endTime;
            var hasDate = TryGetDateRange(result, false, out startTime, out endTime);
            var dateRange = new DateRangeDTO(startTime, endTime);

            if (hasFlexibleEntity)
            {
                await context.PostAsync("It seems like you want to take a flexible holiday");
                await context.Forward(new FlexibleHolidayDialog(dateRange, holidayService), AfterFlexibleHolidayIntent, result, CancellationToken.None);
            }
            else
            {
                List<EntityRecommendation> entities = new List<EntityRecommendation>();
                var hasReasonEntity = await TryFind(result, reasonEntity);
                if (hasReasonEntity)
                {
                    entities.Add(GetEntity(result, reasonEntity));
                }
                if (hasDate)
                {
                    entities.Add(new EntityRecommendation { Type = "StartDate", Entity = startTime.ToString() });
                    if (endTime != null)
                    {
                        entities.Add(new EntityRecommendation { Type = "EndDate", Entity = endTime.ToString() });
                    }
                }

                context.Call(new ApplyLeaveDialog(entities), AfterApplyLeaveRequest);
            }
        }

        [LuisIntent("ShowRequest")]
        public async Task ShowRequestIntent(IDialogContext context, LuisResult result)
        {
            var hasUsername = context.UserData.ContainsKey(usernameKey);
            if (!hasUsername)
            {
                await context.PostAsync("First tell me your name \U0001F61C");
                context.Call(new GreetingDialog(), SetUsername);
            }

            var hasFlexibleEntity = await TryFind(result, flexibleEntity);
            var hasLeaveEntity = await TryFind(result, "Leave");

            DateTime startTime;
            DateTime? endTime;
            var hasDate = TryGetDateRange(result, true, out startTime, out endTime);
            var dateRange = new DateRangeDTO(startTime, endTime);

            if (hasFlexibleEntity && hasLeaveEntity)
            {
                await context.PostAsync("It seems like you want to view your Opted In flexible holidays and your leaves also");
                await context.Forward(new ShowRequestDialog(dateRange, ShowRequest.All), AfterShowRequestIntent, result, CancellationToken.None);

            }
            else if (hasFlexibleEntity)
            {
                await context.PostAsync("It seems like you want to view your Opted In flexible holidays only");
                await context.Forward(new ShowRequestDialog(dateRange, ShowRequest.Flexible), AfterShowRequestIntent, result, CancellationToken.None);

            }
            else if (hasLeaveEntity)
            {
                await context.PostAsync("It seems like you want to view your leaves only");
                await context.Forward(new ShowRequestDialog(dateRange, ShowRequest.Leave), AfterShowRequestIntent, result, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("It seems like you want to view your submitted requests");
                await context.Forward(new ShowRequestDialog(dateRange, ShowRequest.All), AfterShowRequestIntent, result, CancellationToken.None);

            }
        }

        private async Task AfterShowRequestIntent(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("You have viewed your requests");
            context.Done<object>(null);
        }

        private async Task AfterApplyLeaveRequest(IDialogContext context, IAwaitable<object> result)
        {
            var leaveRequest = (LeaveRequest)await result;
            var username = UserService.GetUsername(context);
            var leaveSubmitted = await leaveService.LeaveValidator(username, leaveRequest);
            if (leaveSubmitted.IsValid)
            {
                await leaveService.TakeLeave(username, leaveRequest);
                await context.PostAsync("Your request has been succesfully submitted");
                if (!string.IsNullOrWhiteSpace(leaveSubmitted.Message))
                {
                    await context.PostAsync($"Just a suggestion!! {leaveSubmitted.Message}");
                }
            }
            else
            {
                await context.PostAsync("Your request has been denied!!");
                await context.PostAsync($"{leaveSubmitted.Message}");
            }

        }

        private async Task AfterFlexibleHolidayIntent(IDialogContext context, IAwaitable<object> result)
        {
            try
            {

                if (result != null)
                {
                    var holidayId = Convert.ToInt32(await result);
                    var username = UserService.GetUsername(context);
                    var optInRequest = await flexibleHolidayService.OptInValidator(username, holidayId);
                    if (optInRequest.IsValid)
                    {
                        await flexibleHolidayService.OptUserIn(username, holidayId);
                        await context.PostAsync("You have been succesfully opted in for the flexible Holiday");
                    }
                    else
                    {
                        await context.PostAsync($"Your request has been denied because {optInRequest.Message}");
                        await context.PostAsync("Please try again");
                    }
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync("Sorry there seems an issue currently \n Please try again later");
            }
            finally
            {
                context.Done<object>(null);
            }

        }

        private async Task AfterHolidayIntent(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
        }

        private bool TryGetDateRange(LuisResult result, bool isHolidayIntent, out DateTime startDate, out DateTime? endDate)
        {
            EntityRecommendation entity;

            if (result.TryFindEntity("builtin.datetimeV2.daterange", out entity))
            {
                var pair = (IDictionary<string, object>)entity.Resolution.Values.Select(x => x).OfType<List<object>>().SelectMany(i => i).LastOrDefault();
                startDate = (Convert.ToDateTime(pair["start"].ToString())).Date;
                endDate = (Convert.ToDateTime(pair["end"].ToString())).Date;
                return true;
            }
            else if (result.TryFindEntity("builtin.datetimeV2.datetime", out entity) || result.TryFindEntity("builtin.datetimeV2.date", out entity))
            {
                entity.Type = "Date";
                var pair = (IDictionary<string, object>)entity.Resolution.Values.Select(x => x).OfType<List<object>>().SelectMany(i => i).LastOrDefault();
                startDate = (Convert.ToDateTime(pair["value"].ToString())).Date;
                endDate = null;
                return true;
            }
            else if (result.TryFindEntity("Upcoming", out entity))
            {
                startDate = DateTime.Now.Date;
                endDate = DateTime.MaxValue.Date;
                return true;
            }
            else
            {
                startDate = DateTime.Now.Date;
                endDate = DateTime.MaxValue.Date;

                if (isHolidayIntent)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


        }
        public async Task<bool> TryFind(LuisResult result, string entityToFind)
        {
            EntityRecommendation entity;
            if (result.TryFindEntity(entityToFind, out entity))
            {
                return true;
            }

            return false;
        }
        public EntityRecommendation GetEntity(LuisResult result, string entityToFind)
        {
            EntityRecommendation entity;
            result.TryFindEntity(entityToFind, out entity);
            return entity;
        }
        private async Task SetUsername(IDialogContext context, IAwaitable<object> result)
        {
            var name = await result;
            context.UserData.SetValue("username", name);
            await context.PostAsync($"Hello {name}, How may i help you");
        }

    }
}