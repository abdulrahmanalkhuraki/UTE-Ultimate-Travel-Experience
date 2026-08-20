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
    internal class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext db) : base(db)
        {
        }
    }
}
