using ImportCostPro.BusinessLogic.DTOs.ExchangeRate;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.ExchangeRate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRateService _service;
        private readonly ICurrencyService _currencyService;

        public ExchangeRateController(IExchangeRateService service, ICurrencyService currencyService)
        {
            _service = service;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllExchangeRatesAsync();

            var viewModels = dtos.Select(r => new ExchangeRateViewModel
            {
                Id = r.Id,
                SourceCurrencyId = r.SourceCurrencyId,
                SourceCurrencyCode = r.SourceCurrencyCode,
                SourceCurrencyName = r.SourceCurrencyName,
                TargetCurrencyId = r.TargetCurrencyId,
                TargetCurrencyCode = r.TargetCurrencyCode,
                TargetCurrencyName = r.TargetCurrencyName,
                RateValue = r.RateValue,
                EffectiveDate = r.EffectiveDate,
                IsActive = r.IsActive
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateExchangeRateViewModel
            {
                Currencies = await GetCurrencySelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExchangeRateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Currencies = await GetCurrencySelectListAsync();
                return View(viewModel);
            }

            try
            {
                var dto = new CreateExchangeRateDto
                {
                    SourceCurrencyId = viewModel.SourceCurrencyId,
                    TargetCurrencyId = viewModel.TargetCurrencyId,
                    RateValue = viewModel.RateValue,
                    EffectiveDate = viewModel.EffectiveDate
                };

                await _service.CreateExchangeRatesAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Currencies = await GetCurrencySelectListAsync();
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetExchangeRatesByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new UpdateExchangeRateViewModel
            {
                Id = dto.Id,
                SourceCurrencyId = dto.SourceCurrencyId,
                TargetCurrencyId = dto.TargetCurrencyId,
                RateValue = dto.RateValue,
                EffectiveDate = dto.EffectiveDate,
                IsActive = dto.IsActive,
                Currencies = await GetCurrencySelectListAsync(dto.SourceCurrencyId, dto.TargetCurrencyId)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateExchangeRateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Currencies = await GetCurrencySelectListAsync(viewModel.SourceCurrencyId, viewModel.TargetCurrencyId);
                return View(viewModel);
            }

            try
            {
                var dto = new UpdateExchangeRateDto
                {
                    Id = id,
                    SourceCurrencyId = viewModel.SourceCurrencyId,
                    TargetCurrencyId = viewModel.TargetCurrencyId,
                    RateValue = viewModel.RateValue,
                    EffectiveDate = viewModel.EffectiveDate,
                    IsActive = viewModel.IsActive
                };

                await _service.UpdateExchangeRatesAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Currencies = await GetCurrencySelectListAsync(viewModel.SourceCurrencyId, viewModel.TargetCurrencyId);
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetExchangeRatesByIdAsync(id);

            if (dto is null)
                return NotFound();

            return View(new ExchangeRateViewModel
            {
                Id = dto.Id,
                SourceCurrencyCode = dto.SourceCurrencyCode,
                SourceCurrencyName = dto.SourceCurrencyName,
                TargetCurrencyCode = dto.TargetCurrencyCode,
                TargetCurrencyName = dto.TargetCurrencyName,
                RateValue = dto.RateValue,
                EffectiveDate = dto.EffectiveDate,
                IsActive = dto.IsActive
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var wasSoftDeleted = await _service.DeleteExchangeRateAsync(id);

                if (wasSoftDeleted)
                {
                    TempData["DeleteMessage"] = "La tasa de cambio ha sido desactivada porque tiene registros asociados.";
                    TempData["DeleteType"] = "soft";
                }
                else
                {
                    TempData["DeleteMessage"] = "La tasa de cambio ha sido eliminada permanentemente.";
                    TempData["DeleteType"] = "hard";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["DeleteMessage"] = "La tasa de cambio no fue encontrada.";
                TempData["DeleteType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<List<SelectListItem>> GetCurrencySelectListAsync(Guid? sourceId = null, Guid? targetId = null)
        {
            var currencies = await _currencyService.GetAllCurrenciesAsync();

            return currencies
                .Where(c => c.IsActive || (sourceId.HasValue && c.Id == sourceId.Value) || (targetId.HasValue && c.Id == targetId.Value))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.ISOCode})"
                })
                .ToList();
        }
    }
}
