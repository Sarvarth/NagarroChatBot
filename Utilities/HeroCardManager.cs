using Microsoft.Bot.Connector;
using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Utilities
{
    public class HeroCardManager
    {
        public static IEnumerable<Attachment> CreateFlexibleHolidayCards(List<Holiday> holidays)
        {
            List<Attachment> attachments = new List<Attachment>();
            foreach (var holiday in holidays)
            {
                var heroCard = FillHeroCard(holiday);
                attachments.Add(heroCard);
            }

            return attachments;
        }
        private static Attachment FillHeroCard(Holiday holiday)
        {
            var heroCard = new HeroCard
            {
                Title = holiday.Title,
                Text = holiday.Date.ToString("dd/MM/yyyy"),
                Images = new List<CardImage> { new CardImage("https://gothinkbig.co.uk/wp-content/uploads/2015/06/holiday.png") },
                Subtitle = holiday.Description,
                Buttons = new List<CardAction> {
                    new CardAction
                    {
                        Type= ActionTypes.PostBack,
                        DisplayText = "Congratulations you have opted in for a flexible holiday",
                        Title = "Opt In",
                        Value = $"Opt In {holiday.Id}"
                    }
                }
            };

            return heroCard.ToAttachment();
        }
    }
}