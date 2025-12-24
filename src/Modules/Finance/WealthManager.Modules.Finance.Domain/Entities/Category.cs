namespace WealthManager.Modules.Finance.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    
    // Navigation property
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
