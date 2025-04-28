using System.ComponentModel.DataAnnotations.Schema;

namespace CashFlow.Domain.Entities;

public class CashFlow : EntityControl
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid OwnerId { get; set; } = Guid.Empty;


    [ForeignKey(nameof(OwnerId))]
    public virtual User Owner { get; set; }
}
