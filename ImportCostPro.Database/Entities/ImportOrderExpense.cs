using System;
using ImportCostPro.Database.Entities.Enums;

namespace ImportCostPro.Database.Entities
{
    public class ImportOrderExpense
    {
        // Identification and Foreign Keys
        public Guid Id { get; private set; }
        public Guid ImportOrderId { get; private set; }
        public Guid CurrencyId { get; private set; }

        // Expense Details
        public ExpenseType ExpenseType { get; private set; }
        public decimal Amount { get; private set; } // Monto en la moneda original del gasto
        public decimal ExchangeRateValue { get; private set; } // Tasa de cambio para convertir a la moneda local
        public ApportionmentMethod ApportionmentMethod { get; private set; }
        public DateTime ExpenseDate { get; private set; }
        public string? Description { get; private set; }

        // Navigation Properties
        public ImportOrder ImportOrder { get; private set; } = null!;
        public Currency Currency { get; private set; } = null!;

        // Calculated read-only property in local currency
        public decimal AmountInLocalCurrency => Amount * ExchangeRateValue;

        // Private constructor for EF Core
        private ImportOrderExpense() { }

        // Public constructor for business logic instantiation
        public ImportOrderExpense(Guid importOrderId, ExpenseType expenseType, decimal amount, Guid currencyId, decimal exchangeRateValue, ApportionmentMethod apportionmentMethod, DateTime expenseDate, string? description)
        {
            if (amount <= 0)
                throw new ArgumentException("El monto del gasto debe ser mayor que 0.", nameof(amount));
            if (exchangeRateValue <= 0)
                throw new ArgumentException("La tasa de cambio debe ser mayor que 0.", nameof(exchangeRateValue));

            Id = Guid.NewGuid();
            ImportOrderId = importOrderId;
            ExpenseType = expenseType;
            Amount = amount;
            CurrencyId = currencyId;
            ExchangeRateValue = exchangeRateValue;
            ApportionmentMethod = apportionmentMethod;
            ExpenseDate = expenseDate.Date; // Truncating time to keep date only
            Description = description?.Trim();
        }

        // Domain method to update expense
        public void UpdateExpense(ExpenseType expenseType, decimal amount, Guid currencyId, decimal exchangeRateValue, ApportionmentMethod apportionmentMethod, DateTime expenseDate, string? description)
        {
            if (amount <= 0)
                throw new ArgumentException("El monto del gasto debe ser mayor que 0.", nameof(amount));
            if (exchangeRateValue <= 0)
                throw new ArgumentException("La tasa de cambio debe ser mayor que 0.", nameof(exchangeRateValue));

            ExpenseType = expenseType;
            Amount = amount;
            CurrencyId = currencyId;
            ExchangeRateValue = exchangeRateValue;
            ApportionmentMethod = apportionmentMethod;
            ExpenseDate = expenseDate.Date;
            Description = description?.Trim();
        }
    }
}
