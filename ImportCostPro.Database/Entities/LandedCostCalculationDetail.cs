namespace ImportCostPro.Database.Entities
{
    public class LandedCostCalculationDetail
    {
        public Guid Id { get; private set; }
        public Guid LandedCostCalculationId { get; private set; }
        public Guid ProductId { get; private set; }

        // --- DATOS BASE DEL PRODUCTO ---
        public decimal Quantity { get; private set; }
        
        // --- CONFIGURACIONES CONGELADAS POR PRODUCTO ---
        public decimal TariffPercentageUsed { get; private set; } // Porcentaje de arancel usado
        public decimal ExciseTaxPercentageUsed { get; private set; } // Porcentaje de impuesto selectivo usado

        // --- CÁLCULOS DEL PRODUCTO ---
        public decimal FobOriginal { get; private set; }
        public decimal FobLocal { get; private set; }
        public decimal FreightAssigned { get; private set; }
        public decimal InsuranceAssigned { get; private set; }
        public decimal CifLocal { get; private set; }
        public decimal DutyCalculated { get; private set; } // Arancel calculado
        public decimal SelectiveTaxCalculated { get; private set; } // Selectivo calculado
        public decimal CustomsServiceFeeCalculated { get; private set; } // Tasa aduanal calculada
        public decimal ItbisCalculated { get; private set; } // ITBIS calculado
        public decimal LocalExpensesAssigned { get; private set; } // Gastos locales asignados
        public decimal ImportTotalCost { get; private set; } // Costo total importado
        public decimal UnitLandedCost { get; private set; } // Costo unitario importado
        
        // --- PRECIOS Y MÁRGENES ---
        public decimal DesiredProfitMargin { get; private set; }
        public decimal SuggestedSellingPrice { get; private set; } // Precio de venta sugerido

        // --- NAVEGACIÓN ---
        public LandedCostCalculation Calculation { get; private set; } = null!;
        public Product Product { get; private set; } = null!;

        private LandedCostCalculationDetail() { }

        public LandedCostCalculationDetail(
            Guid productId, Guid landedCostCalculationId, decimal quantity, decimal tariffPct, decimal exciseTaxPct,
            decimal fobOriginal, decimal fobLocal, decimal freight, decimal insurance, 
            decimal cifLocal, decimal duty, decimal selectiveTax, decimal customsFee, 
            decimal itbis, decimal localExpenses, decimal importTotal, 
            decimal unitLandedCost, decimal desiredMargin, decimal suggestedPrice)
        {
            Id = Guid.NewGuid();
            LandedCostCalculationId = landedCostCalculationId;
            ProductId = productId;
            Quantity = quantity;
            
            TariffPercentageUsed = tariffPct;
            ExciseTaxPercentageUsed = exciseTaxPct;

            FobOriginal = fobOriginal;
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
            UnitLandedCost = unitLandedCost;
            
            DesiredProfitMargin = desiredMargin;
            SuggestedSellingPrice = suggestedPrice;
        }
    }
}