﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Domain.Enums;
public enum TransactionTypeEnum
{
    INCOME,
    EXPENSE,
    TRANSFER,
    LOAN,
    INVESTMENT,
    DIVIDEND,
    INTEREST,
    REIMBURSEMENT,
    GIFT,
    OTHER
}
