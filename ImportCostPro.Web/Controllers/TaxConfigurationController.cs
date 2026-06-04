using ImportCostPro.BusinessLogic.DTOs.TaxConfiguration;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Web.ViewModels.TaxConfiguration;
using Microsoft.AspNetCore.Mvc;

namespace ImportCostPro.Web.Controllers
{
    public class TaxConfigurationController : Controller
    {
        private readonly ITaxConfigurationService _service;

        public TaxConfigurationController(ITaxConfigurationService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var dto = await _service.GetCurrentAsync();

            var viewModel = new TaxConfigurationViewModel
            {
                Id = dto.Id,
                GeneralItbisPercentage = dto.GeneralItbisPercentage,
                CustomsServiceFeePercentage = dto.CustomsServiceFeePercentage
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit()
        {
            var dto = await _service.GetCurrentAsync();

            var viewModel = new UpdateTaxConfigurationViewModel
            {
                Id = dto.Id,
                GeneralItbisPercentage = dto.GeneralItbisPercentage,
                CustomsServiceFeePercentage = dto.CustomsServiceFeePercentage
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTaxConfigurationViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new UpdateTaxConfigurationDto
                {
                    Id = viewModel.Id,
                    GeneralItbisPercentage = viewModel.GeneralItbisPercentage,
                    CustomsServiceFeePercentage = viewModel.CustomsServiceFeePercentage
                };

                await _service.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(viewModel);
            }
        }
    }
}
