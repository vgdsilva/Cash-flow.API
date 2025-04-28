using CashFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Entities;

public class Transaction : EntityControl
{
    public string Description { get; set; } = string.Empty;
    
    public DateTime Date { get; set; } = DateTime.Now;
    
    public decimal Value { get; set; }
    
    public TransactionTypeEnum Type { get; set; } = TransactionTypeEnum.EXPENSE;
    
    public string Category { get; set; }
    
    public PaymentMethodEnum PaymentMethod { get; set; } = PaymentMethodEnum.CASH;
    
    public Guid CashFlowId { get; set; } = Guid.Empty;
    
    public Guid UserId { get; set; } = Guid.Empty;
    
    public Guid? CreditCardId { get; set; }

    public Transaction() { }


    [ForeignKey(nameof(CashFlowId))]
    public virtual CashFlow CashFlow { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    [ForeignKey(nameof(CreditCardId))]
    public virtual CreditCard CreditCard { get; set; }

}
