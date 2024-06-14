using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(
            ICityInfoRepository cityInfoRepository,
            IMapper mapper
        )
        {
            this._cityInfoRepository = cityInfoRepository;
            this._mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
            [FromQuery] string? name,
            [FromQuery] string? searchQuery,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            if(pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (citiesToReturn, paginationMetaData) = await _cityInfoRepository.GetCitiesAsync(name,searchQuery,pageNumber,pageSize) ;

            if(citiesToReturn == null) return NotFound();
            
            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

            var result = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(citiesToReturn);
            
            return Ok(result);
        }        

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCity(
            int id,
            bool includePointOfInterest = false
        )
        {
            var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointOfInterest);

            if(cityToReturn == null) 
            {
                return NotFound();
            }
            
            if(includePointOfInterest)
            {
                var cityDto = _mapper.Map<CityDto>(cityToReturn);

                return Ok(cityDto);
            }

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn));
        }


    }
}