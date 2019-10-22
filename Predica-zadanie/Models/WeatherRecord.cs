using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Predica_zadanie.Models
{
    public class WeatherRecord
    {
        public int Id { get; set; }
        public Location Location { get; set; }
        public string TempCurrent { get; set; }
        public string TempMin { get; set; }
        public string TempMax { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
