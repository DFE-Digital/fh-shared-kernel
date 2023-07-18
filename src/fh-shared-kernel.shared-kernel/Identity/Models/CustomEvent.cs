using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.SharedKernel.Identity.Models
{
    //generic wrapper for Azure EventGrid custom event
    public class CustomEvent<T>
    {

        public string Id { get; private set; }

        public string EventType { get; set; } = default!;

        public string Subject { get; set; } = default!;

        public string EventTime { get; private set; }

        public T Data { get; set; } = default!;

        public CustomEvent()
        {
            Id = Guid.NewGuid().ToString();

            DateTime localTime = DateTime.Now;
            DateTimeOffset localTimeAndOffset =
                new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
            EventTime = localTimeAndOffset.ToString("o");
        }
    }
}
