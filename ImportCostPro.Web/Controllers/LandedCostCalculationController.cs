using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities.Enums;
using ImportCostPro.Web.ViewModels.LandedCostCalculation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.Controllers
{
    public class LandedCostCalculationController : Controller
    {
        private readonly ILandedCostCalculationService _calculationService;
        private readonly IImportOrderService _orderService;

        public LandedCostCalculationController(
            ILandedCostCalculationService calculationService,
            IImportOrderService orderService)
        {
            _calculationService = calculationService;
            _orderService = orderService;
        }

        // GET: /LandedCostCalculation
        public async Task<IActionResult> Index()
        {
            var allOrders = await _orderService.GetAllAsync();

            // 1. Populate open orders for the dropdown (sorted by date descending)
            var openOrders = allOrders
                .Where(o => o.Status == ImportOrderStatus.Open)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var viewModel = new LandedCostCalculationIndexViewModel
            {
                OpenOrders = openOrders.Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = $"{o.OrderNumber} - {o.SupplierName} ({o.OrderDate:dd/MM/yyyy})"
                }).ToList()
            };

            // 2. Set the default selected order to the most recent open one
            if (openOrders.Count != 0)
            {
                viewModel.SelectedImportOrderId = openOrders.First().Id;
            }

            // 3. Retrieve historical calculations for Calculated or Closed orders
            var historyItems = new List<LandedCostHistoryItemViewModel>();
            var processedOrders = allOrders
                .Where(o => o.Status == ImportOrderStatus.Calculated || o.Status == ImportOrderStatus.Closed)
                .ToList();

            foreach (var order in processedOrders)
            {
                var calcs = await _calculationService.GetAllByOrderIdAsync(order.Id);
                var latestCalc = calcs.OrderByDescending(c => c.CalculationDate).FirstOrDefault();

                if (latestCalc != null)
                {
                    historyItems.Add(new LandedCostHistoryItemViewModel
                    {
                        CalculationId = latestCalc.Id,
                        ImportOrderId = order.Id,
                        OrderNumber = order.OrderNumber,
                        CalculationDate = latestCalc.CalculationDate,
                        ImporterName = order.ImporterName,
                        SupplierName = order.SupplierName,
                        ImportTotalCost = latestCalc.ImportTotalCost,
                        LocalCurrencyCode = latestCalc.LocalCurrencyCode,
                        OrderStatus = order.Status
                    });
                }
            }

            viewModel.HistoryItems = historyItems.OrderByDescending(h => h.CalculationDate).ToList();

            return View(viewModel);
        }

        // POST: /LandedCostCalculation/Calculate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(Guid importOrderId)
        {
            if (importOrderId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Debe seleccionar una orden de importación válida.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var calculationDto = await _calculationService.CalculateAsync(importOrderId);
                TempData["SuccessMessage"] = "¡Cálculo de costo terrestre procesado y guardado como oficial exitosamente!";
                return RedirectToAction(nameof(Details), new { id = calculationDto.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al calcular landed cost: {ex.Message}";
                
                // If the request was triggered from the import order details view, redirect back there
                var referer = Request.Headers.Referer.ToString();
                if (!string.IsNullOrEmpty(referer) && referer.Contains("ImportOrder/Details", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Details", "ImportOrder", new { id = importOrderId });
                }

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /LandedCostCalculation/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var calc = await _calculationService.GetByIdAsync(id);
            if (calc == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetByIdAsync(calc.ImportOrderId);
            var orderStatus = order?.Status ?? ImportOrderStatus.Calculated;

            var viewModel = new LandedCostCalculationViewModel
            {
                Id = calc.Id,
                ImportOrderId = calc.ImportOrderId,
                OrderNumber = calc.OrderNumber,
                CalculationDate = calc.CalculationDate,
                OrderStatus = orderStatus,
                LocalCurrencyId = calc.LocalCurrencyId,
                LocalCurrencyCode = calc.LocalCurrencyCode,
                ExchangeRateUsed = calc.ExchangeRateUsed,
                GeneralItbisPercentageUsed = calc.GeneralItbisPercentageUsed,
                CustomsServiceFeePercentageUsed = calc.CustomsServiceFeePercentageUsed,
                FobTotalOriginal = calc.FobTotalOriginal,
                FobTotalLocal = calc.FobTotalLocal,
                FreightTotalLocal = calc.FreightTotalLocal,
                InsuranceTotalLocal = calc.InsuranceTotalLocal,
                CifTotalLocal = calc.CifTotalLocal,
                DutyTotalLocal = calc.DutyTotalLocal,
                SelectiveTaxTotalLocal = calc.SelectiveTaxTotalLocal,
                CustomsServiceFeeTotalLocal = calc.CustomsServiceFeeTotalLocal,
                ItbisTotalLocal = calc.ItbisTotalLocal,
                LocalExpensesTotalLocal = calc.LocalExpensesTotalLocal,
                ImportTotalCost = calc.ImportTotalCost,
                TotalQuantity = calc.TotalQuantity,
                Details = calc.Details.Select(d => new LandedCostCalculationDetailViewModel
                {
                    Id = d.Id,
                    LandedCostCalculationId = d.LandedCostCalculationId,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    ProductCodeReference = d.ProductCodeReference,
                    Quantity = d.Quantity,
                    TariffPercentageUsed = d.TariffPercentageUsed,
                    ExciseTaxPercentageUsed = d.ExciseTaxPercentageUsed,
                    FobOriginal = d.FobOriginal,
                    FobLocal = d.FobLocal,
                    FreightAssigned = d.FreightAssigned,
                    InsuranceAssigned = d.InsuranceAssigned,
                    CifLocal = d.CifLocal,
                    DutyCalculated = d.DutyCalculated,
                    SelectiveTaxCalculated = d.SelectiveTaxCalculated,
                    CustomsServiceFeeCalculated = d.CustomsServiceFeeCalculated,
                    ItbisCalculated = d.ItbisCalculated,
                    LocalExpensesAssigned = d.LocalExpensesAssigned,
                    ImportTotalCost = d.ImportTotalCost,
                    UnitLandedCost = d.UnitLandedCost,
                    DesiredProfitMargin = d.DesiredProfitMargin,
                    SuggestedSellingPrice = d.SuggestedSellingPrice
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: /LandedCostCalculation/DetailsByOrder/{orderId}
        public async Task<IActionResult> DetailsByOrder(Guid orderId)
        {
            var calcs = await _calculationService.GetAllByOrderIdAsync(orderId);
            var latest = calcs.OrderByDescending(c => c.CalculationDate).FirstOrDefault();
            if (latest == null)
            {
                TempData["ErrorMessage"] = "No se ha encontrado un cálculo oficial para esta orden.";
                return RedirectToAction("Details", "ImportOrder", new { id = orderId });
            }
            return RedirectToAction(nameof(Details), new { id = latest.Id });
        }
    }
}
