using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.BusinessLogic.DTOs.LandedCostCalculation;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Entities.Enums;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class LandedCostCalculationService(IUnitOfWork unitOfWork) : ILandedCostCalculationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<LandedCostCalculationDto> CalculateAsync(Guid importOrderId)
        {
            //Fetch  the  order with details and expenses 
            var order = await _unitOfWork.ImportOrders.GetByIdWithDetailsAsync(importOrderId) ?? throw new KeyNotFoundException("Import order not found.");
            if (order.Status != ImportOrderStatus.Open)
                throw new InvalidOperationException("La orden debe estar en estado Abierta.");

            //validate that at  least one product is added
            if (order.Details == null || order.Details.Count == 0)
                throw new InvalidOperationException("La orden debe tener al menos un producto agregado.");

            //Load the active global  tax config
            var taxConfig = await _unitOfWork.TaxConfigurations.GetCurrentAsync() ?? throw new InvalidOperationException("Debe existir una configuración de impuestos registrada.");
            //load the active local currency
            var localCurrencies = await _unitOfWork.Currencies.GetLocalCurrenciesAsync();
            var localCurrency = localCurrencies.FirstOrDefault() ?? throw new InvalidOperationException("Debe existir una moneda local configurada.");

            //check order currecny exchange rate
            bool isOrderCurrencyLocal = order.CurrencyId == localCurrency.Id;
            decimal exchangeRateUsed = isOrderCurrencyLocal ? 1.0m : order.ExchangeRateValue;

            if (!isOrderCurrencyLocal && exchangeRateUsed <= 0)
                throw new InvalidOperationException("No existe una tasa de cambio activa desde la moneda de la orden hacia la moneda local para la fecha de la orden.");

            //Check exchange rates for all expenses
            foreach (var expense in order.Expenses)
            {
                bool isExpenseCurrencyLocal = expense.CurrencyId == localCurrency.Id;
                if (!isExpenseCurrencyLocal && expense.ExchangeRateValue <= 0)
                    throw new InvalidOperationException("No existe una tasa de cambio activa desde la moneda del gasto hacia la moneda local para la fecha del gasto.");
            }

            //validate mandatory international expenses (freight and insurance)
            bool hasFreight = order.Expenses.Any(e => e.ExpenseType == ExpenseType.InternationalFreight);
            if (!hasFreight)
                throw new InvalidOperationException("No se puede calcular la orden porque no tiene registrado el gasto de flete internacional.");

            bool hasInsurance = order.Expenses.Any(e => e.ExpenseType == ExpenseType.InternationalInsurance);
            if (!hasInsurance)
                throw new InvalidOperationException("No se puede calcular la orden porque no tiene registrado el gasto de seguro internacional.");

            //validate product details and ther profit margins
            foreach (var d in order.Details)
            {
                if (d.Product == null || d.Product.TariffCategory == null)
                    throw new InvalidOperationException("Cada producto agregado debe tener categoría arancelaria válida.");
                if (d.DesiredProfitMargin < 0 || d.DesiredProfitMargin >= 100)
                    throw new InvalidOperationException("El margen de ganancia deseado de cada producto debe ser mayor o igual que 0 y menor que 100.");
            }
            // calculate bases for apportionment
            decimal totalFobLocal = 0;
            decimal totalWeight = 0;
            decimal totalVolume = 0;
            decimal totalQuantity = 0;

            foreach (var d in order.Details)
            {
                var qty = d.Quantity;
                var fobOriginal = qty * d.UnitCostFob;
                var fobLocal = fobOriginal * exchangeRateUsed;
                totalFobLocal += fobLocal;

                totalWeight += qty * d.UnitWeight;
                totalVolume += qty * d.Product.Length.GetValueOrDefault() * d.Product.Width.GetValueOrDefault() * d.Product.Height.GetValueOrDefault();
                totalQuantity += qty;
            }

            //Validate distribution bases if corresponding methods are used
            bool hasWeightExpenses = order.Expenses.Any(e => e.ApportionmentMethod == ApportionmentMethod.ByWeight);
            if (hasWeightExpenses)
            {
                if (order.Details.Any(d => d.UnitWeight <= 0))
                    throw new InvalidOperationException("No se puede calcular el landed cost porque existen gastos distribuidos por peso y uno o más productos no tienen peso configurado.");
                if (totalWeight <= 0)
                    throw new InvalidOperationException("El peso total de la orden debe ser mayor que 0 para prorratear por peso.");
            }

            bool hasVolumeExpenses = order.Expenses.Any(e => e.ApportionmentMethod == ApportionmentMethod.ByVolume);
            if (hasVolumeExpenses)
            {
                if (order.Details.Any(d => d.Product.Length.GetValueOrDefault() <= 0 ||
                                          d.Product.Width.GetValueOrDefault() <= 0 ||
                                          d.Product.Height.GetValueOrDefault() <= 0))
                {
                    throw new InvalidOperationException("No se puede calcular el landed cost porque existen gastos distribuidos por volumen y uno o más productos no tienen largo, ancho y alto mayores que 0.");
                }
                if (totalVolume <= 0)
                    throw new InvalidOperationException("El volumen total de la orden debe ser mayor que 0 para prorratear por volumen.");

            }

            bool hasValueExpenses = order.Expenses.Any(e => e.ApportionmentMethod == ApportionmentMethod.ByValue);
            if (hasValueExpenses && totalFobLocal <= 0)
                throw new InvalidOperationException("No se puede calcular el landed cost porque existen gastos distribuidos por valor FOB y el FOB total de la orden es 0.");

            bool hasQuantityExpenses = order.Expenses.Any(e => e.ApportionmentMethod == ApportionmentMethod.ByQuantity);
            if (hasQuantityExpenses && totalQuantity <= 0)
                throw new InvalidOperationException("La cantidad total de productos debe ser mayor que 0 para prorratear por cantidad.");

            //Create a temporary list to hold calculated detail values before writing them
            var tempDetails = new List<(
                Guid ProductId,
                decimal Quantity,
                decimal TariffPct,
                decimal ExciseTaxPct,
                decimal FobOriginal,
                decimal FobLocal,
                decimal Freight,
                decimal Insurance,
                decimal CifLocal,
                decimal Duty,
                decimal SelectiveTax,
                decimal CustomsFee,
                decimal Itbis,
                decimal LocalExpenses,
                decimal ImportTotal,
                decimal UnitLandedCost,
                decimal DesiredMargin,
                decimal SuggestedPrice
            )>();

            foreach (var d in order.Details)
            {
                var qty = d.Quantity;
                var fobOriginal = Math.Round(qty * d.UnitCostFob, 2, MidpointRounding.AwayFromZero);
                var fobLocal = Math.Round(fobOriginal * exchangeRateUsed, 2, MidpointRounding.AwayFromZero);

                var detailWeight = qty * d.UnitWeight;
                var detailVolume = qty * d.Product.Length.GetValueOrDefault() * d.Product.Width.GetValueOrDefault() * d.Product.Height.GetValueOrDefault();
                var detailQuantity = qty;

                decimal freightAssigned = 0;
                decimal insuranceAssigned = 0;
                decimal localExpensesAssigned = 0;

                //Distribute all expenses proportionally to this detail
                foreach (var expense in order.Expenses)
                {
                    var apportionedLocal = ApportionExpense(expense, fobLocal, totalFobLocal, detailWeight, totalWeight, detailVolume, totalVolume, detailQuantity, totalQuantity);

                    apportionedLocal = Math.Round(apportionedLocal, 2, MidpointRounding.AwayFromZero);

                    if (expense.ExpenseType == ExpenseType.InternationalFreight)
                        freightAssigned += apportionedLocal;
                    else if (expense.ExpenseType == ExpenseType.InternationalInsurance)
                        insuranceAssigned += apportionedLocal;
                    else
                        localExpensesAssigned += apportionedLocal;

                }
                var cifLocal = Math.Round(fobLocal + freightAssigned + insuranceAssigned, 2, MidpointRounding.AwayFromZero);

                // Duty (Arancel)
                var tariffPct = d.CustomDutyPercentage;
                var dutyCalculated = Math.Round(cifLocal * (tariffPct / 100m), 2, MidpointRounding.AwayFromZero);

                // Selective Tax (Selectivo Consumo)
                var excisePct = d.Product.TariffCategory.ApplyExciseTax ? d.Product.TariffCategory.ExciseTaxPercentage : 0m;
                var selectiveTaxCalculated = d.Product.TariffCategory.ApplyExciseTax
                    ? Math.Round(cifLocal * (excisePct / 100m), 2, MidpointRounding.AwayFromZero)
                    : 0m;

                // Customs Service Fee (Tasa Servicio)
                var customsFeeCalculated = Math.Round(cifLocal * (taxConfig.CustomsServiceFeePercentage / 100m), 2, MidpointRounding.AwayFromZero);

                // ITBIS (VAT)
                decimal itbisCalculated = 0;
                if (d.Product.TariffCategory.ApplyITBIS)
                {
                    var itbisBase = cifLocal + dutyCalculated + selectiveTaxCalculated + customsFeeCalculated;
                    itbisCalculated = Math.Round(itbisBase * (taxConfig.GeneralItbisPercentage / 100m), 2, MidpointRounding.AwayFromZero);
                }

                // Total Cost & Landed Unit Cost
                var importTotalCost = fobLocal + freightAssigned + insuranceAssigned + dutyCalculated + selectiveTaxCalculated + customsFeeCalculated + itbisCalculated + localExpensesAssigned;
                var unitLandedCost = qty == 0 ? 0m : Math.Round(importTotalCost / qty, 2, MidpointRounding.AwayFromZero);

                // Suggested Price
                var suggestedPrice = d.DesiredProfitMargin == 100m
                    ? 0m
                    : Math.Round(unitLandedCost / (1m - (d.DesiredProfitMargin / 100m)), 2, MidpointRounding.AwayFromZero);

                tempDetails.Add((
                    d.ProductId,
                    qty,
                    tariffPct,
                    excisePct,
                    fobOriginal,
                    fobLocal,
                    freightAssigned,
                    insuranceAssigned,
                    cifLocal,
                    dutyCalculated,
                    selectiveTaxCalculated,
                    customsFeeCalculated,
                    itbisCalculated,
                    localExpensesAssigned,
                    importTotalCost,
                    unitLandedCost,
                    d.DesiredProfitMargin,
                    suggestedPrice
        ));
            }

            //  Sum all detail values to obtain the header's total values
            decimal fobTotalOriginalSum = tempDetails.Sum(x => x.FobOriginal);
            decimal fobTotalLocalSum = tempDetails.Sum(x => x.FobLocal);
            decimal freightTotalSum = tempDetails.Sum(x => x.Freight);
            decimal insuranceTotalSum = tempDetails.Sum(x => x.Insurance);
            decimal cifTotalSum = tempDetails.Sum(x => x.CifLocal);
            decimal dutyTotalSum = tempDetails.Sum(x => x.Duty);
            decimal selectiveTaxTotalSum = tempDetails.Sum(x => x.SelectiveTax);
            decimal customsFeeTotalSum = tempDetails.Sum(x => x.CustomsFee);
            decimal itbisTotalSum = tempDetails.Sum(x => x.Itbis);
            decimal localExpensesTotalSum = tempDetails.Sum(x => x.LocalExpenses);
            decimal importTotalSum = tempDetails.Sum(x => x.ImportTotal);

            //  Instantiate the LandedCostCalculation header
            var calculation = new LandedCostCalculation(
                importOrderId: order.Id,
                localCurrencyId: localCurrency.Id,
                exchangeRateUsed: exchangeRateUsed,
                generalItbisPct: taxConfig.GeneralItbisPercentage,
                customsServiceFeePct: taxConfig.CustomsServiceFeePercentage,
                fobTotalOriginal: fobTotalOriginalSum,
                fobTotalLocal: fobTotalLocalSum,
                freightTotal: freightTotalSum,
                insuranceTotal: insuranceTotalSum,
                cifTotal: cifTotalSum,
                dutyTotal: dutyTotalSum,
                selectiveTaxTotal: selectiveTaxTotalSum,
                customsFeeTotal: customsFeeTotalSum,
                itbisTotal: itbisTotalSum,
                localExpensesTotal: localExpensesTotalSum,
                importTotal: importTotalSum,
                totalQuantity: totalQuantity
            );

            //  Create and link each detail calculation entity
            foreach (var t in tempDetails)
            {
                var calcDetail = new LandedCostCalculationDetail(
                    productId: t.ProductId,
                    landedCostCalculationId: calculation.Id,
                    quantity: t.Quantity,
                    tariffPct: t.TariffPct,
                    exciseTaxPct: t.ExciseTaxPct,
                    fobOriginal: t.FobOriginal,
                    fobLocal: t.FobLocal,
                    freight: t.Freight,
                    insurance: t.Insurance,
                    cifLocal: t.CifLocal,
                    duty: t.Duty,
                    selectiveTax: t.SelectiveTax,
                    customsFee: t.CustomsFee,
                    itbis: t.Itbis,
                    localExpenses: t.LocalExpenses,
                    importTotal: t.ImportTotal,
                    unitLandedCost: t.UnitLandedCost,
                    desiredMargin: t.DesiredMargin,
                    suggestedPrice: t.SuggestedPrice
                );
                calculation.AddDetail(calcDetail);
            }

            // Persist to DB & transition order status
            await _unitOfWork.LandedCostCalculations.AddAsync(calculation);
            order.MarkAsCalculated();

            await _unitOfWork.CompleteAsync();

            //  Return the mapped calculation DTO
            var savedCalculation = await _unitOfWork.LandedCostCalculations.GetByIdWithDetailsAsync(calculation.Id)
                ?? throw new InvalidOperationException("Failed to retrieve saved calculation.");

            return MapToDto(savedCalculation);

        }

        public async Task<List<LandedCostCalculationDto>> GetAllByOrderIdAsync(Guid importOrderId)
        {
            var calculations = await _unitOfWork.LandedCostCalculations.GetAllByOrderIdAsync(importOrderId);
            return calculations.Select(MapToDto).ToList();
        }

        public async Task<LandedCostCalculationDto?> GetByIdAsync(Guid id)
        {
            var calculation = await _unitOfWork.LandedCostCalculations.GetByIdWithDetailsAsync(id);
            return calculation == null ? null : MapToDto(calculation);
        }

        //helper
        private static decimal ApportionExpense(
     ImportOrderExpense expense,
     decimal detailFobLocal, decimal totalFobLocal,
     decimal detailWeight, decimal totalWeight,
     decimal detailVolume, decimal totalVolume,
     decimal detailQuantity, decimal totalQuantity)
        {
            var expenseAmountLocal = expense.AmountInLocalCurrency;
            var share = expense.ApportionmentMethod switch
            {
                ApportionmentMethod.ByValue => totalFobLocal == 0 ? 0 : detailFobLocal / totalFobLocal,
                ApportionmentMethod.ByWeight => totalWeight == 0 ? 0 : detailWeight / totalWeight,
                ApportionmentMethod.ByVolume => totalVolume == 0 ? 0 : detailVolume / totalVolume,
                ApportionmentMethod.ByQuantity => totalQuantity == 0 ? 0 : detailQuantity / totalQuantity,
                _ => totalFobLocal == 0 ? 0 : detailFobLocal / totalFobLocal
            };

            return expenseAmountLocal * share;
        }

        private static LandedCostCalculationDto MapToDto(LandedCostCalculation c) => new()
        {
            Id = c.Id,
            ImportOrderId = c.ImportOrderId,
            OrderNumber = c.ImportOrder?.OrderNumber ?? string.Empty,
            CalculationDate = c.CalculationDate,
            LocalCurrencyId = c.LocalCurrencyId,
            LocalCurrencyCode = c.LocalCurrency?.ISOCode ?? string.Empty,
            ExchangeRateUsed = c.ExchangeRateUsed,
            GeneralItbisPercentageUsed = c.GeneralItbisPercentageUsed,
            CustomsServiceFeePercentageUsed = c.CustomsServiceFeePercentageUsed,
            FobTotalOriginal = c.FobTotalOriginal,
            FobTotalLocal = c.FobTotalLocal,
            FreightTotalLocal = c.FreightTotalLocal,
            InsuranceTotalLocal = c.InsuranceTotalLocal,
            CifTotalLocal = c.CifTotalLocal,
            DutyTotalLocal = c.DutyTotalLocal,
            SelectiveTaxTotalLocal = c.SelectiveTaxTotalLocal,
            CustomsServiceFeeTotalLocal = c.CustomsServiceFeeTotalLocal,
            ItbisTotalLocal = c.ItbisTotalLocal,
            LocalExpensesTotalLocal = c.LocalExpensesTotalLocal,
            ImportTotalCost = c.ImportTotalCost,
            TotalQuantity = c.TotalQuantity,
            Details = c.Details?.Select(MapDetailToDto).ToList() ?? new()
        };

        private static LandedCostCalculationDetailDto MapDetailToDto(LandedCostCalculationDetail d) => new()
        {
            Id = d.Id,
            LandedCostCalculationId = d.LandedCostCalculationId,
            ProductId = d.ProductId,
            ProductName = d.Product?.Name ?? string.Empty,
            ProductCodeReference = d.Product?.CodeReference ?? string.Empty,
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
        };

    }
}