using CashFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Entities;

public class CashFlowUser : EntityControl
{
    public Guid CashFlowId { get; set; } = Guid.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
    public AccessPermissionsEnum Permission { get; set; } = AccessPermissionsEnum.EDIT;


    public virtual CashFlow CashFlow { get; set; }
    public virtual User User { get; set; }
}
