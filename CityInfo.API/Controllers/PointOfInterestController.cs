using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId:int}/PointOfInterest")]
    [ApiController]
    [Authorize]
    public class PointOfInterestController : Controller
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointOfInterestController(
            ILogger<PointOfInterestController> logger,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper
        )
        {
            this._logger = logger;
            this._cityInfoRepository = cityInfoRepository;
            this._mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            
            // var city = CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id == cityId);
            // var city = await _cityInfoRepository.GetPointsOfInterestsOfCityAsync(cityId);
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id: {cityId} wasn't found when accessing points of interest.");
                return NotFound($"City with id: {cityId} wasn't found when accessing points of interest.");
            } 

            var pointOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestsOfCityAsync(cityId);


            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity));
        }
        
        [HttpGet("{pointOfInterestId:int}")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointsOfInterest(int cityId, int pointOfInterestId)
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestsOfCityAsync(cityId, pointOfInterestId);

            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId, 
           [FromBody] PointOfInterestForCreationDto pointOfInterest
        )
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangeAsync();

            var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtAction(nameof(GetPointsOfInterest),new {
                cityId = cityId,
                pointOfInterestId = createdPointOfInterest.Id
            },createdPointOfInterest);

        }

        [HttpPut("{pointOfInterestId:int}")]
        public async Task<ActionResult> UpdatePointOfInterest(
            int cityId,
            int pointOfInterestId,
           [FromBody] PointOfInterestForCreationDto pointOfinterest
        )
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestsOfCityAsync(cityId,pointOfInterestId);

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfinterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangeAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId:int}")]
        public async Task<ActionResult> DeletePointOfInterest(
            int cityId,
            int pointOfInterestId
        )
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var pointOfInterestEntity = await _cityInfoRepository
            .GetPointOfInterestsOfCityAsync(cityId,pointOfInterestId);

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangeAsync();

            return NoContent();
        }

    }
}