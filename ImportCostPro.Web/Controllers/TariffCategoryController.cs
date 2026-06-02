using ImportCostPro.BusinessLogic.DTOs.TariffCategory;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.TariffCategory;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.Web.Controllers
{
    public class TariffCategoryController : Controller
    {
        private readonly ITariffCategoryService _service;

        public TariffCategoryController(ITariffCategoryService service)
        {
            _service = service;
        }

        // GET: /TariffCategory
        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllTariffCategoriesAsync();

            var viewModels = dtos.Select(t => new TariffCategoryViewModel
            {
                Id = t.Id,
                TariffCode = t.TariffCode,
                Name = t.Name,
                TariffPercentage = t.TariffPercentage,
                ApplyITBIS = t.ApplyITBIS,
                ApplyExciseTax = t.ApplyExciseTax,
                ExciseTaxPercentage = t.ExciseTaxPercentage,
                IsActive = t.IsActive
            }).ToList();

            return View(viewModels);
        }

        // GET: /TariffCategory/Create
        public IActionResult Create()
        {
            return View(new CreateTariffCategoryViewModel());
        }

        // POST: /TariffCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTariffCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new CreateTariffCategoryDto
                {
                    TariffCode = viewModel.TariffCode,
                    Name = viewModel.Name,
                    TariffPercentage = viewModel.TariffPercentage,
                    ApplyITBIS = viewModel.ApplyITBIS,
                    ApplyExciseTax = viewModel.ApplyExciseTax,
                    ExciseTaxPercentage = viewModel.ExciseTaxPercentage
                };

                await _service.CreateTariffCategoryAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(viewModel);
            }
        }

        // GET: /TariffCategory/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetTariffCategoryByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new UpdateTariffCategoryViewModel
            {
                Id = dto.Id,
                TariffCode = dto.TariffCode,
                Name = dto.Name,
                TariffPercentage = dto.TariffPercentage,
                ApplyITBIS = dto.ApplyITBIS,
                ApplyExciseTax = dto.ApplyExciseTax,
                ExciseTaxPercentage = dto.ExciseTaxPercentage,
                IsActive = dto.IsActive
            };

            return View(viewModel);
        }

        // POST: /TariffCategory/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateTariffCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new UpdateTariffCategoryDto
                {
                    Id = id,
                    TariffCode = viewModel.TariffCode,
                    Name = viewModel.Name,
                    TariffPercentage = viewModel.TariffPercentage,
                    ApplyITBIS = viewModel.ApplyITBIS,
                    ApplyExciseTax = viewModel.ApplyExciseTax,
                    ExciseTaxPercentage = viewModel.ExciseTaxPercentage,
                    IsActive = viewModel.IsActive
                };

                await _service.UpdateTariffCategoryAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(viewModel);
            }
        }

        // GET: /TariffCategory/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetTariffCategoryByIdAsync(id);

            if (dto is null)
                return NotFound();

            return View(new TariffCategoryViewModel
            {
                Id = dto.Id,
                TariffCode = dto.TariffCode,
                Name = dto.Name,
                TariffPercentage = dto.TariffPercentage,
                ApplyITBIS = dto.ApplyITBIS,
                ApplyExciseTax = dto.ApplyExciseTax,
                ExciseTaxPercentage = dto.ExciseTaxPercentage,
                IsActive = dto.IsActive
            });
        }

        // POST: /TariffCategory/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var wasSoftDeleted = await _service.DeleteTariffCategoryAsync(id);

                if (wasSoftDeleted)
                {
                    TempData["DeleteMessage"] = "La categoría ha sido desactivada porque tiene registros asociados.";
                    TempData["DeleteType"] = "soft";
                }
                else
                {
                    TempData["DeleteMessage"] = "La categoría ha sido eliminada permanentemente.";
                    TempData["DeleteType"] = "hard";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["DeleteMessage"] = "La categoría no fue encontrada.";
                TempData["DeleteType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
