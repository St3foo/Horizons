using System.Globalization;
using Horizons.Data;
using Horizons.Data.Models;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Horizons.GCommon.ValidationConstatnts.Destination;

namespace Horizons.Services.Core
{
    public class DestinationService : IDestinationService
    {
        private readonly HorizonDbContext _context;
        private readonly UserManager<IdentityUser> _user;

        public DestinationService(HorizonDbContext horizonDb, UserManager<IdentityUser> user)
        {
            _context = horizonDb;
            _user = user;
        }

        public async Task AddDestinationAsync(string userId, AddDestinationViewModel model)
        {
            IdentityUser? user = await _user.FindByIdAsync(userId);
            Terrain? terrain = await _context.Terrains.FindAsync(model.TerrainId);
            bool isDateValid = DateTime.TryParseExact(model.PublishedOn, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

            if ((user != null) && (terrain != null) && (isDateValid))
            {

                Destination destination = new Destination
                {
                    Name = model.Name,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    TerrainId = model.TerrainId,
                    PublishedOn = date,
                    PublisherId = userId,
                };

                await _context.Destinations.AddAsync(destination);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> AddEditedInfoAsync(string userId, EditDestinationViewModel model)
        {
            bool res = false;
            IdentityUser? user = await _user.FindByIdAsync(userId);
            Destination? destination = await _context
                .Destinations
                .FindAsync(model.Id);

            Terrain? terrain = await _context.Terrains.FindAsync(model.TerrainId);
            bool isDateValid = DateTime.TryParseExact(model.PublishedOn, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

            if ((user != null) && (terrain != null) && (isDateValid) && (destination != null) && (destination.PublisherId.ToLower() == userId.ToLower()))
            {
                destination.Name = model.Name;
                destination.Description = model.Description;
                destination.ImageUrl = model.ImageUrl;
                destination.TerrainId = model.TerrainId;
                destination.PublishedOn = date;

                await _context.SaveChangesAsync();
                res = true;
            }

            return res;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int id)
        {
            bool result = false;
            IdentityUser? user = await _user.FindByIdAsync(userId);
            Destination? destination = await _context
                .Destinations
                .FindAsync(id);

            if ((user != null) && (destination != null) && (destination.PublisherId.ToLower() != userId.ToLower()))
            {
                UserDestination? userFavDes = await _context.UsersDestinations
                    .SingleOrDefaultAsync(ud => ud.UserId.ToLower() == userId.ToLower() && ud.DestinationId == destination.Id);

                if (userFavDes == null)
                {
                    UserDestination favorites = new UserDestination
                    {
                        UserId = userId,
                        DestinationId = id
                    };

                    await _context.UsersDestinations .AddAsync(favorites);
                    await _context.SaveChangesAsync();

                    result = true;
                }
            }


            return result;
        }

        public async Task<bool> DeleteConfirmedAsync(string userId, DeleteViewModel model)
        {
            bool result = false;
            IdentityUser? user = await _user.FindByIdAsync(userId);
            Destination? destination = await _context
                .Destinations
                .FindAsync(model.Id);

            if ((user != null) && (destination != null) && (userId.ToLower() == destination.PublisherId.ToLower()))
            {
                destination.IsDeleted = true;

                await _context.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        public async Task<DeleteViewModel?> DeleteInfoModelAsync(string userId, int? destId)
        {
            DeleteViewModel? deleteModel = null;

            if (destId != null)
            {
                Destination? destinationToDelete = await _context
                    .Destinations
                    .Include(d => d.Publisher)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d => d.Id == destId);

                if ((destinationToDelete != null) && (destinationToDelete.PublisherId.ToLower() == userId.ToLower()))
                {
                    deleteModel = new DeleteViewModel
                    {
                        Id = destinationToDelete.Id,
                        Name = destinationToDelete.Name,
                        Publisher = destinationToDelete.Publisher.NormalizedUserName,
                        PublisherId = destinationToDelete.PublisherId,
                    };
                }
            }

            return deleteModel;
        }

        public async Task<IEnumerable<AllDestinationsViewModel>> GetAllDestinationsAsync(string? userId)
        {
            IEnumerable<AllDestinationsViewModel> allDestinations = await _context
                .Destinations
                .AsNoTracking()
                .Select(d => new AllDestinationsViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    ImageUrl = d.ImageUrl,
                    Terrain = d.Terrain.Name,
                    FavoritesCount = d.UsersDestinations.Count,
                    IsPublisher = String.IsNullOrEmpty(userId) == false ?
                            d.PublisherId.ToLower() == userId.ToLower() : false,
                    IsFavorite = String.IsNullOrEmpty(userId) == false ?
                            d.UsersDestinations.Any(ud => ud.UserId.ToLower() == userId.ToLower()) : false,
                })
                .ToListAsync();

            return allDestinations;
        }

        public async Task<DetailsDestinationViewModel> GetDestinationDetailsAsync(int? id, string? userId)
        {
            DetailsDestinationViewModel? destinationDetails = null;

            if (id.HasValue)
            {
                Destination? destination = await _context
                    .Destinations
                    .Include(d => d.Terrain)
                    .Include(d => d.Publisher)
                    .Include(d => d.UsersDestinations)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d => d.Id == id.Value);


                if (destination != null)
                {
                    destinationDetails = new DetailsDestinationViewModel
                    {
                        Id = destination.Id,
                        Name = destination.Name,
                        Description = destination.Description,
                        ImageUrl = destination.ImageUrl,
                        Terrain = destination.Terrain.Name,
                        PublishedOn = destination.PublishedOn,
                        Publisher = destination.Publisher.NormalizedUserName,
                        IsPublisher = userId != null ? destination.PublisherId.ToLower() == userId.ToLower() : false,
                        IsFavorite = userId != null ? destination.UsersDestinations.Any(ud => ud.UserId.ToLower() == userId.ToLower()) : false
                    };
                }
            }

            return destinationDetails;
        }

        public async Task<EditDestinationViewModel?> GetDestinationForEdit(string userId, int? destId)
        {
            EditDestinationViewModel? editDest = null;

            if (destId != null)
            {
                Destination? dest = await _context
                    .Destinations
                    .AsNoTracking()
                    .SingleOrDefaultAsync(d => d.Id == destId);

                if ((dest != null) && (dest.PublisherId.ToLower() == userId.ToLower()))
                {
                    editDest = new EditDestinationViewModel
                    {
                        Id = dest.Id,
                        Name = dest.Name,
                        Description = dest.Description,
                        ImageUrl = dest.ImageUrl,
                        PublishedOn = dest.PublishedOn.ToString(DateFormat),
                        PublisherId = dest.PublisherId,
                        TerrainId = dest.TerrainId,
                    };
                }
            }

            return editDest;
        }

        public async Task<IEnumerable<FavoritesViewModel>?> GetFavoritesListAsync(string userId)
        {
            IdentityUser? user = await _user.FindByIdAsync(userId);
            IEnumerable<FavoritesViewModel>? models = null!;
            if (user != null)
            {
                models = await _context.UsersDestinations
                    .Include(ud => ud.Destination)
                    .ThenInclude(d => d.Terrain)
                    .AsNoTracking()
                    .Where(ud => ud.UserId.ToLower() == userId.ToLower())
                    .Select(ud => new FavoritesViewModel
                    {
                        Id = ud.DestinationId,
                        Name = ud.Destination.Name,
                        ImageUrl = ud.Destination.ImageUrl,
                        Terrain = ud.Destination.Terrain.Name
                    })
                    .ToArrayAsync();
            }

            return models;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int id)
        {
            bool result = false;
            IdentityUser? user = await _user.FindByIdAsync(userId);
            UserDestination? destination = await _context.UsersDestinations
                .FirstOrDefaultAsync(ud => ud.UserId.ToLower() == userId.ToLower() && ud.DestinationId == id);

            if (destination != null)
            {
                _context.UsersDestinations.Remove(destination);
                await _context.SaveChangesAsync();

                result = true;
            }

            return result;
        }
    }
}
