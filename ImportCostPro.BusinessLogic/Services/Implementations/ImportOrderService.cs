using FluentValidation;
using ImportCostPro.BusinessLogic.DTOs.ImportOrder;
using ImportCostPro.BusinessLogic.Services.Interfaces;
using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Entities.Enums;
using ImportCostPro.Database.Repositories.Interfaces;

namespace ImportCostPro.BusinessLogic.Services.Implementations
{
    public class ImportOrderService : IImportOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateImportOrderDto> _createValidator;
        private readonly IValidator<UpdateImportOrderDto> _updateValidator;
        private readonly IValidator<CreateImportOrderDetailDto> _createDetailValidator;
        private readonly IValidator<UpdateImportOrderDetailDto> _updateDetailValidator;
        private readonly IValidator<CreateImportOrderExpenseDto> _createExpenseValidator;
        private readonly IValidator<UpdateImportOrderExpenseDto> _updateExpenseValidator;

        public ImportOrderService(
            IUnitOfWork unitOfWork,
            IValidator<CreateImportOrderDto> createValidator,
            IValidator<UpdateImportOrderDto> updateValidator,
            IValidator<CreateImportOrderDetailDto> createDetailValidator,
            IValidator<UpdateImportOrderDetailDto> updateDetailValidator,
            IValidator<CreateImportOrderExpenseDto> createExpenseValidator,
            IValidator<UpdateImportOrderExpenseDto> updateExpenseValidator)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _createDetailValidator = createDetailValidator;
            _updateDetailValidator = updateDetailValidator;
            _createExpenseValidator = createExpenseValidator;
            _updateExpenseValidator = updateExpenseValidator;
        }

        // ─── Queries ─────────────────────────────────────────────────────────────

        public async Task<List<ImportOrderDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.ImportOrders.GetAllWithDetailsAsync();
            return orders.Select(MapToDto).ToList();
        }

        public async Task<ImportOrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _unitOfWork.ImportOrders.GetByIdWithDetailsAsync(id);
            return order == null ? null : MapToDto(order);
        }

        // ─── Order Header ─────────────────────────────────────────────────────────

        public async Task<ImportOrderDto> CreateAsync(CreateImportOrderDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _createValidator.ValidateAndThrowAsync(dto);

            var order = new ImportOrder(
                dto.OrderNumber.Trim(),
                dto.OrderDate,
                dto.ImporterId,
                dto.SupplierId,
                dto.CountryId,
                dto.CurrencyId,
                dto.ExchangeRateValue,
                dto.TransportMode);

            await _unitOfWork.ImportOrders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            // Reload with navigation properties
            var saved = await _unitOfWork.ImportOrders.GetByIdWithDetailsAsync(order.Id)
                ?? throw new InvalidOperationException("Failed to retrieve saved import order.");
            return MapToDto(saved);
        }

        public async Task<ImportOrderDto> UpdateHeaderAsync(Guid id, UpdateImportOrderDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _updateValidator.ValidateAndThrowAsync(dto);

            var order = await _unitOfWork.ImportOrders.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Import order not found.");

            // Entity method already throws InvalidOperationException if status != Open
            order.UpdateHeader(
                dto.OrderNumber.Trim(),
                dto.OrderDate,
                dto.ImporterId,
                dto.SupplierId,
                dto.CountryId,
                dto.CurrencyId,
                dto.ExchangeRateValue,
                dto.TransportMode);

            await _unitOfWork.CompleteAsync();

            var updated = await _unitOfWork.ImportOrders.GetByIdWithDetailsAsync(id);
            return MapToDto(updated!);
        }

        // ─── Details ──────────────────────────────────────────────────────────────

        public async Task<ImportOrderDetailDto> AddDetailAsync(CreateImportOrderDetailDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _createDetailValidator.ValidateAndThrowAsync(dto);

            // Guard: no duplicate product in same order
            if (await _unitOfWork.ImportOrders.ProductAlreadyInOrderAsync(dto.ImportOrderId, dto.ProductId))
                throw new InvalidOperationException("This product is already in the order. Update the existing line instead.");

            var detail = new ImportOrderDetail(
                dto.ImportOrderId,
                dto.ProductId,
                dto.Quantity,
                dto.UnitCostFob,
                dto.UnitWeight,
                dto.CustomDutyPercentage,
                dto.DesiredProfitMargin);

            await _unitOfWork.ImportOrders.AddDetailAsync(detail);
            await _unitOfWork.CompleteAsync();

            var saved = await _unitOfWork.ImportOrders.GetDetailByIdAsync(detail.Id)
                ?? throw new InvalidOperationException("Failed to retrieve saved detail.");
            return MapDetailToDto(saved);
        }

        public async Task<ImportOrderDetailDto> UpdateDetailAsync(Guid detailId, UpdateImportOrderDetailDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _updateDetailValidator.ValidateAndThrowAsync(dto);

            var detail = await _unitOfWork.ImportOrders.GetDetailByIdAsync(detailId)
                ?? throw new KeyNotFoundException("Order detail not found.");

            // Guard: parent order must be Open
            var order = await _unitOfWork.ImportOrders.GetByIdAsync(detail.ImportOrderId)
                ?? throw new KeyNotFoundException("Parent order not found.");
            if (order.Status != ImportOrderStatus.Open)
                throw new InvalidOperationException("Cannot modify details of an order that is not Open.");

            detail.UpdateDetail(dto.Quantity, dto.UnitCostFob, dto.UnitWeight, dto.CustomDutyPercentage, dto.DesiredProfitMargin);
            await _unitOfWork.CompleteAsync();

            var updated = await _unitOfWork.ImportOrders.GetDetailByIdAsync(detailId);
            return MapDetailToDto(updated!);
        }

        public async Task<bool> RemoveDetailAsync(Guid detailId)
        {
            var detail = await _unitOfWork.ImportOrders.GetDetailByIdAsync(detailId)
                ?? throw new KeyNotFoundException("Order detail not found.");

            var order = await _unitOfWork.ImportOrders.GetByIdAsync(detail.ImportOrderId)
                ?? throw new KeyNotFoundException("Parent order not found.");
            if (order.Status != ImportOrderStatus.Open)
                throw new InvalidOperationException("Cannot remove details from an order that is not Open.");

            await _unitOfWork.ImportOrders.RemoveDetailAsync(detailId);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ─── Expenses ─────────────────────────────────────────────────────────────

        public async Task<ImportOrderExpenseDto> AddExpenseAsync(CreateImportOrderExpenseDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _createExpenseValidator.ValidateAndThrowAsync(dto);

            var expense = new ImportOrderExpense(
                dto.ImportOrderId,
                dto.ExpenseType,
                dto.Amount,
                dto.CurrencyId,
                dto.ExchangeRateValue,
                dto.ApportionmentMethod,
                dto.ExpenseDate);

            await _unitOfWork.ImportOrders.AddExpenseAsync(expense);
            await _unitOfWork.CompleteAsync();

            var saved = await _unitOfWork.ImportOrders.GetExpenseByIdAsync(expense.Id)
                ?? throw new InvalidOperationException("Failed to retrieve saved expense.");
            return MapExpenseToDto(saved);
        }

        public async Task<ImportOrderExpenseDto> UpdateExpenseAsync(Guid expenseId, UpdateImportOrderExpenseDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            await _updateExpenseValidator.ValidateAndThrowAsync(dto);

            var expense = await _unitOfWork.ImportOrders.GetExpenseByIdAsync(expenseId)
                ?? throw new KeyNotFoundException("Expense not found.");

            var order = await _unitOfWork.ImportOrders.GetByIdAsync(expense.ImportOrderId)
                ?? throw new KeyNotFoundException("Parent order not found.");
            if (order.Status != ImportOrderStatus.Open)
                throw new InvalidOperationException("Cannot modify expenses of an order that is not Open.");

            expense.UpdateExpense(dto.ExpenseType, dto.Amount, dto.CurrencyId, dto.ExchangeRateValue, dto.ApportionmentMethod, dto.ExpenseDate);
            await _unitOfWork.CompleteAsync();

            var updated = await _unitOfWork.ImportOrders.GetExpenseByIdAsync(expenseId);
            return MapExpenseToDto(updated!);
        }

        public async Task<bool> RemoveExpenseAsync(Guid expenseId)
        {
            var expense = await _unitOfWork.ImportOrders.GetExpenseByIdAsync(expenseId)
                ?? throw new KeyNotFoundException("Expense not found.");

            var order = await _unitOfWork.ImportOrders.GetByIdAsync(expense.ImportOrderId)
                ?? throw new KeyNotFoundException("Parent order not found.");
            if (order.Status != ImportOrderStatus.Open)
                throw new InvalidOperationException("Cannot remove expenses from an order that is not Open.");

            await _unitOfWork.ImportOrders.RemoveExpenseAsync(expenseId);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ─── Status Transitions ───────────────────────────────────────────────────

        public async Task<ImportOrderDto> CloseOrderAsync(Guid id)
        {
            var order = await _unitOfWork.ImportOrders.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Import order not found.");

            // Entity validates: must be Calculated — throws InvalidOperationException if not
            order.CloseOrder();
            await _unitOfWork.CompleteAsync();
            return MapToDto(order);
        }

        public async Task<ImportOrderDto> CancelOrderAsync(Guid id)
        {
            var order = await _unitOfWork.ImportOrders.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Import order not found.");

            // Entity validates: cannot cancel a Closed order
            order.CancelOrder();
            await _unitOfWork.CompleteAsync();
            return MapToDto(order);
        }

        // ─── Mapping Helpers ──────────────────────────────────────────────────────

        private static ImportOrderDto MapToDto(ImportOrder o) => new()
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TransportMode = o.TransportMode,
            ImporterId = o.ImporterId,
            ImporterName = o.Importer?.Name ?? string.Empty,
            SupplierId = o.SupplierId,
            SupplierName = o.Supplier?.Name ?? string.Empty,
            CountryId = o.CountryId,
            CountryName = o.Country?.Name ?? string.Empty,
            CurrencyId = o.CurrencyId,
            CurrencyCode = o.Currency?.ISOCode ?? string.Empty,
            CurrencyName = o.Currency?.Name ?? string.Empty,
            ExchangeRateValue = o.ExchangeRateValue,
            Details = o.Details?.Select(MapDetailToDto).ToList() ?? new(),
            Expenses = o.Expenses?.Select(MapExpenseToDto).ToList() ?? new()
        };

        private static ImportOrderDetailDto MapDetailToDto(ImportOrderDetail d) => new()
        {
            Id = d.Id,
            ImportOrderId = d.ImportOrderId,
            ProductId = d.ProductId,
            ProductName = d.Product?.Name ?? string.Empty,
            ProductCodeReference = d.Product?.CodeReference ?? string.Empty,
            Quantity = d.Quantity,
            UnitCostFob = d.UnitCostFob,
            UnitWeight = d.UnitWeight,
            CustomDutyPercentage = d.CustomDutyPercentage,
            DesiredProfitMargin = d.DesiredProfitMargin
        };

        private static ImportOrderExpenseDto MapExpenseToDto(ImportOrderExpense e) => new()
        {
            Id = e.Id,
            ImportOrderId = e.ImportOrderId,
            CurrencyId = e.CurrencyId,
            CurrencyCode = e.Currency?.ISOCode ?? string.Empty,
            ExpenseType = e.ExpenseType,
            Amount = e.Amount,
            ExchangeRateValue = e.ExchangeRateValue,
            ApportionmentMethod = e.ApportionmentMethod,
            ExpenseDate = e.ExpenseDate
        };
    }
}
