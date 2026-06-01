using ImportCostPro.BusinessLogic.DTOs.ImportOrder;

namespace ImportCostPro.BusinessLogic.Services.Interfaces
{
    public interface IImportOrderService
    {
        Task<List<ImportOrderDto>> GetAllAsync();
        Task<ImportOrderDto?> GetByIdAsync(Guid id);
        Task<ImportOrderDto> CreateAsync(CreateImportOrderDto dto);
        Task<ImportOrderDto> UpdateHeaderAsync(Guid id, UpdateImportOrderDto dto);

        // Detail operations — only valid when order is Open
        Task<ImportOrderDetailDto> AddDetailAsync(CreateImportOrderDetailDto dto);
        Task<ImportOrderDetailDto> UpdateDetailAsync(Guid detailId, UpdateImportOrderDetailDto dto);
        Task<bool> RemoveDetailAsync(Guid detailId);

        // Expense operations — only valid when order is Open
        Task<ImportOrderExpenseDto> AddExpenseAsync(CreateImportOrderExpenseDto dto);
        Task<ImportOrderExpenseDto> UpdateExpenseAsync(Guid expenseId, UpdateImportOrderExpenseDto dto);
        Task<bool> RemoveExpenseAsync(Guid expenseId);

        // Status transitions (entity enforces the rules internally)
        Task<ImportOrderDto> CloseOrderAsync(Guid id);
        Task<ImportOrderDto> CancelOrderAsync(Guid id);
    }
}
