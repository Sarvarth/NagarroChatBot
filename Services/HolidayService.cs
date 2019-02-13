using SimpleEchoBot.Models;
using SimpleEchoBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleEchoBot.DataWarehouse;
using System.Threading.Tasks;

namespace SimpleEchoBot.Services
{
    [Serializable]
    public class HolidayService
    {
        public List<Holiday> GetHolidays(bool getFlexibleHolidaysOnly, DateRangeDTO dateRange)
        {
            var holidays = DataWarehouse.DataWarehouse.Holidays
                                        .Where(x => x.Date.Date >= dateRange.StartDate.Date);
                                        
            if(dateRange.EndDate != null)
            {
                holidays = holidays.Where(x => x.Date.Date <= dateRange.EndDate?.Date);
            }
            else
            {
                holidays = holidays.Where(x => x.Date.Date == dateRange.StartDate.Date);
            }

            if (getFlexibleHolidaysOnly)
            {
                holidays = holidays
                                .Where(x => x.HolidayType == HolidayType.Flexible)
                                .ToList();
            }
            else
            {
                holidays = holidays.ToList();

            }

            return (List<Holiday>)holidays;
        }

        public async Task<LeaveValidatorDTO> IsAPublicHoliday(DateTime date)
        {
            var holiday = DataWarehouse.DataWarehouse.Holidays
                                                     .FirstOrDefault(x => x.Date.Date == date.Date);

            if(holiday == null)
            {
                return new LeaveValidatorDTO
                {
                    IsValid = false
                };
            }
            else
            {
                // A flexible holiday is a valid holiday for leave
                if(holiday.HolidayType == HolidayType.Flexible)
                {
                    return new LeaveValidatorDTO
                    {
                        IsValid = false,
                        IsFlexibleHoliday = true,
                        Message = $"{date.ToString("dd/MM/yyyy")} is {holiday.Title} which is a Flexible Holiday"
                    };
                }
                return new LeaveValidatorDTO
                {
                    IsValid = true,
                    Message = $"{date.ToString("dd/MM/yyyy")} is {holiday.Title} which is a Public Holiday"
                };
            }
        }

        public async Task<Holiday> GetHolidayAsync(int holidayId)
        {
            return DataWarehouse.DataWarehouse.Holidays.FirstOrDefault(x => x.Id == holidayId);
        }
    }
}