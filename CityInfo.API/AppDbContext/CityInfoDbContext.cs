using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.AppDbContext
{
    public class CityInfoDbContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointOfInterests { get; set; }

        public CityInfoDbContext(DbContextOptions<CityInfoDbContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
            .HasData(
                new City("New York City"){
                    Id = 1,
                    Description = "The one with that big park.",
                },
                new City("Antwerp"){
                    Id = 2,
                    Description = "The one with the cathedral that was never really finished.",
                },
                new City("Paris"){
                    Id = 3,
                    Description = "The one with that big tower.",
                }
            );

            modelBuilder.Entity<PointOfInterest>()
            .HasData(
                new PointOfInterest("Central Park"){
                    Id = 1,
                    Description = "The most visited urban park in the United States.",
                    CityId= 1,
                },
                new PointOfInterest("Antwerp - Central Park"){
                    Id = 2,
                    Description = "The most visited urban park in the United States.",
                    CityId= 2,
                },
                new PointOfInterest("PARIS - Central Park"){
                    Id = 3,
                    Description = "The most visited urban park in the United States.",
                    CityId= 3,
                }
            );
        }
    }
}