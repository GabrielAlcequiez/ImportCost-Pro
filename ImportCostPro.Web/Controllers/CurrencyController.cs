using ImportCostPro.BusinessLogic.DTOs.Currency;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.Currency;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.Web.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _service;

        public CurrencyController(ICurrencyService service)
        {
            _service = service;
        }

        // GET: /Currency
        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllCurrenciesAsync();

            var viewModels = dtos.Select(c => new CurrencyViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ISOCode = c.ISOCode,
                Symbol = c.Symbol,
                IsLocalCurrency = c.IsLocalCurrency,
                IsActive = c.IsActive
            }).ToList();

            return View(viewModels);
        }

        // GET: /Currency/Create
        public IActionResult Create()
        {
            return View(new CreateCurrencyViewModel());
        }

        // POST: /Currency/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCurrencyViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new CreateCurrencyDto
                {
                    Name = viewModel.Name,
                    ISOCode = viewModel.ISOCode,
                    Symbol = viewModel.Symbol,
                    IsLocalCurrency = viewModel.IsLocalCurrency
                };

                await _service.CreateCurrencyAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(viewModel);
            }
        }

        // GET: /Currency/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetCurrencyByIdAsync(id);

            if (dto is null)
                return NotFound();

            var viewModel = new UpdateCurrencyViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                ISOCode = dto.ISOCode,
                Symbol = dto.Symbol,
                IsLocalCurrency = dto.IsLocalCurrency,
                IsActive = dto.IsActive
            };

            return View(viewModel);
        }

        // POST: /Currency/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateCurrencyViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new UpdateCurrencyDto
                {
                    Id = id,
                    Name = viewModel.Name,
                    ISOCode = viewModel.ISOCode,
                    Symbol = viewModel.Symbol,
                    IsLocalCurrency = viewModel.IsLocalCurrency,
                    IsActive = viewModel.IsActive
                };

                await _service.UpdateCurrencyAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(viewModel);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(viewModel);
            }
        }

        // GET: /Currency/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetCurrencyByIdAsync(id);

            if (dto is null)
                return NotFound();

            return View(new CurrencyViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                ISOCode = dto.ISOCode,
                Symbol = dto.Symbol,
                IsLocalCurrency = dto.IsLocalCurrency,
                IsActive = dto.IsActive
            });
        }

        // POST: /Currency/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var wasSoftDeleted = await _service.DeleteCurrencyAsync(id);

                if (wasSoftDeleted)
                {
                    TempData["DeleteMessage"] = "La moneda ha sido desactivada porque tiene registros asociados.";
                    TempData["DeleteType"] = "soft";
                }
                else
                {
                    TempData["DeleteMessage"] = "La moneda ha sido eliminada permanentemente.";
                    TempData["DeleteType"] = "hard";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["DeleteMessage"] = "La moneda no fue encontrada.";
                TempData["DeleteType"] = "error";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["DeleteMessage"] = ex.Message;
                TempData["DeleteType"] = "error";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
