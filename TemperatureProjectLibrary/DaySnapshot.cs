using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemperatureProjectLibrary
{
    public class DaySnapshot
    {
        public string City { get; set; }
        public string Region { get; set; }
        public DateTime Date { get; set; } 
        public IEnumerable<TemperatureSnapshot> Snapshots { get; set; } = Enumerable.Empty<TemperatureSnapshot>();
    }
}
