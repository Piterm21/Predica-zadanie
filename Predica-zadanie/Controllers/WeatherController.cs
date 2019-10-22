using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Predica_zadanie.Models;
using Predica_zadanie.Data;
using Microsoft.AspNetCore.Authorization;
using Predica_zadanie.Utilities;
using static Predica_zadanie.ViewModels.WeatherRecordsDctionaryViewModel;

namespace Predica_zadanie.Controllers
{
    [Authorize(Policy = "RequiresAdmin")]
    public class WeatherController : Controller
    {
        class ListResponse
        {
            public class Record
            {
                public class Weather
                {
                    [JsonPropertyName("temp")]
                    public float Temp { get; set; }
                    [JsonPropertyName("temp_min")]
                    public float TempMin { get; set; }
                    [JsonPropertyName("temp_max")]
                    public float TempMax { get; set; }
                }

                public class WeatherDescription
                {
                    [JsonPropertyName("description")]
                    public string Description { get; set; }
                }

                [JsonPropertyName("weather")]
                public WeatherDescription[] WeatherData { get; set; }
                [JsonPropertyName("main")]
                public Weather Main { get; set; }
                [JsonPropertyName("id")]
                public int Id { get; set; }
            }

            [JsonPropertyName("list")]
            public Record[] List { get; set; }
        }

        public class AddedWeatherRecord
        {
            public int LocationId { get; set; }
            public string Description { get; set; }
            public string CreateDateTime { get; set; }
            public string TempCurrent { get; set; }
            public string TempMax { get; set; }
            public string TempMin { get; set; }

            public AddedWeatherRecord (WeatherRecord weatherRecord) {
                LocationId = weatherRecord.Location.Id;
                Description = weatherRecord.Description;
                CreateDateTime = weatherRecord.CreateDateTime.ToString();
                TempCurrent = weatherRecord.TempCurrent;
                TempMax = weatherRecord.TempMax;
                TempMin = weatherRecord.TempMin;
            }
        }

        private readonly IConfiguration configuration;
        private readonly WeatherContext weatherContext;

        public WeatherController (IConfiguration configuration, WeatherContext context)
        {
            this.configuration = configuration;
            weatherContext = context;
        }

        [HttpGet("api/weatherRefresh")]
        public async Task<IActionResult> Refresh()
        {
            IDictionary<int, Location> locations;
            string jsonString;

            Uri uri = new Uri("https://api.openweathermap.org/data/2.5/group?id=765876,6695624,7531002&units=metric&appid=" + configuration["Weather:ServiceApiKey"]);
            
            using (HttpClient httpClient = new HttpClient()) {
                jsonString = await httpClient.GetStringAsync(uri);
                locations = weatherContext.Locations.ToDictionary(x => x.ExternalId);
            }

            ListResponse listResponse = JsonSerializer.Deserialize<ListResponse>(jsonString);

            List<AddedWeatherRecord> weatherRecords = new List<AddedWeatherRecord>();

            DateTime datetimeNow = DateTime.Now;

            foreach (ListResponse.Record pulledWeatherRecord in listResponse.List) {
                WeatherRecord weatherRecord = new WeatherRecord {
                    Description = pulledWeatherRecord.WeatherData[0].Description,
                    TempCurrent = pulledWeatherRecord.Main.Temp.ToString(),
                    TempMax = pulledWeatherRecord.Main.TempMax.ToString(),
                    TempMin = pulledWeatherRecord.Main.TempMin.ToString(),
                    Location = locations[pulledWeatherRecord.Id],
                    CreateDateTime = datetimeNow
                };

                weatherContext.WeatherRecords.Add(weatherRecord);
                weatherRecords.Add(new AddedWeatherRecord(weatherRecord));
            }

            weatherContext.SaveChanges();

            return Ok(JsonSerializer.Serialize(weatherRecords.ToArray()));
        }

        [HttpGet("api/weatherRecord/index/{pageNumber:int}/{locationId:int}")]
        public async Task<IActionResult> Index(int pageNumber, int locationId) 
        {
            var weatherRecordsViewModel = new WeatherRecordListWithLocationId(
                locationId, 
                await PaginatedList<WeatherRecord>.CreateAsync(
                weatherContext.WeatherRecords
                    .OrderByDescending(w => w.CreateDateTime)
                    .Where(w => w.Location.Id == locationId), pageNumber, 10
                )
            );

            var html = await this.RenderViewAsync("PaginatedWeatherRecordList", weatherRecordsViewModel, true);
            var result = new { LocationId=locationId, Html=html };

            return Ok(JsonSerializer.Serialize(result));
        }
    }
}