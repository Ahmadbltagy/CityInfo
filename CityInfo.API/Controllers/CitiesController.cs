using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            var citiesToReturn = CitiesDataStore.Current.Cities;

            if(citiesToReturn == null) return NotFound();

            return Ok(citiesToReturn);
        }        

        [HttpGet("{id:int}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if(cityToReturn == null) return NotFound();

            return Ok(cityToReturn);
        }


    }
}