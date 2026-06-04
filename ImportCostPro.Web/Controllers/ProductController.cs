using ImportCostPro.BusinessLogic.DTOs.Product;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities.Enums;
using ImportCostPro.Web.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        private readonly ICountryService _countryService;
        private readonly ITariffCategoryService _tariffCategoryService;

        public ProductController(
            IProductService service,
            ICountryService countryService,
            ITariffCategoryService tariffCategoryService)
        {
            _service = service;
            _countryService = countryService;
            _tariffCategoryService = tariffCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllProductAsync();

            var viewModels = dtos.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                CodeReference = p.CodeReference,
                CountryName = p.CountryName,
                TariffCategoryName = p.TariffCategoryName,
                TariffCode = p.TariffCode,
                TariffPercentage = p.TariffPercentage,
                UnitWeight = p.UnitWeight,
                Length = p.Length,
                Width = p.Width,
                Height = p.Height,
                UnitOfMeasure = p.UnitOfMeasure,
                Description = p.Description,
                IsActive = p.IsActive
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateProductViewModel
            {
                Countries = await GetCountrySelectListAsync(),
                TariffCategories = await GetTariffCategorySelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Countries = await GetCountrySelectListAsync();
                viewModel.TariffCategories = await GetTariffCategorySelectListAsync();
                return View(viewModel);
            }

            try
            {
                var dto = new CreateProductDto
                {
                    Name = viewModel.Name,
                    CodeReference = viewModel.CodeReference,
                    CountryId = viewModel.CountryId,
                    TariffCategoryId = viewModel.TariffCategoryId,
                    UnitWeight = viewModel.UnitWeight,
                    Length = viewModel.Length,
                    Width = viewModel.Width,
                    Height = viewModel.Height,
                    UnitOfMeasure = viewModel.UnitOfMeasure,
                    Description = viewModel.Description
                };

                await _service.CreateProductAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Countries = await GetCountrySelectListAsync();
                viewModel.TariffCategories = await GetTariffCategorySelectListAsync();
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetProductByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new UpdateProductViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                CodeReference = dto.CodeReference,
                CountryId = dto.CountryId,
                TariffCategoryId = dto.TariffCategoryId,
                UnitWeight = dto.UnitWeight,
                Length = dto.Length,
                Width = dto.Width,
                Height = dto.Height,
                UnitOfMeasure = dto.UnitOfMeasure,
                Description = dto.Description,
                IsActive = dto.IsActive,
                Countries = await GetCountrySelectListAsync(dto.CountryId),
                TariffCategories = await GetTariffCategorySelectListAsync(dto.TariffCategoryId)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                viewModel.TariffCategories = await GetTariffCategorySelectListAsync(viewModel.TariffCategoryId);
                return View(viewModel);
            }

            try
            {
                var dto = new UpdateProductDto
                {
                    Id = id,
                    Name = viewModel.Name,
                    CodeReference = viewModel.CodeReference,
                    CountryId = viewModel.CountryId,
                    TariffCategoryId = viewModel.TariffCategoryId,
                    UnitWeight = viewModel.UnitWeight,
                    Length = viewModel.Length,
                    Width = viewModel.Width,
                    Height = viewModel.Height,
                    UnitOfMeasure = viewModel.UnitOfMeasure,
                    Description = viewModel.Description,
                    IsActive = viewModel.IsActive
                };

                await _service.UpdateProductAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                viewModel.TariffCategories = await GetTariffCategorySelectListAsync(viewModel.TariffCategoryId);
                return View(viewModel);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                viewModel.TariffCategories = await GetTariffCategorySelectListAsync(viewModel.TariffCategoryId);
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetProductByIdAsync(id);

            if (dto is null)
                return NotFound();

            return View(new ProductViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                CodeReference = dto.CodeReference,
                CountryName = dto.CountryName,
                TariffCategoryName = dto.TariffCategoryName,
                TariffCode = dto.TariffCode,
                TariffPercentage = dto.TariffPercentage,
                UnitWeight = dto.UnitWeight,
                Length = dto.Length,
                Width = dto.Width,
                Height = dto.Height,
                UnitOfMeasure = dto.UnitOfMeasure,
                Description = dto.Description,
                IsActive = dto.IsActive
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var wasSoftDeleted = await _service.DeleteProductAsync(id);

                if (wasSoftDeleted)
                {
                    TempData["DeleteMessage"] = "El producto ha sido desactivado porque tiene registros asociados.";
                    TempData["DeleteType"] = "soft";
                }
                else
                {
                    TempData["DeleteMessage"] = "El producto ha sido eliminado permanentemente.";
                    TempData["DeleteType"] = "hard";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["DeleteMessage"] = "El producto no fue encontrado.";
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

        private async Task<List<SelectListItem>> GetTariffCategorySelectListAsync(Guid? selectedId = null)
        {
            var categories = await _tariffCategoryService.GetAllTariffCategoriesAsync();

            return categories
                .Where(c => c.IsActive || (selectedId.HasValue && c.Id == selectedId.Value))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.TariffCode} - {c.Name}",
                    Selected = selectedId.HasValue && c.Id == selectedId.Value
                })
                .ToList();
        }
    }
}
