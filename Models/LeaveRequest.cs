using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.Models
{
    [Serializable]
    public class LeaveRequest
    {
        [Describe("Reason of your leave")]
        public string Reason { get; set; }

        [Describe("Leave Start Date")]
        public DateTime StartDate { get; set; }

        [Optional]
        [Describe("Leave End Date")]
        public DateTime? EndDate { get; set; }

        [Describe("Comments")]
        public string Comments { get; set; }


        public static IForm<LeaveRequest> BuildForm()
        {
            return new FormBuilder<LeaveRequest>()
                .Message("Welcome to Leave Request Form")
                .Field(nameof(Reason))
                .Field(nameof(StartDate), validate: async (state, value) => 
                {
                    DateTime now = DateTime.Now;
                    DateTime startDate = (DateTime)value;
                    var result = new ValidateResult() { IsValid = true, Value = startDate };
                    if (startDate < now)
                    {
                        result.IsValid = false;
                        result.Feedback = "The Leave Start Date you entered is in the past";
                    }
                    return result;
                })
                .Field(nameof(EndDate), validate: async (state, value) => 
                {
                    if (value != null)
                    {
                        DateTime now = DateTime.Now.Date;
                        DateTime endDate = (DateTime)value;
                        var result = new ValidateResult() { IsValid = true, Value = endDate.Date };
                        if (endDate < now)
                        {
                            result.IsValid = false;
                            result.Feedback = "The Leave End Date you entered is in the past";
                            return result;
                        }
                        else if(endDate.Date < state.StartDate.Date)
                        {
                            result.IsValid = false;
                            result.Feedback = "The Leave End Date cannot be less than Leave Start Date";
                            return result;
                        }
                        else
                        {
                            return result;
                        }
                        
                    }
                    else
                    {
                        var result = new ValidateResult() { IsValid = true, Value = value };
                        return result;
                    }
                })
                .AddRemainingFields()
                .Confirm("Is this information correct \n {*filled}")
                .Build();
        }
    }
}