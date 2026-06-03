using ImportCostPro.BusinessLogic.DTOs.Supplier;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.Supplier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _service;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;

        public SupplierController(ISupplierService service, ICountryService countryService, ICurrencyService currencyService)
        {
            _service = service;
            _countryService = countryService;
            _currencyService = currencyService;
        }

        // GET: /Supplier
        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllSupplierAsync();

            var viewModels = dtos.Select(s => new SupplierViewModel
            {
                Id = s.Id,
                Name = s.Name,
                CountryId = s.CountryId,
                CountryName = s.CountryName,
                Email = s.Email,
                Telephone = s.Telephone,
                CurrencyId = s.CurrencyId,
                CurrencyName = s.CurrencyName,
                IsActive = s.IsActive
            }).ToList();

            return View(viewModels);
        }

        // GET: /Supplier/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateSupplierViewModel
            {
                Countries = await GetCountrySelectListAsync(),
                Currencies = await GetCurrencySelectListAsync()
            };

            return View(viewModel);
        }

        // POST: /Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupplierViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Countries = await GetCountrySelectListAsync();
                viewModel.Currencies = await GetCurrencySelectListAsync();
                return View(viewModel);
            }

            try
            {
                var dto = new CreateSupplierDto
                {
                    Name = viewModel.Name,
                    CountryId = viewModel.CountryId,
                    Email = viewModel.Email,
                    Telephone = viewModel.Telephone,
                    CurrencyId = viewModel.CurrencyId
                };

                await _service.CreateSupplierAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Countries = await GetCountrySelectListAsync();
                viewModel.Currencies = await GetCurrencySelectListAsync();
                return View(viewModel);
            }
        }

        // GET: /Supplier/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetSupplierByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new UpdateSupplierViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                CountryId = dto.CountryId,
                Email = dto.Email,
                Telephone = dto.Telephone,
                CurrencyId = dto.CurrencyId,
                IsActive = dto.IsActive,
                Countries = await GetCountrySelectListAsync(dto.CountryId),
                Currencies = await GetCurrencySelectListAsync(dto.CurrencyId)
            };

            return View(viewModel);
        }

        // POST: /Supplier/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateSupplierViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                viewModel.Currencies = await GetCurrencySelectListAsync(viewModel.CurrencyId);
                return View(viewModel);
            }

            try
            {
                var dto = new UpdateSupplierDto
                {
                    Id = id,
                    Name = viewModel.Name,
                    CountryId = viewModel.CountryId,
                    Email = viewModel.Email,
                    Telephone = viewModel.Telephone,
                    CurrencyId = viewModel.CurrencyId,
                    IsActive = viewModel.IsActive
                };

                await _service.UpdateSupplierAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                viewModel.Currencies = await GetCurrencySelectListAsync(viewModel.CurrencyId);
                return View(viewModel);
            }
        }

        // GET: /Supplier/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetSupplierByIdAsync(id);

            if (dto is null)
                return NotFound();

            return View(new SupplierViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                CountryName = dto.CountryName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                CurrencyName = dto.CurrencyName,
                IsActive = dto.IsActive
            });
        }

        // POST: /Supplier/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var wasSoftDeleted = await _service.DeleteSupplierAsync(id);

                if (wasSoftDeleted)
                {
                    TempData["DeleteMessage"] = "El proveedor ha sido desactivado porque tiene registros asociados.";
                    TempData["DeleteType"] = "soft";
                }
                else
                {
                    TempData["DeleteMessage"] = "El proveedor ha sido eliminado permanentemente.";
                    TempData["DeleteType"] = "hard";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["DeleteMessage"] = "El proveedor no fue encontrado.";
                TempData["DeleteType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<List<SelectListItem>> GetCountrySelectListAsync(Guid? selectedId = null)
        {
            var countries = await _countryService.GetAllCountriesAsync();

            return countries
                .Where(c => c.IsActive || (selectedId.HasValue && c.Id == selectedId.Value))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = selectedId.HasValue && c.Id == selectedId.Value
                })
                .ToList();
        }

        private async Task<List<SelectListItem>> GetCurrencySelectListAsync(Guid? selectedId = null)
        {
            var currencies = await _currencyService.GetAllCurrenciesAsync();

            return currencies
                .Where(c => c.IsActive || (selectedId.HasValue && c.Id == selectedId.Value))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.ISOCode})",
                    Selected = selectedId.HasValue && c.Id == selectedId.Value
                })
                .ToList();
        }
    }
}
