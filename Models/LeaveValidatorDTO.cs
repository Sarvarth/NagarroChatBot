using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    public class LeaveValidatorDTO : ValidatorDTO
    {
        public bool IsFlexibleHoliday { get; set; } = false;
    }
}