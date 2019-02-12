using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.AzureStorage.Entities
{
    public class LeaveEntity : TableEntity
    {
        public LeaveEntity(string username, DateTime date)
        {
            this.PartitionKey = username;
            this.RowKey = Guid.NewGuid().ToString();
            this.Date = date.Date;
        }
        public LeaveEntity()
        {
        }
        public DateTime Date { get; set; }
        public string LeaveId { get; set; }
        public string Reason { get; set; }
        public string Comments { get; set; }

    }
}