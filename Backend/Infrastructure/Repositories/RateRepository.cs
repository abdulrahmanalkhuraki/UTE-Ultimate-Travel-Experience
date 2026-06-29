using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class RateRepository : GenericRepository<Rate>, IRateRepository
    {
        public RateRepository(AppDbContext db) : base(db)
        {
        }
    }
}
