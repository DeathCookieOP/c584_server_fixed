using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AngularApp1.Server.DTO
{
    public class CityDTO
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Ascii { get; set; } = null!;

        public decimal Lat { get; set; }

        public decimal Lag { get; set; }

        public int Population { get; set; }

        //made required cuz every city has a country.
        //so nullable isnt an option for us
        public required string CountryName { get; set; } 

    }
}
