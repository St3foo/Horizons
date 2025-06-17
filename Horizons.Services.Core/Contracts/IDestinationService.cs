using Horizons.Web.ViewModels;

namespace Horizons.Services.Core.Contracts
{
    public interface IDestinationService
    {
        Task<IEnumerable<AllDestinationsViewModel>> GetAllDestinationsAsync(string? userId);

        Task<DetailsDestinationViewModel> GetDestinationDetailsAsync(int? id , string? userId);

        Task AddDestinationAsync(string userId, AddDestinationViewModel model);

        Task<EditDestinationViewModel?> GetDestinationForEdit(string userId, int? destId);

        Task<bool> AddEditedInfoAsync(string userId, EditDestinationViewModel model);

        Task<DeleteViewModel?> DeleteInfoModelAsync(string userId, int? destId);

        Task<bool> DeleteConfirmedAsync(string userId, DeleteViewModel model);

        Task<IEnumerable<FavoritesViewModel>?> GetFavoritesListAsync(string userId);

        Task<bool> AddToFavoritesAsync(string userId, int id);

        Task<bool> RemoveFromFavoritesAsync(string userId, int id);

    }
}
