using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class City
    {
        public int Id { get; set;}
        public string Name { get; set;}
        public string Description { get; set; }
        public ICollection<PointOfInterest> MyProperty { get; set; } = new List<PointOfInterest>();

        public City(string name)
        {
            Name = name;
        }
    }

}