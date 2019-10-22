using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Predica_zadanie.Data;
using Predica_zadanie.Models;
using Predica_zadanie.Utilities;
using Predica_zadanie.ViewModels;
using static Predica_zadanie.ViewModels.WeatherRecordsDctionaryViewModel;

namespace Predica_zadanie.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly WeatherContext weatherContext;

        public HomeController(WeatherContext context)
        {
            weatherContext = context;
        }

        public async Task<IActionResult> Index()
        {
            WeatherRecordsDctionaryViewModel weatherRecordsViewModel = new WeatherRecordsDctionaryViewModel();
            var locations = weatherContext.Locations.ToArray();

            foreach (Location location in locations) {
                weatherRecordsViewModel.Dictionary.Add(
                    location.Name,
                    new WeatherRecordListWithLocationId(
                        location.Id,
                        await PaginatedList<WeatherRecord>.CreateAsync(
                            weatherContext.WeatherRecords
                                .OrderByDescending(w => w.CreateDateTime)
                                .Where(w => w.Location.Id == location.Id), 1, 10)
                        )
                );
            }
            
            return View(weatherRecordsViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
