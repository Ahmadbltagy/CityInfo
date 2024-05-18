using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId:int}/PointOfInterest")]
    [ApiController]
    public class PointOfInterestController : Controller
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id == cityId);

            if(city == null) return NotFound($"There is no city with this id {cityId}");

            var pointsOfInterest = city.PointsOfInterest;

            if(pointsOfInterest == null) return NotFound($"city with id: {cityId} don't have any pointsOfInterest");


            return Ok(pointsOfInterest);
        }
        
        [HttpGet("{pointOfInterestId:int}")]
        public ActionResult<PointOfInterestDto> GetPointsOfInterest(int cityId,int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities
            .FirstOrDefault(c=>c.Id == cityId);

            if(city == null) return NotFound($"No City with this id {cityId}");

            var pointsOfInterest = city.PointsOfInterest.FirstOrDefault(p=>p.Id == pointOfInterestId);

            if(pointsOfInterest == null) return NotFound($"No Point of inteterest with this id: {pointOfInterestId}");

            return Ok(pointsOfInterest);

        }

    }
}