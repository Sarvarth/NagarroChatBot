using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace SimpleEchoBot.AzureStorage.Entities
{
    public class FlexibleHolidayEntity : TableEntity
    {
        public FlexibleHolidayEntity(string username, int holidayId)
        {
            this.PartitionKey = username;
            this.RowKey = holidayId.ToString();
        }
        public FlexibleHolidayEntity()
        {
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        //public string Username { get; set; }
        //public int HolidayId { get; set; }

    }
}