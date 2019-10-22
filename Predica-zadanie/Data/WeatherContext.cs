using Microsoft.EntityFrameworkCore;
using Predica_zadanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Predica_zadanie.Data
{
    public class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options) 
            : base(options)
        {
        }

        public DbSet<WeatherRecord> WeatherRecords { get; set; }
        public DbSet<Location> Locations { get; set; }
    }
}
