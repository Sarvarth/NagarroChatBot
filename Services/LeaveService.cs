using Microsoft.WindowsAzure.Storage.Table;
using SimpleEchoBot.AzureStorage;
using SimpleEchoBot.AzureStorage.Entities;
using SimpleEchoBot.Models;
using SimpleEchoBot.Utilities.Mapper;
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
        private const string userLeavesTable = "userleaves";
        private HolidayService holidayService;
        private FlexibleHolidayService flexibleHolidayService;

        public LeaveService()
        {
            holidayService = new HolidayService();
            flexibleHolidayService = new FlexibleHolidayService();
        }

        public async Task<ValidatorDTO> LeaveValidator(string username, LeaveRequest leaveRequest)
        {
            // Checking if the leave apply date is an opted in flexible holiday.
            var dateRangeDTO = new DateRangeDTO(leaveRequest.StartDate, leaveRequest.EndDate);

            var optedInHolidays = await flexibleHolidayService.GetOptedInHolidays(username, dateRangeDTO);
            if (optedInHolidays.Any())
            {
                return new ValidatorDTO
                {
                    IsValid = false,
                    Message = $"You have opted in for a flexible holiday on {optedInHolidays.First().Date.ToString("dd/MM/yyyy")}"
                };
            }

            // No day in between the LeaveDateRange can be a Public Holiday
            var publicHoliday = await PublicHolidayValidator(leaveRequest);

            return publicHoliday;
        }
        public async Task TakeLeave(string username, LeaveRequest leaveRequest)
        {
            var tableManager = new AzureTableManager(userLeavesTable);
            // Make an entry in Azure Table Storage for the same
            // Using BatchOperation to add these bulk leaveRequests
            leaveRequest.Id = Guid.NewGuid().ToString();

            if (leaveRequest.EndDate == null)
            {
                var newLeaveEntity = new LeaveEntity(username, leaveRequest.StartDate);

                newLeaveEntity.LeaveId = leaveRequest.Id;
                newLeaveEntity.Reason = leaveRequest.Reason;
                newLeaveEntity.Comments = leaveRequest.Comments;

                await tableManager.InsertEntity<LeaveEntity>(newLeaveEntity, true);
            }
            else
            {
                TableBatchOperation batchOperation = new TableBatchOperation();
                for (DateTime date = leaveRequest.StartDate.Date; date.Date <= leaveRequest.EndDate?.Date; date = date.AddDays(1))
                {
                    var newLeaveEntity = new LeaveEntity(username, date);

                    newLeaveEntity.LeaveId = leaveRequest.Id;
                    newLeaveEntity.Reason = leaveRequest.Reason;
                    newLeaveEntity.Comments = leaveRequest.Comments;

                    batchOperation.Insert(newLeaveEntity);
                }

                await tableManager.BatchInsert(batchOperation);
            }

        }
        public async Task<List<LeaveRequest>> GetLeaveAsync(string username, DateRangeDTO dateRange)
        {
            var tableManager = new AzureTableManager(userLeavesTable);

            string leaveDateQuery;

            if (dateRange.EndDate == null)
            {
                leaveDateQuery = TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username),
                                        TableOperators.And,
                                        TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.Equal, dateRange.StartDate)
                                        );
            }
            else
            {
                leaveDateQuery = TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username),
                                        TableOperators.And,
                                        TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.GreaterThanOrEqual, dateRange.StartDate),
                                        TableOperators.And,
                                        TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.LessThanOrEqual, (DateTime)dateRange.EndDate))
                                        );
            }

            var leaveEntities = await tableManager.RetrieveEntity<LeaveEntity>(leaveDateQuery);
            var leaveRequests = Mapper.MapLeaveEntityToLeaveRequest(leaveEntities);

            return leaveRequests;
        }

        private async Task<ValidatorDTO> PublicHolidayValidator(LeaveRequest leaveRequest)
        {
            if (leaveRequest.EndDate == null)
            {
                var holiday = await holidayService.IsAPublicHoliday(leaveRequest.StartDate);
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
                for (DateTime date = leaveRequest.StartDate.Date; date.Date <= leaveRequest.EndDate?.Date; date = date.AddDays(1))
                {
                    var holiday = await holidayService.IsAPublicHoliday(date);
                    // User can take a leave on a Flexible Holiday, and is shown as a suggestion to user
                    if (holiday.IsFlexibleHoliday)
                    {
                        suggestion = suggestion + " \n " + holiday.Message;
                    }
                    if (holiday.IsValid)
                    {
                        return new ValidatorDTO
                        {
                            IsValid = false,
                            Message = $"You cannot take a leave on a public holiday which is on {date.ToString("dd/MM/yyyy")}"
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
    }
}