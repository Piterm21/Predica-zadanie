using Predica_zadanie.Models;
using Predica_zadanie.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Predica_zadanie.ViewModels
{
    public class WeatherRecordsDctionaryViewModel
    {
        public class WeatherRecordListWithLocationId
        {
            public int LocationId;
            public PaginatedList<WeatherRecord> List;

            public WeatherRecordListWithLocationId(int locationId, PaginatedList<WeatherRecord> list) {
                LocationId = locationId;
                List = list;
            }
        }

        public IDictionary<string, WeatherRecordListWithLocationId> Dictionary = new Dictionary<string, WeatherRecordListWithLocationId>();
    }
}
