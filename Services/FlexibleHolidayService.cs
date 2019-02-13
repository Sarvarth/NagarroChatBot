using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SimpleEchoBot.AzureStorage;
using SimpleEchoBot.AzureStorage.Entities;
using SimpleEchoBot.Models;
using SimpleEchoBot.Utilities.Mapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace SimpleEchoBot.Services
{
    [Serializable]
    public class FlexibleHolidayService
    {
        //private AzureTableManager tableManager;
        private const string userFlexibleHolidaysTable = "userflexibleholidays";
        private HolidayService holidayService;

        public FlexibleHolidayService()
        {
            holidayService = new HolidayService();
        }

        public async Task<ValidatorDTO> OptInValidator(string username, int holidayId)
        {
            var tableManager = new AzureTableManager(userFlexibleHolidaysTable);

            // Check if user has already opted in for the same
            var hasUserOptedIn = await HasUserOptedIn(username, holidayId);
            if (hasUserOptedIn)
            {
                return new ValidatorDTO
                {
                    IsValid = false,
                    Message = "User has already opted in for this holiday"
                };
            }

            // Check if user has opted in for 2 Flexible Holidays
            var optedInHolidayQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username);
            var optedInHolidayList = await tableManager.RetrieveEntity<FlexibleHolidayEntity>(optedInHolidayQuery);
            if (optedInHolidayList.Count == 2)
            {
                return new ValidatorDTO
                {
                    IsValid = false,
                    Message = "User has already opted for 2 holidays"
                };
            }
            // For leaves : Check if the leave apply date is an holiday or he is an flexible holiday.
            return new ValidatorDTO
            {
                IsValid = true
            };
        }
        public async Task OptUserIn(string username, int holidayId)
        {
            var tableManager = new AzureTableManager(userFlexibleHolidaysTable);

            // Make an entry in Azure Table Storage for the same
            var flexibleHoliday = await holidayService.GetHolidayAsync(holidayId);

            var newFlexibleHoliday = new FlexibleHolidayEntity(username, holidayId);
            newFlexibleHoliday.Date = flexibleHoliday.Date;
            newFlexibleHoliday.Description = flexibleHoliday.Description;
            newFlexibleHoliday.Title = flexibleHoliday.Title;

            await tableManager.InsertEntity(newFlexibleHoliday, true);
        }
        public async Task<List<Holiday>> GetOptedInHolidays(string username, DateRangeDTO dateRange)
        {
            var tableManager = new AzureTableManager(userFlexibleHolidaysTable);

            string optInHolidayDateQuery;

            if (dateRange.EndDate == null)
            {
                optInHolidayDateQuery = TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username),
                                        TableOperators.And,
                                        TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.Equal, dateRange.StartDate)
                                        );
            }
            else
            {
                optInHolidayDateQuery = TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username),
                                        TableOperators.And,
                                        TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.GreaterThanOrEqual, dateRange.StartDate),
                                        TableOperators.And,
                                        TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.LessThanOrEqual, (DateTime)dateRange.EndDate))
                                        );
            }

            var holidayEntities = await tableManager.RetrieveEntity<FlexibleHolidayEntity>(optInHolidayDateQuery);
            var holidayList = Mapper.MapHolidayEntityToHoliday(holidayEntities);

            return holidayList;
        }
        private async Task<bool> HasUserOptedIn(string username, int holidayId)
        {
            var tableManager = new AzureTableManager(userFlexibleHolidaysTable);

            // Check if user has already opted in for this holiday
            return await tableManager.DoesEntityExist(username, holidayId.ToString());
        }

    }
}