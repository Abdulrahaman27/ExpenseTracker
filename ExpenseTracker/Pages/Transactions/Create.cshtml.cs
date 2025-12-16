using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace ExpenseTracker.Pages.Transactions
{

    public class CreateModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            ITransactionService transactionService,
            ICategoryService categoryService,
            ILogger<CreateModel> logger)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [BindProperty]
        public Transaction Transaction { get; set; } = new();

        public List<Category> Categories { get; private set; } = new();

        [BindProperty]
        public string Action { get; set; } = "create";

        /* ============================
           GET
        ============================= */
        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();

            Transaction.Date = DateTime.Now;
            Transaction.Type = TransactionType.Expense;
        }

        /* ============================
           POST
        ============================= */
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategoriesAsync();

            _logger.LogInformation(
                "Create Transaction POST → Desc={Description}, Amount={Amount}, Type={Type}, CategoryId={CategoryId}",
                Transaction.Description,
                Transaction.Amount,
                Transaction.Type,
                Transaction.CategoryId
            );

            if (!ModelState.IsValid)
            {
                LogModelStateErrors();
                return Page();
            }

            SetAuditFields();

            try
            {
                await _transactionService.CreateTransactionAsync(Transaction);

                TempData["SuccessMessage"] =
                    $"Transaction '{Transaction.Description}' created successfully.";

                return Action == "createAndAddAnother"
                    ? RedirectToPage("Create")
                    : RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create transaction");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the transaction.");
                return Page();
            }
        }

        /* ============================
           Helpers
        ============================= */
        private async Task LoadCategoriesAsync()
        {
            Categories = await _categoryService.GetCategoriesAsync();
        }

        private void SetAuditFields()
        {
            var now = DateTime.Now;

            Transaction.CreatedDate = now;
            Transaction.ModifiedDate = now;

            if (Transaction.Date == default)
            {
                Transaction.Date = now;
            }
        }

        private void LogModelStateErrors()
        {
            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    _logger.LogWarning(
                        "Validation error → Field: {Field}, Error: {Error}",
                        entry.Key,
                        error.ErrorMessage
                    );
                }
            }
        }
    }
}
