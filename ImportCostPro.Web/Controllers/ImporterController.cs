using ImportCostPro.BusinessLogic.DTOs.Importer;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.Importer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.Controllers
{
    public class ImporterController : Controller
    {
        private readonly IImporterService _service;
        private readonly ICountryService _countryService;

        public ImporterController(IImporterService service, ICountryService countryService)
        {
            _service = service;
            _countryService = countryService;
        }

        // GET: /Importer
        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllImportersAsync();

            var viewModels = dtos.Select(i => new ImporterViewModel
            {
                Id = i.Id,
                Name = i.Name,
                TaxId = i.TaxId,
                CountryId = i.CountryId,
                CountryName = i.CountryName,
                Phone = i.Phone,
                Email = i.Email,
                Address = i.Address,
                IsActive = i.IsActive
            }).ToList();

            return View(viewModels);
        }

        // GET: /Importer/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateImporterViewModel
            {
                Countries = await GetCountrySelectListAsync()
            };

            return View(viewModel);
        }

        // POST: /Importer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateImporterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Countries = await GetCountrySelectListAsync();
                return View(viewModel);
            }

            try
            {
                var dto = new CreateImporterDto
                {
                    Name = viewModel.Name,
                    TaxId = viewModel.TaxId,
                    CountryId = viewModel.CountryId,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    Address = viewModel.Address
                };

                await _service.CreateImporterAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Countries = await GetCountrySelectListAsync();
                return View(viewModel);
            }
        }

        // GET: /Importer/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetImporterByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new UpdateImporterViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                TaxId = dto.TaxId,
                CountryId = dto.CountryId,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = dto.IsActive,
                Countries = await GetCountrySelectListAsync(dto.CountryId)
            };

            return View(viewModel);
        }

        // POST: /Importer/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateImporterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                return View(viewModel);
            }

            try
            {
                var dto = new UpdateImporterDto
                {
                    Id = id,
                    Name = viewModel.Name,
                    TaxId = viewModel.TaxId,
                    CountryId = viewModel.CountryId,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    Address = viewModel.Address,
                    IsActive = viewModel.IsActive
                };

                await _service.UpdateImporterAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                return View(viewModel);
            }
        }

        // GET: /Importer/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetImporterByIdAsync(id);

            if (dto is null)
                return NotFound();

            return View(new ImporterViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                TaxId = dto.TaxId,
                CountryName = dto.CountryName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = dto.IsActive
            });
        }

        // POST: /Importer/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var wasSoftDeleted = await _service.DeleteImporterAsync(id);

                if (wasSoftDeleted)
                {
                    TempData["DeleteMessage"] = "El importador ha sido desactivado porque tiene registros asociados.";
                    TempData["DeleteType"] = "soft";
                }
                else
                {
                    TempData["DeleteMessage"] = "El importador ha sido eliminado permanentemente.";
                    TempData["DeleteType"] = "hard";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["DeleteMessage"] = "El importador no fue encontrado.";
                TempData["DeleteType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper: construye el SelectList de países, con la opción pre-seleccionada si se pasa el id
        private async Task<List<SelectListItem>> GetCountrySelectListAsync(Guid? selectedId = null)
        {
            var countries = await _countryService.GetAllCountriesAsync();

            return countries
                .Where(c => c.IsActive)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = selectedId.HasValue && c.Id == selectedId.Value
                })
                .ToList();
        }
    }
}
