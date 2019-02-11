using AdaptiveCards;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace SimpleEchoBot.Utilities
{
    public class AdaptiveCardJsonManager
    {
        public static IEnumerable<Attachment> CreateCards<T>(List<T> cardList)
        {
            List<Attachment> attachments = new List<Attachment>();
            string pathToFiles = GetPathToFile(typeof(T));
            foreach (var card in cardList)
            {
                string adaptiveCardJson = FillAdaptiveCard<T>(card, pathToFiles);
                AdaptiveCard adaptiveCard = JsonConvert.DeserializeObject<AdaptiveCard>(adaptiveCardJson);

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = adaptiveCard
                };
                attachments.Add(attachment);
            }

            return attachments;
        }
        private static string FillAdaptiveCard<T>(T card, string pathToFiles)
        {
            if (typeof(T) == typeof(Holiday))
            {
                Holiday holiday = card as Holiday;
                StringBuilder adaptiveCardJsonToModify = new StringBuilder(File.ReadAllText(pathToFiles));
                foreach (var prop in card.GetType().GetProperties())
                {
                    adaptiveCardJsonToModify.Replace("{" + prop.Name + "}", prop.GetValue(holiday).ToString());
                }
                adaptiveCardJsonToModify.Replace("{DayOfWeek}", holiday.Date.DayOfWeek.ToString()).Replace("{Date.Date}", holiday.Date.ToString("dd/MM/yyyy"));

                return adaptiveCardJsonToModify.ToString();
            }
            else
            {
                LeaveRequest leaveRequest = card as LeaveRequest;
                StringBuilder adaptiveCardJsonToModify = new StringBuilder(File.ReadAllText(pathToFiles));
                foreach (var prop in leaveRequest.GetType().GetProperties())
                {
                    adaptiveCardJsonToModify.Replace("{" + prop.Name + "}", prop.GetValue(leaveRequest).ToString());
                }
                adaptiveCardJsonToModify.Replace("{StartDate.DayOfWeek}", leaveRequest.StartDate.DayOfWeek.ToString()).Replace("{StartDate.Date}", leaveRequest.StartDate.ToString("dd/MM/yyyy"));
                if (leaveRequest.EndDate != null)
                {
                    adaptiveCardJsonToModify = adaptiveCardJsonToModify.Append("\n To \n");
                    adaptiveCardJsonToModify.Replace("{End.DayOfWeek}", leaveRequest.EndDate?.DayOfWeek.ToString()).Replace("{EndDate.Date}", leaveRequest.EndDate?.ToString("dd/MM/yyyy"));

                }

                return adaptiveCardJsonToModify.ToString();
            }
        }
        private static string GetPathToFile(Type fileType)
        {
            string pathToFile = "";

            if(fileType == typeof(Holiday))
            {
                pathToFile = "~/Utilities/AdaptiveCards/HolidayCard.json";
            }
            else if(fileType == typeof(HolidayModel))
            {
                pathToFile = "~/Utilities/AdaptiveCards/FlexibleHolidayCard.json";
            }
            else
            {
                pathToFile = "~/Utilities/AdaptiveCards/LeaveCard.json";
            }

            return HttpContext.Current.Server.MapPath(pathToFile);
        }
    }
}