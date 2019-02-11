using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleEchoBot.Services
{
    [Serializable]
    public class FlexibleHolidayService
    {
        public async Task<ValidatorDTO> OptInValidator(int holidayId)
        {
            // Check if user has already opted in for the same
            // Check if user has opted in for 2 Flexible Holidays

            // For leaves : Check if the leave apply date is an holiday or he is an flexible holiday.
            return new ValidatorDTO();
        }

        public async Task OptUserIn(int holidayId)
        {
            // Make an entry in Azure Table Storage for the same
        }

        public async Task<bool> HasUserOptedIn(DateTime date)
        {
            // Check if user has already in for this holiday

            return false;
        }
        public async Task<List<Holiday>> GetOptedInHolidays(DateRangeDTO dateRange)
        {
            return new List<Holiday>();
        } 
    }
}