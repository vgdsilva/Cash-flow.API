using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Entities;

public class CreditCard : EntityControl
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string CardNumberHash { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public int BillingCicleDay { get; set; }


    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
}
