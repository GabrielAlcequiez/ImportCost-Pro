using ImportCostPro.Database.Entities;
using ImportCostPro.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Database.Repositories.Implementations
{
    public class ImportOrderRepository(AppDbContext context) : BaseRepository<ImportOrder>(context), IImportOrderRepository
    {

        public async Task<IReadOnlyList<ImportOrder>> GetAllWithDetailsAsync()
        {
            return await _context.ImportOrders
                .Include(o => o.Importer)
                .Include(o => o.Supplier)
                .Include(o => o.Country)
                .Include(o => o.Currency)
                .Include(o => o.Details)
                    .ThenInclude(d => d.Product)
                      .ThenInclude(p => p.TariffCategory)
                .Include(o => o.Expenses)
                    .ThenInclude(e => e.Currency)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ImportOrder?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ImportOrders
                .Include(o => o.Importer)
                .Include(o => o.Supplier)
                .Include(o => o.Country)
                .Include(o => o.Currency)
                .Include(o => o.Details)
                    .ThenInclude(d => d.Product)
                      .ThenInclude(p => p.TariffCategory)
                .Include(o => o.Expenses)
                    .ThenInclude(e => e.Currency)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, Guid? excludeId = null)
        {
            var query = _context.ImportOrders
                .Where(o => o.OrderNumber == orderNumber.Trim().ToUpper());
            if (excludeId.HasValue)
                query = query.Where(o => o.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        // ─── Details ────────────────────────────────────────────────────────────

        public async Task<ImportOrderDetail?> GetDetailByIdAsync(Guid detailId)
        {
            return await _context.ImportOrderDetails
                .Include(d => d.Product)
                .FirstOrDefaultAsync(d => d.Id == detailId);
        }

        public async Task AddDetailAsync(ImportOrderDetail detail)
        {
            await _context.ImportOrderDetails.AddAsync(detail);
        }

        public async Task<bool> RemoveDetailAsync(Guid detailId)
        {
            var detail = await _context.ImportOrderDetails.FindAsync(detailId);
            if (detail == null) return false;
            _context.ImportOrderDetails.Remove(detail);
            return true;
        }

        public async Task<bool> ProductAlreadyInOrderAsync(Guid importOrderId, Guid productId, Guid? excludeDetailId = null)
        {
            var query = _context.ImportOrderDetails
                .Where(d => d.ImportOrderId == importOrderId && d.ProductId == productId);
            if (excludeDetailId.HasValue)
                query = query.Where(d => d.Id != excludeDetailId.Value);
            return await query.AnyAsync();
        }

        // ─── Expenses ────────────────────────────────────────────────────────────

        public async Task<ImportOrderExpense?> GetExpenseByIdAsync(Guid expenseId)
        {
            return await _context.ImportOrderExpenses
                .Include(e => e.Currency)
                .FirstOrDefaultAsync(e => e.Id == expenseId);
        }

        public async Task AddExpenseAsync(ImportOrderExpense expense)
        {
            await _context.ImportOrderExpenses.AddAsync(expense);
        }

        public async Task<bool> RemoveExpenseAsync(Guid expenseId)
        {
            var expense = await _context.ImportOrderExpenses.FindAsync(expenseId);
            if (expense == null) return false;
            _context.ImportOrderExpenses.Remove(expense);
            return true;
        }
    }
}