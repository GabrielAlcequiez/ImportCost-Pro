using ImportCostPro.Database.Entities;

namespace ImportCostPro.Database.Repositories.Interfaces
{
    public interface IImportOrderRepository : IBaseRepository<ImportOrder>
    {
        // Full load with all navigation props (used for service operations)
        Task<IReadOnlyList<ImportOrder>> GetAllWithDetailsAsync();
        Task<ImportOrder?> GetByIdWithDetailsAsync(Guid id);
        Task<bool> ExistsByOrderNumberAsync(string orderNumber, Guid? excludeId = null);

        // Detail management (children of the aggregate)
        Task<ImportOrderDetail?> GetDetailByIdAsync(Guid detailId);
        Task AddDetailAsync(ImportOrderDetail detail);
        Task<bool> RemoveDetailAsync(Guid detailId);
        Task<bool> ProductAlreadyInOrderAsync(Guid importOrderId, Guid productId, Guid? excludeDetailId = null);

        // Expense management
        Task<ImportOrderExpense?> GetExpenseByIdAsync(Guid expenseId);
        Task AddExpenseAsync(ImportOrderExpense expense);
        Task<bool> RemoveExpenseAsync(Guid expenseId);
    }
}