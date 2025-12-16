
using ExpenseTracker.Data;
using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Id)
                .ToListAsync();
        }
        public async Task<Category> GetCategoryAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryAsync(id);
            if (category != null)
            {
                //check if category is used in transactions
                var hasTransactions = await _context.Transactions.AnyAsync(t => t.CategoryId == id);

                if (hasTransactions)
                {
                    throw new InvalidOperationException("Cannot delete category that is ued in transactions");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Category>> GetCategoriesByTypeAsync(TransactionType type)
        {
            return await _context.Categories
                .Where(c => c.Type == type)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
