using ExpenseTracker.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ExpenseTracker.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task<List<Category>> GetCategoriesByTypeAsync(TransactionType type);

        Task<int> GetTransactionCountAsync(int CategoryId);
    }
}
