using Horizons.Data;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Horizons.Services.Core
{
    public class TerrainService : ITerrainService
    {
        private readonly HorizonDbContext _context;

        public TerrainService(HorizonDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TerrainViewModel>> GetAllTerrainsAsync()
        {
            return await _context
                .Terrains
                .AsNoTracking()
                .Select(t => new TerrainViewModel
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();
        }
    }
}
