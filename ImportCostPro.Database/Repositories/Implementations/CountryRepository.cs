using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class CountryRepository : BaseRepository<Country>, ICountryRepository
    {
       public CountryRepository(AppDbContext context) : base(context)
       {
       }
    }

}