using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    [Serializable]
    public class DateRangeDTO
    {
        public DateRangeDTO(DateTime startTime, DateTime? endTime)
        {
            StartDate = startTime;
            EndDate = endTime;
        }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
    }
}