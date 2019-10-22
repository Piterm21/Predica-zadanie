using Microsoft.AspNetCore.Identity;
using Predica_zadanie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Predica_zadanie.Data
{
    public static class DbInitializer
    {
        private const int lublinId = 765876;
        private const int warszawaId = 6695624;
        private const int gdanskId = 7531002;

        public async static Task Initialize(WeatherContext weatherContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            weatherContext.Database.EnsureCreated();

            if (!weatherContext.Locations.Any())
            {
                // For test purposes city id's are hard coded, in real situation I'd probably create periodic action to fetch service's file with ids and parse
                // into the database, and then pull the id from the database base on locations name/country
                var locations = new Location[]
                {
                    new Location{ExternalId=lublinId, Name="Lublin", Country="Poland"},
                    new Location{ExternalId=warszawaId, Name="Warszawa", Country="Poland"},
                    new Location{ExternalId=gdanskId, Name="Gdańsk", Country="Poland"}
                };

                foreach (Location location in locations) {
                    weatherContext.Locations.Add(location);
                }

                weatherContext.SaveChanges();
            }

            string adminRoleName = "Administrator";
            IdentityRole adminRole = await roleManager.FindByNameAsync(adminRoleName);

            if (adminRole == null) {
                adminRole = new IdentityRole { Name=adminRoleName };
                await roleManager.CreateAsync(adminRole);
            }

            string adminEmail = "admin@admin.com";
            var adminUsers = await userManager.GetUsersInRoleAsync(adminRole.Name);

            if (adminUsers.Count() == 0) {
                var user = new IdentityUser { Email=adminEmail, UserName=adminEmail, EmailConfirmed=true };
                var userCreateResult = await userManager.CreateAsync(user, "@Dmin12aads");

                if (userCreateResult.Succeeded) {
                    await userManager.AddToRoleAsync(user, adminRole.Name);
                }
            }
        }
    }
}
