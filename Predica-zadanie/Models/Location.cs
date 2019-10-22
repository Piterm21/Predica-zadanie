using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Predica_zadanie.Models
{
    public class Location
    {
        public int Id { get; set; }
        public int ExternalId { get; set; }
        public virtual ICollection<WeatherRecord> WeatherRecords { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
