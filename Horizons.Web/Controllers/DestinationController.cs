using Horizons.Services.Core;
using Horizons.Services.Core.Contracts;
using Horizons.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static Horizons.GCommon.ValidationConstatnts.Destination;

namespace Horizons.Web.Controllers
{
    public class DestinationController : BaseController
    {
        private readonly IDestinationService _destinationService;
        private readonly ITerrainService _terrainService;

        public DestinationController(IDestinationService destinationService, ITerrainService terrainService)
        {
            _destinationService = destinationService;
            _terrainService = terrainService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var allDestinations = await _destinationService.GetAllDestinationsAsync(GetUserId());

                return View(allDestinations);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return RedirectToAction(nameof(Index), "Home");
            }
           
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int? id) 
        {
            try
            {
                DetailsDestinationViewModel destination = await _destinationService.GetDestinationDetailsAsync(id, GetUserId());

                if (destination == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View(destination);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Add() 
        {
            try
            {
                AddDestinationViewModel addDestination = new AddDestinationViewModel
                {
                    PublishedOn = DateTime.UtcNow.ToString(DateFormat),
                    Terrains = await _terrainService.GetAllTerrainsAsync()
                };

                return View(addDestination);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return RedirectToAction(nameof(Index));
            }
          
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddDestinationViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid) 
                {
                    return View(model);
                }

                await _destinationService.AddDestinationAsync(GetUserId() ,model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id) 
        {
            try
            {
                EditDestinationViewModel? model = await _destinationService.GetDestinationForEdit(GetUserId(), id);

                if (model == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                model.Terrains = await _terrainService.GetAllTerrainsAsync();

                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDestinationViewModel model) 
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return View(model);
                }

                bool result = await _destinationService.AddEditedInfoAsync(GetUserId(), model);

                if (!result)
                {
                    return View(model);
                }

                return RedirectToAction(nameof(Details) , new { id = model.Id});

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                DeleteViewModel? model = await _destinationService.DeleteInfoModelAsync(GetUserId(), id);

                if (model == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteViewModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }

                bool result = await _destinationService.DeleteConfirmedAsync(GetUserId(), model);

                if (!result)
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            try
            {
                IEnumerable<FavoritesViewModel>? favorites = await _destinationService.GetFavoritesListAsync(GetUserId());

                if (favorites == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return View(favorites);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                bool result = await _destinationService.AddToFavoritesAsync(GetUserId(), id.Value);

                if (result == false)
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                bool result = await _destinationService.RemoveFromFavoritesAsync(GetUserId(), id.Value);


                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
