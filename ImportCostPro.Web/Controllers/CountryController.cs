using ImportCostPro.BusinessLogic.DTOs.Country;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.Country;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.Web.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryService _service;

        public CountryController(ICountryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllCountriesAsync();

            var viewModels = dtos.Select(country => new CountryViewModel
            {
                Id = country.Id,
                Name = country.Name,
                ISOCode = country.ISOCode,
                IsActive = country.IsActive
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Create()
        {
            return View(new CreateCountryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCountryViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var createDto = new CreateCountryDto
                {
                    Name = viewModel.Name,
                    ISOCode = viewModel.ISOCode
                };

                await _service.CreateCountryAsync(createDto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetCountryByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new UpdateCountryViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                ISOCode = dto.ISOCode,
                IsActive = dto.IsActive
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCountryViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var updateDto = new UpdateCountryDto
                {
                    Id = id,
                    Name = viewModel.Name,
                    ISOCode = viewModel.ISOCode,
                    IsActive = viewModel.IsActive
                };

                await _service.UpdateCountryAsync(id, updateDto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(viewModel);
            }
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetCountryByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new CountryViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                ISOCode = dto.ISOCode,
                IsActive = dto.IsActive
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var wasSoftDeleted = await _service.DeleteCountryAsync(id);

                if (wasSoftDeleted)
                {
                    TempData["DeleteMessage"] = "El país ha sido desactivado porque tiene registros asociados.";
                    TempData["DeleteType"] = "soft";
                }
                else
                {
                    TempData["DeleteMessage"] = "El país ha sido eliminado permanentemente.";
                    TempData["DeleteType"] = "hard";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["DeleteMessage"] = "El país no fue encontrado.";
                TempData["DeleteType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}