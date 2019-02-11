using SimpleEchoBot.Models;
using SimpleEchoBot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleEchoBot.DataWarehouse
{
    public class DataWarehouse
    {
#pragma warning disable S1104 // Fields should not have public accessibility
        public static List<Holiday> Holidays = new List<Holiday>
        {
            new Holiday
            {
                Id = 1,
                Title = "Republic Holiday",
                Description = "Indian constitution",
                Date = new DateTime(2018,1,26),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 2,
                Title = "DummyTitle1",
                Description = "DummyDescription1",
                Date = new DateTime(2018,10,26),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 3,
                Title = "DummyTitle2",
                Description = "DummyDescription2",
                Date = new DateTime(2018,6,20),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 4,
                Title = "DummyTitle3",
                Description = "DummyDescription3",
                Date = new DateTime(2018,11,21),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 5,
                Title = "DummyTitle4",
                Description = "DummyDescription4",
                Date = new DateTime(2018,4,16),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 6,
                Title = "DummyTitle5",
                Description = "DummyDescription5",
                Date = new DateTime(2018,1,2),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 7,
                Title = "Independence Holiday",
                Description = "Indian Freedom",
                Date = new DateTime(2018,1,26),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 8,
                Title = "DummyTitle6",
                Description = "DummyDescription6",
                Date = new DateTime(2018,5,6),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 9,
                Title = "DummyTitle7",
                Description = "DummyDescription7",
                Date = new DateTime(2018,6,2),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 10,
                Title = "DummyTitle8",
                Description = "DummyDescription8",
                Date = new DateTime(2018,3,31),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 11,
                Title = "DummyTitle9",
                Description = "DummyDescription9",
                Date = new DateTime(2018,4,1),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 12,
                Title = "DummyTitle10",
                Description = "DummyDescription10",
                Date = new DateTime(2018,1,29),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 13,
                Title = "Republic Holiday",
                Description = "Indian constitution",
                Date = new DateTime(2019,1,26),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 14,
                Title = "DummyTitle1",
                Description = "DummyDescription1",
                Date = new DateTime(2019,10,26),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 15,
                Title = "DummyTitle2",
                Description = "DummyDescription2",
                Date = new DateTime(2019,6,20),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 16,
                Title = "DummyTitle3",
                Description = "DummyDescription3",
                Date = new DateTime(2019,11,21),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 17,
                Title = "DummyTitle4",
                Description = "DummyDescription4",
                Date = new DateTime(2019,4,16),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 18,
                Title = "DummyTitle5",
                Description = "DummyDescription5",
                Date = new DateTime(2019,1,2),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 19,
                Title = "Independence Holiday",
                Description = "Indian Freedom",
                Date = new DateTime(2019,1,26),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 20,
                Title = "DummyTitle6",
                Description = "DummyDescription6",
                Date = new DateTime(2019,5,6),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 21,
                Title = "DummyTitle7",
                Description = "DummyDescription7",
                Date = new DateTime(2019,6,2),
                HolidayType = HolidayType.Public
            },
            new Holiday
            {
                Id = 22,
                Title = "DummyTitle8",
                Description = "DummyDescription8",
                Date = new DateTime(2019,3,31),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 23,
                Title = "DummyTitle9",
                Description = "DummyDescription9",
                Date = new DateTime(2019,4,1),
                HolidayType = HolidayType.Flexible
            },
            new Holiday
            {
                Id = 24,
                Title = "DummyTitle10",
                Description = "DummyDescription10",
                Date = new DateTime(2019,1,29),
                HolidayType = HolidayType.Public
            },
        };
#pragma warning restore S1104 // Fields should not have public accessibility
    }
}