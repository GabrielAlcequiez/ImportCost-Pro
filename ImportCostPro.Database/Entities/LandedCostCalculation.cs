namespace ImportCostPro.Database.Entities
{
    public class LandedCostCalculation
    {
        public Guid Id { get; private set; }
        public Guid ImportOrderId { get; private set; }
        public DateTime CalculationDate { get; private set; }
        
        // --- CONFIGURACIONES CONGELADAS (Históricas) ---
        public Guid LocalCurrencyId { get; private set; } // Moneda local usada
        public decimal ExchangeRateUsed { get; private set; } // Tasa de cambio usada para la orden
        public decimal GeneralItbisPercentageUsed { get; private set; } // ITBIS general usado
        public decimal CustomsServiceFeePercentageUsed { get; private set; } // Tasa aduanal general usada

        // --- RESUMEN GENERAL EN MONEDA LOCAL ---
        public decimal FobTotalOriginal { get; private set; } // FOB en la moneda de la orden
        public decimal FobTotalLocal { get; private set; }
        public decimal FreightTotalLocal { get; private set; }
        public decimal InsuranceTotalLocal { get; private set; }
        public decimal CifTotalLocal { get; private set; }
        public decimal DutyTotalLocal { get; private set; } // Total Arancel
        public decimal SelectiveTaxTotalLocal { get; private set; } // Total Selectivo
        public decimal CustomsServiceFeeTotalLocal { get; private set; } // Total Tasa Aduanal
        public decimal ItbisTotalLocal { get; private set; } // Total ITBIS
        public decimal LocalExpensesTotalLocal { get; private set; } // Total Gastos Locales
        public decimal ImportTotalCost { get; private set; } // Costo total de importación
        public decimal TotalQuantity { get; private set; } // Cantidad total importada

        // --- NAVEGACIÓN ---
        public ImportOrder ImportOrder { get; private set; } = null!;
        public Currency LocalCurrency { get; private set; } = null!;
        public List<LandedCostCalculationDetail> Details { get; private set; }

        private LandedCostCalculation() 
        { 
            Details = new List<LandedCostCalculationDetail>();
        }

        public LandedCostCalculation(
            Guid importOrderId, Guid localCurrencyId, decimal exchangeRateUsed, 
            decimal generalItbisPct, decimal customsServiceFeePct, 
            decimal fobTotalOriginal, decimal fobTotalLocal, decimal freightTotal, 
            decimal insuranceTotal, decimal cifTotal, decimal dutyTotal, 
            decimal selectiveTaxTotal, decimal customsFeeTotal, decimal itbisTotal, 
            decimal localExpensesTotal, decimal importTotal, decimal totalQuantity)
        {
            Id = Guid.NewGuid();
            ImportOrderId = importOrderId;
            CalculationDate = DateTime.UtcNow; // Fecha exacta del cálculo oficial
            
            LocalCurrencyId = localCurrencyId;
            ExchangeRateUsed = exchangeRateUsed;
            GeneralItbisPercentageUsed = generalItbisPct;
            CustomsServiceFeePercentageUsed = customsServiceFeePct;

            FobTotalOriginal = fobTotalOriginal;
            FobTotalLocal = fobTotalLocal;
            FreightTotalLocal = freightTotal;
            InsuranceTotalLocal = insuranceTotal;
            CifTotalLocal = cifTotal;
            DutyTotalLocal = dutyTotal;
            SelectiveTaxTotalLocal = selectiveTaxTotal;
            CustomsServiceFeeTotalLocal = customsFeeTotal;
            ItbisTotalLocal = itbisTotal;
            LocalExpensesTotalLocal = localExpensesTotal;
            ImportTotalCost = importTotal;
            TotalQuantity = totalQuantity;

            Details = new List<LandedCostCalculationDetail>();
        }

        // Método para agregar los detalles a la cabecera
        public void AddDetail(LandedCostCalculationDetail detail)
        {
            Details.Add(detail);
        }
    }
}