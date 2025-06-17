using Horizons.Web.ViewModels;

namespace Horizons.Services.Core.Contracts
{
    public interface ITerrainService
    {
        Task<IEnumerable<TerrainViewModel>> GetAllTerrainsAsync();
    }
}
