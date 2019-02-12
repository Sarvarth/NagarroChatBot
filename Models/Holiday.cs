using SimpleEchoBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    public class Holiday : HolidayModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public HolidayType HolidayType { get; set; }
    }
}