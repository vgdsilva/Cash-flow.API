using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CashFlow.Domain.Entities;

public class EntityControl
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [NotMapped]
    public string __TableName => GetType().Name.ToUpper();
}
