using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Database.Entities
{
    public class ImportOrder
    {
        public Guid Id { get; private set; }
        public string OrderNumber { get; private set; } = string.Empty;
        public DateTime OrderDate { get; private set; }
        public ImportOrderStatus Status { get; private set; }
        public TransportMode TransportMode { get; private set; }

        // Core Relationships (Foreign Keys)
        public Guid ImporterId { get; private set; }
        public Guid SupplierId { get; private set; }
        public Guid CountryId { get; private set; }
        public Guid CurrencyId { get; private set; }
        public decimal ExchangeRateValue { get; private set; }



        // Navigation Properties
        public Importer Importer { get; private set; } = null!;
        public Supplier Supplier { get; private set; } = null!;
        public Country Country { get; private set; } = null!; // <-- Navegación al País de Origen
        public Currency Currency { get; private set; } = null!;

        public List<ImportOrderDetail> Details { get; private set; }
        public List<ImportOrderExpense> Expenses { get; private set; }

        // Private constructor for EF Core hydration
        private ImportOrder()
        {
            Details = new List<ImportOrderDetail>();
            Expenses = new List<ImportOrderExpense>();
        }

        // Public constructor for business logic instantiation
        public ImportOrder(string orderNumber, DateTime orderDate, Guid importerId, Guid supplierId, Guid countryId, Guid currencyId, decimal exchangeRateValue, TransportMode transportMode)
        {
            Id = Guid.NewGuid();
            OrderNumber = orderNumber.Trim().ToUpper(); // Validación del trim y mayúsculas
            OrderDate = orderDate;
            ImporterId = importerId;
            SupplierId = supplierId;
            CountryId = countryId;
            CurrencyId = currencyId;
            ExchangeRateValue = exchangeRateValue;
            TransportMode = transportMode;
            Status = ImportOrderStatus.Open; // Estado inicial "Abierta" 

            Details = new List<ImportOrderDetail>();
            Expenses = new List<ImportOrderExpense>();
        }

        // Domain method to update header in Open status
        public void UpdateHeader(string orderNumber, DateTime orderDate, Guid importerId, Guid supplierId, Guid countryId, Guid currencyId, decimal exchangeRateValue, TransportMode transportMode)
        {
            if (Status != ImportOrderStatus.Open)
                throw new InvalidOperationException("No se puede editar una orden que no esté en estado Abierta.");

            OrderNumber = orderNumber.Trim().ToUpper();
            OrderDate = orderDate;
            ImporterId = importerId;
            SupplierId = supplierId;
            CountryId = countryId;
            CurrencyId = currencyId;
            ExchangeRateValue = exchangeRateValue;
            TransportMode = transportMode;
        }


        // Transition from Calculated to Closed
        public void CloseOrder()
        {
            if (Status != ImportOrderStatus.Calculated)
                throw new InvalidOperationException("La orden debe calcularse oficialmente antes de poder cerrarse.");

            Status = ImportOrderStatus.Closed;
        }

        // Transition from any state except Closed to Cancelled
        public void CancelOrder()
        {
            if (Status == ImportOrderStatus.Closed)
                throw new InvalidOperationException("Una orden Cerrada ya no puede ser cancelada.");

            Status = ImportOrderStatus.Cancelled;
        }
        // Transition from Open to Calculated after a landed cost calculation is saved
        public void MarkAsCalculated()
        {
            if (Status != ImportOrderStatus.Open)
                throw new InvalidOperationException("Only Open orders can be marked as Calculated.");
            Status = ImportOrderStatus.Calculated;
        }
    }
}