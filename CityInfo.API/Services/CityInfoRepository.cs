using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.AppDbContext;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoDbContext _context;
        public CityInfoRepository(CityInfoDbContext context)
        {
            _context = context;
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId);
            if(city != null)
            {
                city.PointOfInterest.Add(pointOfInterest);
            }
            
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context
            .Cities
            .AnyAsync(c => c.Id == cityId); 
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context
                .Cities
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
      
        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(
            string? name,
            string? searchQuery,
            int pageNumber,
            int pageSize
        )
        {
            var collection = _context.Cities as IQueryable<City>; //defered Execution
        
            if(!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection
                .Where(c => c.Name.ToLower() == name.ToLower());
            }

            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection
                .Where(c => 
                c.Name.ToLower().Contains(searchQuery.ToLower())
                || (c.Description != null && c.Description.ToLower().Contains(searchQuery.ToLower()))
                );
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetaData(totalItemCount,pageSize,pageNumber);

           var collectionToReturn =   await collection
            .OrderBy(c => c.Name)
            .Skip(pageSize * (pageNumber-1))
            .Take(pageSize)
            .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest = false)
        {
            if(includePointsOfInterest)
            {
                return await _context
                .Cities
                .Include(c => c.PointOfInterest)
                .FirstOrDefaultAsync(c => c.Id == cityId);
            }
            return await _context
            .Cities
            .FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<PointOfInterest?> GetPointOfInterestsOfCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context
            .PointOfInterests
            .FirstOrDefaultAsync(p => p.Id == pointOfInterestId && p.CityId == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsOfCityAsync(int cityId)
        {
            return await _context
            .PointOfInterests
            .Where(p => p.CityId == cityId)
            .ToListAsync();
        }

        public async Task<bool> SaveChangeAsync()
        {
            return (await _context.SaveChangesAsync() >= 0); 
        }
    }
}