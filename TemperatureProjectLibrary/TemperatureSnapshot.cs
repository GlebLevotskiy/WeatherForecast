using System;
using System.Collections.Generic;
using System.Text;

namespace TemperatureProjectLibrary
{
    public class TemperatureSnapshot
    {
        public string Icon { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public DateTime Time { get; set; }
        public float TempValue { get; set; }
        public float Humidity { get; set; }
        public int Clouds { get; set; }
        public float Pressure { get; set; }
        public float WindSpeed { get; set; }
    }
}
