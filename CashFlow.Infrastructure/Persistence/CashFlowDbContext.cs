using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Persistence;

public class CashFlowDbContext : DbContext
{
    public DbSet<Domain.Entities.CashFlow> CashFlows { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<CashFlowUser> CashFlowUsers { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public CashFlowDbContext(DbContextOptions<CashFlowDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração do relacionamento muitos para muitos entre User e CashFlow
        modelBuilder.Entity<CashFlowUser>()
            .HasKey(cfu => cfu.Id); // Define uma chave primária para a entidade de junção

        //modelBuilder.Entity<CashFlowUser>()
        //    .HasOne(cfu => cfu.CashFlow)
        //    .WithMany(cf => cf.CashFlowUsers)
        //    .HasForeignKey(cfu => cfu.CashFlowId)
        //    .OnDelete(DeleteBehavior.Cascade); 
        // Define o comportamento em caso de exclusão do CashFlow

        //modelBuilder.Entity<CashFlowUser>()
        //    .HasOne(cfu => cfu.User)
        //    .WithMany(u => u.CashFlowUsers)
        //    .HasForeignKey(cfu => cfu.UserId)
        //    .OnDelete(DeleteBehavior.Cascade); 
        // Define o comportamento em caso de exclusão do User

        // Configuração do relacionamento um para muitos entre User e CreditCard
        //modelBuilder.Entity<CreditCard>()
        //    .HasOne(cc => cc.User)
        //    .WithMany(u => u.CreditCards)
        //    .HasForeignKey(cc => cc.UserId)
        //    .OnDelete(DeleteBehavior.Cascade);

        // Configuração do relacionamento um para muitos entre CashFlow e Transaction
        //modelBuilder.Entity<Transaction>()
        //    .HasOne(t => t.CashFlow)
        //    .WithMany(cf => cf.Transactions)
        //    .HasForeignKey(t => t.CashFlowId)
        //    .OnDelete(DeleteBehavior.Cascade);

        // Configuração do relacionamento opcional um para muitos entre User e Transaction (quem criou)
        //modelBuilder.Entity<Transaction>()
        //    .HasOne(t => t.User)
        //    .WithMany(u => u.Transactions)
        //    .HasForeignKey(t => t.UserId)
        //    .OnDelete(DeleteBehavior.SetNull); 
        // Define o comportamento em caso de exclusão do User (a transação permanece, mas sem usuário criador)                                       
        // .IsRequired(false); // Garante que UserId pode ser nulo

        // Configuração do relacionamento opcional um para um entre Transaction e CreditCard
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.CreditCard)
            .WithMany() // Uma transação pode ter um cartão, um cartão pode ter muitas transações (mas geralmente queremos acessar as transações pelo CashFlow)
            .HasForeignKey(t => t.CreditCardId)
            .OnDelete(DeleteBehavior.SetNull); 
        // Define o comportamento em caso de exclusão do CreditCard (a transação permanece, mas sem cartão associado)
        // .IsRequired(false); // Garante que CreditCardId pode ser nulo

        // Exemplo de como configurar um enum como string no banco de dados (opcional)
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();

        modelBuilder.Entity<CashFlowUser>()
            .Property(cfu => cfu.Permission)
            .HasConversion<string>();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.PaymentMethod)
            .HasConversion<string>();


        base.OnModelCreating(modelBuilder);
    }
}
