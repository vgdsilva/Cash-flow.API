using CashFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories;

public class CashFlowRepository : BaseRepository<Domain.Entities.CashFlow>
{
    public CashFlowRepository(CashFlowDbContext context) : base(context)
    {

    }
}
