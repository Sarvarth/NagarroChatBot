using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleEchoBot.Services
{
    [Serializable]
    public class LeaveService
    {
        private HolidayService _holidayService;
        public LeaveService()
        {
            this._holidayService = new HolidayService();
        }
        public async Task<ValidatorDTO> LeaveValidator(LeaveRequest leaveRequest)
        {
            // For leaves : Check if the leave apply date is an opted in flexible holiday.
            // ToDo

            if (leaveRequest.EndDate == null)
            {
                var holiday = await _holidayService.IsAPublicHoliday(leaveRequest.StartDate);
                if (holiday.IsValid)
                {
                    return new ValidatorDTO
                    {
                        IsValid = false,
                        Message = holiday.Message
                    };
                }
                else
                {
                    return new ValidatorDTO
                    {
                        IsValid = true,
                        Message = holiday.Message
                    };
                }
            }
            else
            {
                string suggestion = "";
                for(DateTime date = leaveRequest.StartDate.Date; date.Date <= leaveRequest.EndDate?.Date; date = date.AddDays(1))
                {
                    var holiday = await _holidayService.IsAPublicHoliday(date);
                    if (holiday.IsFlexibleHoliday)
                    {
                        suggestion = suggestion + " \n " + holiday.Message;
                    }
                    if (holiday.IsValid)
                    {
                        return new ValidatorDTO
                        {
                            IsValid = false,
                            Message = suggestion
                        };
                    }
                    
                }
                return new ValidatorDTO
                {
                    IsValid = true,
                    Message = suggestion
                };
            }
        }
        public async Task TakeLeave(LeaveRequest leaveRequest)
        {
            // ToDo
            // Make an entry in Azure Table Storage for the same
            
        }

        public async Task<bool> HasUserAppliedForLeave(DateTime dateTime)
        {
            // ToDo
            return false;
        }
        public async Task<List<LeaveRequest>> GetLeaveAsync(DateRangeDTO dateRange)
        {
            // ToDo
            return new List<LeaveRequest>();
        }
    }
}