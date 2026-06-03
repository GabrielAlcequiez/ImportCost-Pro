using ImportCostPro.BusinessLogic.DTOs.ImportOrder;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities.Enums;
using ImportCostPro.Web.ViewModels.ImportOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImportCostPro.Web.Controllers
{
    public class ImportOrderController : Controller
    {
        private readonly IImportOrderService _service;
        private readonly IImporterService _importerService;
        private readonly ISupplierService _supplierService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly IExchangeRateService _exchangeRateService;

        public ImportOrderController(
            IImportOrderService service,
            IImporterService importerService,
            ISupplierService supplierService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IProductService productService,
            IExchangeRateService exchangeRateService)
        {
            _service = service;
            _importerService = importerService;
            _supplierService = supplierService;
            _countryService = countryService;
            _currencyService = currencyService;
            _productService = productService;
            _exchangeRateService = exchangeRateService;
        }

        // GET: /ImportOrder
        public async Task<IActionResult> Index()
        {
            var dtos = await _service.GetAllAsync();

            var viewModels = dtos.Select(d => new ImportOrderIndexViewModel
            {
                Id = d.Id,
                OrderNumber = d.OrderNumber,
                OrderDate = d.OrderDate,
                Status = d.Status,
                ImporterName = d.ImporterName,
                SupplierName = d.SupplierName,
                CountryName = d.CountryName,
                CurrencyCode = d.CurrencyCode,
                ExchangeRateValue = d.ExchangeRateValue,
                DetailCount = d.Details?.Count ?? 0
            }).ToList();

            return View(viewModels);
        }

        // GET: /ImportOrder/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateImportOrderViewModel
            {
                Importers = await GetImporterSelectListAsync(),
                Suppliers = await GetSupplierSelectListAsync(),
                Countries = await GetCountrySelectListAsync(),
                Currencies = await GetCurrencySelectListAsync(),
                TransportModes = GetTransportModeSelectList()
            };

            return View(viewModel);
        }

        // POST: /ImportOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateImportOrderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Importers = await GetImporterSelectListAsync();
                viewModel.Suppliers = await GetSupplierSelectListAsync();
                viewModel.Countries = await GetCountrySelectListAsync();
                viewModel.Currencies = await GetCurrencySelectListAsync();
                viewModel.TransportModes = GetTransportModeSelectList();
                return View(viewModel);
            }

            try
            {
                var dto = new CreateImportOrderDto
                {
                    OrderNumber = viewModel.OrderNumber,
                    OrderDate = viewModel.OrderDate,
                    ImporterId = viewModel.ImporterId,
                    SupplierId = viewModel.SupplierId,
                    CountryId = viewModel.CountryId,
                    CurrencyId = viewModel.CurrencyId,
                    ExchangeRateValue = viewModel.ExchangeRateValue,
                    TransportMode = viewModel.TransportMode
                };

                await _service.CreateAsync(dto);
                TempData["SuccessMessage"] = "Orden de importación creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                viewModel.Importers = await GetImporterSelectListAsync();
                viewModel.Suppliers = await GetSupplierSelectListAsync();
                viewModel.Countries = await GetCountrySelectListAsync();
                viewModel.Currencies = await GetCurrencySelectListAsync();
                viewModel.TransportModes = GetTransportModeSelectList();
                return View(viewModel);
            }
        }

        // GET: /ImportOrder/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null)
                return NotFound();

            var viewModel = new UpdateImportOrderViewModel
            {
                Id = dto.Id,
                Status = dto.Status,
                OrderNumber = dto.OrderNumber,
                OrderDate = dto.OrderDate,
                ImporterId = dto.ImporterId,
                ImporterName = dto.ImporterName,
                SupplierId = dto.SupplierId,
                SupplierName = dto.SupplierName,
                CountryId = dto.CountryId,
                CountryName = dto.CountryName,
                CurrencyId = dto.CurrencyId,
                CurrencyCode = dto.CurrencyCode,
                ExchangeRateValue = dto.ExchangeRateValue,
                TransportMode = dto.TransportMode,
                Importers = await GetImporterSelectListAsync(dto.ImporterId),
                Suppliers = await GetSupplierSelectListAsync(dto.SupplierId),
                Countries = await GetCountrySelectListAsync(dto.CountryId),
                Currencies = await GetCurrencySelectListAsync(dto.CurrencyId),
                TransportModes = GetTransportModeSelectList(),
                Details = dto.Details?.Select(d => new ImportOrderDetailViewModel
                {
                    Id = d.Id,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    ProductCodeReference = d.ProductCodeReference,
                    Quantity = d.Quantity,
                    UnitCostFob = d.UnitCostFob,
                    UnitWeight = d.UnitWeight,
                    CustomDutyPercentage = d.CustomDutyPercentage,
                    DesiredProfitMargin = d.DesiredProfitMargin
                }).ToList() ?? new(),
                Expenses = dto.Expenses?.Select(e => new ImportOrderExpenseViewModel
                {
                    Id = e.Id,
                    CurrencyId = e.CurrencyId,
                    CurrencyCode = e.CurrencyCode,
                    ExpenseType = e.ExpenseType,
                    Amount = e.Amount,
                    ExchangeRateValue = e.ExchangeRateValue,
                    ApportionmentMethod = e.ApportionmentMethod,
                    ExpenseDate = e.ExpenseDate
                }).ToList() ?? new(),
                AvailableProducts = await GetProductSelectListAsync(),
                ExpenseCurrencies = await GetCurrencySelectListAsync(),
                ExpenseTypesList = GetEnumSelectList<ExpenseType>(),
                ApportionmentMethodsList = GetEnumSelectList<ApportionmentMethod>()
            };

            return View(viewModel);
        }

        // POST: /ImportOrder/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateImportOrderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Importers = await GetImporterSelectListAsync(viewModel.ImporterId);
                viewModel.Suppliers = await GetSupplierSelectListAsync(viewModel.SupplierId);
                viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
                viewModel.Currencies = await GetCurrencySelectListAsync(viewModel.CurrencyId);
                viewModel.TransportModes = GetTransportModeSelectList();
                viewModel.AvailableProducts = await GetProductSelectListAsync();
                viewModel.ExpenseCurrencies = await GetCurrencySelectListAsync();
                viewModel.ExpenseTypesList = GetEnumSelectList<ExpenseType>();
                viewModel.ApportionmentMethodsList = GetEnumSelectList<ApportionmentMethod>();
                return View(viewModel);
            }

            try
            {
                var dto = new UpdateImportOrderDto
                {
                    Id = id,
                    OrderNumber = viewModel.OrderNumber,
                    OrderDate = viewModel.OrderDate,
                    ImporterId = viewModel.ImporterId,
                    SupplierId = viewModel.SupplierId,
                    CountryId = viewModel.CountryId,
                    CurrencyId = viewModel.CurrencyId,
                    ExchangeRateValue = viewModel.ExchangeRateValue,
                    TransportMode = viewModel.TransportMode
                };

                await _service.UpdateHeaderAsync(id, dto);
                TempData["SuccessMessage"] = "Orden actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            viewModel.Importers = await GetImporterSelectListAsync(viewModel.ImporterId);
            viewModel.Suppliers = await GetSupplierSelectListAsync(viewModel.SupplierId);
            viewModel.Countries = await GetCountrySelectListAsync(viewModel.CountryId);
            viewModel.Currencies = await GetCurrencySelectListAsync(viewModel.CurrencyId);
            viewModel.TransportModes = GetTransportModeSelectList();
            viewModel.AvailableProducts = await GetProductSelectListAsync();
            viewModel.ExpenseCurrencies = await GetCurrencySelectListAsync();
            viewModel.ExpenseTypesList = GetEnumSelectList<ExpenseType>();
            viewModel.ApportionmentMethodsList = GetEnumSelectList<ApportionmentMethod>();
            return View(viewModel);
        }

        // POST: /ImportOrder/AddDetail/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetail(Guid id, Guid productId, decimal quantity, decimal unitCostFob, decimal unitWeight, decimal customDutyPercentage, decimal desiredProfitMargin)
        {
            try
            {
                var dto = new CreateImportOrderDetailDto
                {
                    ImportOrderId = id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitCostFob = unitCostFob,
                    UnitWeight = unitWeight,
                    CustomDutyPercentage = customDutyPercentage,
                    DesiredProfitMargin = desiredProfitMargin
                };

                await _service.AddDetailAsync(dto);
                TempData["SuccessMessage"] = "Producto agregado a la orden.";
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Edit), new { id });
        }

        // POST: /ImportOrder/RemoveDetail/{detailId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDetail(Guid detailId, Guid orderId)
        {
            try
            {
                await _service.RemoveDetailAsync(detailId);
                TempData["SuccessMessage"] = "Producto eliminado de la orden.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "El detalle no fue encontrado.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Edit), new { id = orderId });
        }

        // POST: /ImportOrder/AddExpense/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(Guid id, ExpenseType expenseType, decimal amount, Guid currencyId, decimal exchangeRateValue, ApportionmentMethod apportionmentMethod, DateTime expenseDate)
        {
            try
            {
                var dto = new CreateImportOrderExpenseDto
                {
                    ImportOrderId = id,
                    ExpenseType = expenseType,
                    Amount = amount,
                    CurrencyId = currencyId,
                    ExchangeRateValue = exchangeRateValue,
                    ApportionmentMethod = apportionmentMethod,
                    ExpenseDate = expenseDate
                };

                await _service.AddExpenseAsync(dto);
                TempData["SuccessMessage"] = "Gasto agregado a la orden.";
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Edit), new { id });
        }

        // POST: /ImportOrder/RemoveExpense/{expenseId}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveExpense(Guid expenseId, Guid orderId)
        {
            try
            {
                await _service.RemoveExpenseAsync(expenseId);
                TempData["SuccessMessage"] = "Gasto eliminado de la orden.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "El gasto no fue encontrado.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Edit), new { id = orderId });
        }

        // GET: /ImportOrder/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null)
                return NotFound();

            var viewModel = new UpdateImportOrderViewModel
            {
                Id = dto.Id,
                Status = dto.Status,
                OrderNumber = dto.OrderNumber,
                OrderDate = dto.OrderDate,
                ImporterId = dto.ImporterId,
                ImporterName = dto.ImporterName,
                SupplierId = dto.SupplierId,
                SupplierName = dto.SupplierName,
                CountryId = dto.CountryId,
                CountryName = dto.CountryName,
                CurrencyId = dto.CurrencyId,
                CurrencyCode = dto.CurrencyCode,
                ExchangeRateValue = dto.ExchangeRateValue,
                TransportMode = dto.TransportMode,
                Details = dto.Details?.Select(d => new ImportOrderDetailViewModel
                {
                    Id = d.Id, ProductId = d.ProductId, ProductName = d.ProductName,
                    ProductCodeReference = d.ProductCodeReference, Quantity = d.Quantity,
                    UnitCostFob = d.UnitCostFob, UnitWeight = d.UnitWeight,
                    CustomDutyPercentage = d.CustomDutyPercentage, DesiredProfitMargin = d.DesiredProfitMargin
                }).ToList() ?? new(),
                Expenses = dto.Expenses?.Select(e => new ImportOrderExpenseViewModel
                {
                    Id = e.Id, CurrencyId = e.CurrencyId, CurrencyCode = e.CurrencyCode,
                    ExpenseType = e.ExpenseType, Amount = e.Amount, ExchangeRateValue = e.ExchangeRateValue,
                    ApportionmentMethod = e.ApportionmentMethod, ExpenseDate = e.ExpenseDate
                }).ToList() ?? new()
            };

            return View(viewModel);
        }

        // GET: /ImportOrder/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null)
                return NotFound();

            var viewModel = new ImportOrderIndexViewModel
            {
                Id = dto.Id,
                OrderNumber = dto.OrderNumber,
                OrderDate = dto.OrderDate,
                Status = dto.Status,
                ImporterName = dto.ImporterName,
                SupplierName = dto.SupplierName,
                CountryName = dto.CountryName,
                CurrencyCode = dto.CurrencyCode,
                ExchangeRateValue = dto.ExchangeRateValue,
                DetailCount = dto.Details?.Count ?? 0
            };

            return View(viewModel);
        }

        // POST: /ImportOrder/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _service.CancelOrderAsync(id);
                TempData["SuccessMessage"] = "Orden cancelada exitosamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "La orden no fue encontrada.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /ImportOrder/CancelOrder/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            try
            {
                await _service.CancelOrderAsync(id);
                TempData["SuccessMessage"] = "Orden cancelada exitosamente.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "La orden no fue encontrada.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /ImportOrder/CloseOrder/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseOrder(Guid id)
        {
            try
            {
                await _service.CloseOrderAsync(id);
                TempData["SuccessMessage"] = "Orden de importación cerrada y archivada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cerrar la orden: {ex.Message}";
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && referer.Contains("LandedCostCalculation/Details", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(referer);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /ImportOrder/GetLatestExchangeRate
        [HttpGet]
        public async Task<IActionResult> GetLatestExchangeRate(Guid currencyId, DateTime orderDate)
        {
            try
            {
                var currencies = await _currencyService.GetAllCurrenciesAsync();
                var localCurrency = currencies.FirstOrDefault(c => c.IsLocalCurrency);

                if (localCurrency == null)
                {
                    return Json(new { success = false, message = "Moneda local no configurada." });
                }

                if (currencyId == localCurrency.Id)
                {
                    return Json(new { success = true, rate = 1.0m });
                }

                var rates = await _exchangeRateService.GetAllExchangeRatesAsync();
                var latestRate = rates
                    .Where(r => r.SourceCurrencyId == currencyId && 
                                r.TargetCurrencyId == localCurrency.Id && 
                                r.EffectiveDate.Date <= orderDate.Date && 
                                r.IsActive)
                    .OrderByDescending(r => r.EffectiveDate)
                    .FirstOrDefault();

                if (latestRate == null)
                {
                    return Json(new { success = false, message = "No existe una tasa de cambio activa para esta moneda en la fecha seleccionada." });
                }

                return Json(new { success = true, rate = latestRate.RateValue });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /ImportOrder/GetProductDetails
        [HttpGet]
        public async Task<IActionResult> GetProductDetails(Guid productId)
        {
            try
            {
                var p = await _productService.GetProductByIdAsync(productId);
                if (p == null)
                {
                    return Json(new { success = false, message = "Producto no encontrado." });
                }

                return Json(new { success = true, unitWeight = p.UnitWeight, customDutyPercentage = p.TariffPercentage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // --- Dropdown Helpers ---

        private async Task<List<SelectListItem>> GetImporterSelectListAsync(Guid? selectedId = null)
        {
            var items = await _importerService.GetAllImportersAsync();
            return items
                .Where(i => i.IsActive || (selectedId.HasValue && i.Id == selectedId.Value))
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Name} ({i.TaxId})"
                })
                .ToList();
        }

        private async Task<List<SelectListItem>> GetSupplierSelectListAsync(Guid? selectedId = null)
        {
            var items = await _supplierService.GetAllSupplierAsync();
            return items
                .Where(s => s.IsActive || (selectedId.HasValue && s.Id == selectedId.Value))
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();
        }

        private async Task<List<SelectListItem>> GetCountrySelectListAsync(Guid? selectedId = null)
        {
            var items = await _countryService.GetAllCountriesAsync();
            return items
                .Where(c => c.IsActive || (selectedId.HasValue && c.Id == selectedId.Value))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.ISOCode})"
                })
                .ToList();
        }

        private async Task<List<SelectListItem>> GetCurrencySelectListAsync(Guid? selectedId = null)
        {
            var items = await _currencyService.GetAllCurrenciesAsync();
            return items
                .Where(c => c.IsActive || (selectedId.HasValue && c.Id == selectedId.Value))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.ISOCode})"
                })
                .ToList();
        }

        private async Task<List<SelectListItem>> GetProductSelectListAsync()
        {
            var items = await _productService.GetAllProductAsync();
            return items
                .Where(p => p.IsActive)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.CodeReference} - {p.Name}"
                })
                .ToList();
        }

        private static List<SelectListItem> GetTransportModeSelectList()
        {
            return Enum.GetValues<TransportMode>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e switch
                    {
                        TransportMode.Maritime => "Marítimo",
                        TransportMode.Air => "Aéreo",
                        TransportMode.Land => "Terrestre",
                        _ => e.ToString()
                    }
                })
                .ToList();
        }

        private static List<SelectListItem> GetEnumSelectList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = GetEnumDisplayName(e)
                })
                .ToList();
        }

        private static string GetEnumDisplayName(Enum value)
        {
            return value switch
            {
                ExpenseType et => et switch
                {
                    ExpenseType.InternationalFreight => "Flete Internacional",
                    ExpenseType.InternationalInsurance => "Seguro Internacional",
                    ExpenseType.PortExpenses => "Gastos Portuarios",
                    ExpenseType.LocalTransport => "Transporte Local",
                    ExpenseType.CustomsBrokerFees => "Honorarios Aduanales",
                    ExpenseType.Storage => "Almacenaje",
                    ExpenseType.Handling => "Manejo de Carga",
                    ExpenseType.OtherExpenses => "Otros Gastos",
                    _ => et.ToString()
                },
                ApportionmentMethod am => am switch
                {
                    ApportionmentMethod.ByValue => "Por Valor",
                    ApportionmentMethod.ByWeight => "Por Peso",
                    ApportionmentMethod.ByVolume => "Por Volumen",
                    ApportionmentMethod.ByQuantity => "Por Cantidad",
                    _ => am.ToString()
                },
                _ => value.ToString()
            };
        }
    }
}
