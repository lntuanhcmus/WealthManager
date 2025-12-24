namespace WealthManager.Modules.Finance.Domain.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}
