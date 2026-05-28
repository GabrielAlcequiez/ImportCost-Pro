namespace ImportCostPro.Database.Entities
{
    public class ImportOrderDetail
    {
        // Identification and Foreign Keys
        public Guid Id { get; private set; }
        public Guid ImportOrderId { get; private set; }
        public Guid ProductId { get; private set; }

        // Input Fields (Inputs)
        public decimal Quantity { get; private set; }
        public decimal UnitCostFob { get; private set; }
        public decimal UnitWeight { get; private set; }
        public decimal CustomDutyPercentage { get; private set; } // Frozen from TariffCategory
        public decimal DesiredProfitMargin { get; private set; } // En porcentaje (ej. 30 para 30%)

        // Calculation Results (Nullables until calculated)
        public decimal? FobLocal { get; private set; }
        public decimal? FreightAssigned { get; private set; }
        public decimal? InsuranceAssigned { get; private set; }
        public decimal? CifLocal { get; private set; }
        public decimal? DutyCalculated { get; private set; } // Arancel calculado
        public decimal? SelectiveTaxCalculated { get; private set; }
        public decimal? CustomsServiceFeeCalculated { get; private set; }
        public decimal? ItbisCalculated { get; private set; }
        public decimal? LocalExpensesAssigned { get; private set; }
        public decimal? ImportTotalCost { get; private set; }
        public decimal? UnitLandedCost { get; private set; } // Costo unitario importado
        public decimal? SuggestedSellingPrice { get; private set; } // Precio de venta sugerido

        // Navigation Properties
        public ImportOrder ImportOrder { get; private set; } = null!;
        public Product Product { get; private set; } = null!;

        // Private constructor for EF Core
        private ImportOrderDetail() { }

        // Public constructor for business logic instantiation
        public ImportOrderDetail(Guid importOrderId, Guid productId, decimal quantity, decimal unitCostFob, decimal unitWeight, decimal customDutyPercentage, decimal desiredProfitMargin)
        {
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que 0.", nameof(quantity));
            if (unitCostFob <= 0)
                throw new ArgumentException("El costo FOB debe ser mayor que 0.", nameof(unitCostFob));
            if (desiredProfitMargin < 0 || desiredProfitMargin >= 100)
                throw new ArgumentException("El margen deseado debe estar entre 0% y menor que 100%.", nameof(desiredProfitMargin));

            Id = Guid.NewGuid();
            ImportOrderId = importOrderId;
            ProductId = productId;
            Quantity = quantity;
            UnitCostFob = unitCostFob;
            UnitWeight = unitWeight;
            CustomDutyPercentage = customDutyPercentage;
            DesiredProfitMargin = desiredProfitMargin;
        }

        // Domain method to update details when order is in Open state
        public void UpdateDetail(decimal quantity, decimal unitCostFob, decimal unitWeight, decimal customDutyPercentage, decimal desiredProfitMargin)
        {
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que 0.", nameof(quantity));
            if (unitCostFob < 0)
                throw new ArgumentException("El costo FOB no puede ser negativo.", nameof(unitCostFob));
            if (desiredProfitMargin < 0 || desiredProfitMargin >= 100)
                throw new ArgumentException("El margen deseado debe estar entre 0% y menor que 100%.", nameof(desiredProfitMargin));

            Quantity = quantity;
            UnitCostFob = unitCostFob;
            UnitWeight = unitWeight;
            CustomDutyPercentage = customDutyPercentage;
            DesiredProfitMargin = desiredProfitMargin;
        }

        // Internal helper to set calculation results (called by Domain Service during landed cost calculation)
        public void SetCalculationResults(decimal fobLocal, decimal freight, decimal insurance, decimal cifLocal, decimal duty, decimal selectiveTax, decimal customsFee, decimal itbis, decimal localExpenses, decimal importTotal, decimal unitLanded, decimal suggestedPrice)
        {
            FobLocal = fobLocal;
            FreightAssigned = freight;
            InsuranceAssigned = insurance;
            CifLocal = cifLocal;
            DutyCalculated = duty;
            SelectiveTaxCalculated = selectiveTax;
            CustomsServiceFeeCalculated = customsFee;
            ItbisCalculated = itbis;
            LocalExpensesAssigned = localExpenses;
            ImportTotalCost = importTotal;
            UnitLandedCost = unitLanded;
            SuggestedSellingPrice = suggestedPrice;
        }
    }
}
