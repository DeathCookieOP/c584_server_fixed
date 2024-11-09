using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using DataModel;
using AngularApp1.Server.Data;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Identity;

namespace AngularApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController(WorldContext context, IHostEnvironment environment, UserManager<AppUser> userManager) : ControllerBase
    {

        private readonly string _pathName = Path.Combine(environment.ContentRootPath, "Data/worldcities.csv");


        //async programming
        //Executes something asynchronously
        //Then comes back and gets into the point where
        //it's supposed to come back
        
        [HttpPost("Countries")] //Posting the data to the DB
        public async Task<IActionResult> ImportCountriesAsync() {
            // create a lookup dictionary containing all the countries already existing 
            // into the Database (it will be empty on first run).
            Dictionary<string, Country> countriesByName = context.Countries
                .AsNoTracking().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };

            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);

            List<WorldCitiesCSV> records = csv.GetRecords<WorldCitiesCSV>().ToList();
            foreach (WorldCitiesCSV record in records)
            {
                if (countriesByName.ContainsKey(record.country))
                {
                    continue;
                }

                Country country = new()
                {
                    Name = record.country,
                    Iso2 = record.iso2,
                    Iso3 = record.iso3
                };
                await context.Countries.AddAsync(country);
                countriesByName.Add(record.country, country);
            }

            await context.SaveChangesAsync();

            return new JsonResult(countriesByName.Count);
        }

        [HttpPost("Cities")]
        public async Task<IActionResult> ImportCitiesAsync()

        {
            Dictionary<string, Country> countries = await context.Countries//.AsNoTracking()
            .ToDictionaryAsync(c => c.Name);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null
            };
            int cityCount = 0;
            using (StreamReader reader = new(_pathName))
            using (CsvReader csv = new(reader, config))
            {
                IEnumerable<WorldCitiesCSV>? records = csv.GetRecords<WorldCitiesCSV>();
                foreach (WorldCitiesCSV record in records)
                {
                    if (!countries.TryGetValue(record.country, out Country? value))
                    {
                        Console.WriteLine($"Not found country for {record.city}");
                        return NotFound(record);
                    }

                    if (!record.population.HasValue || string.IsNullOrEmpty(record.city_ascii))
                    {
                        Console.WriteLine($"Skipping {record.city}");
                        continue;
                    }
                    City city = new()
                    {
                        Name = record.city,
                        Lat = record.lat,
                        Lag = record.lng,
                        Population = (int)record.population.Value,
                        CountryId = value.Id
                    };
                    context.Cities.Add(city);
                    cityCount++;
                }
                await context.SaveChangesAsync();
            }
            return new JsonResult(cityCount);
        }


        [HttpPost("Users")]
        public async Task<IActionResult> ImportUsersAsync() {

            //Tuplet: two values combined in parantheses
            //UserID here is name
            (string name, string email) = ("Ronny", "ronny@csun.edu");
            AppUser user = new AppUser() { 
                UserName = name,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if (await userManager.FindByEmailAsync(email) is not null) return new JsonResult("Email already registered.");

            //The password needs Upper, Lower, Numeric, And Special
            IdentityResult result = await userManager.CreateAsync(user, "Password!23");

            //we go and check if its confirmed, but for this we will just confirm it manually, unlocking the acc
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;

            await context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
