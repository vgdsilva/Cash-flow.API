using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Entities;

public class User : EntityControl
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;


    [NotMapped]
    public List<Transaction> Transactions { get; set; }

    [NotMapped]
    public List<CreditCard> CreditCards { get; set; }

}
