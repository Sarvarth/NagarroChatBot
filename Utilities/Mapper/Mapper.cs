using SimpleEchoBot.AzureStorage.Entities;
using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Utilities.Mapper
{
    public class Mapper
    {
        public static List<Holiday> MapHolidayEntityToHoliday(List<FlexibleHolidayEntity> holidayEntities)
        {
            List<Holiday> holidayList = new List<Holiday>();
            foreach (var entity in holidayEntities)
            {
                holidayList.Add(new Holiday
                {
                    Id = Convert.ToInt32(entity.RowKey),
                    Date = entity.Date.Date,
                    Description = entity.Description,
                    HolidayType = Models.Enums.HolidayType.Flexible,
                    Title = entity.Title
                });
            }

            return holidayList;
        }
        public static List<LeaveRequest> MapLeaveEntityToLeaveRequest(List<LeaveEntity> leaveEntities)
        {
            List<LeaveRequest> leaveRequestList = new List<LeaveRequest>();
            var leavesIds = leaveEntities.Select(x => x.LeaveId).Distinct();
            foreach (var leaveId in leavesIds)
            {
                var firstLeave = leaveEntities.FirstOrDefault(x => x.LeaveId == leaveId);
                var leaveCountWithSameId = leaveEntities.Count(x => x.LeaveId == leaveId);
                DateTime? endDate;
                if (leaveCountWithSameId == 1)
                {
                    endDate = null;
                }
                else
                {
                    endDate = Convert.ToDateTime(leaveEntities.LastOrDefault(x => x.LeaveId == leaveId).Date);
                }

                leaveRequestList.Add(new LeaveRequest
                {
                    Id = leaveId,
                    Comments = firstLeave.Comments,
                    Reason = firstLeave.Reason,
                    StartDate = Convert.ToDateTime(firstLeave.Date),
                    EndDate = endDate
                });
            }

            return leaveRequestList;
        }
    }
}