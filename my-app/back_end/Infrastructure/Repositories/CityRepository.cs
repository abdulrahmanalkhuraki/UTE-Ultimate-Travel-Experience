using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class CityRepository : GenericRepository<City>, ICityRepository
    {
        public CityRepository(AppDbContext db) : base(db)
        {
        }
    }
}
