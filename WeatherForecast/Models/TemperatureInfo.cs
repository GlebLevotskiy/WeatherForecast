using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemperatureProjectLibrary;

namespace WeatherForecast.Models
{
    public class TemperatureInfo
    {
        public TemperatureSnapshot Snapshot { get; set; }
        public IList<DaySnapshot> DaySnapshots { get; set; }
    }
}
