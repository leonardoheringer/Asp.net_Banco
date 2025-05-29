using System.Collections.Generic;
using System.Reflection.Emit;
using DigiBank.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiBank.Data
{
    public class DigiBankContext : DbContext
    {
        public DigiBankContext(DbContextOptions<DigiBankContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração das entidades
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasIndex(c => c.CPF).IsUnique();
                entity.Property(c => c.DataCadastro).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Conta>(entity =>
            {
                entity.HasIndex(c => c.Numero).IsUnique();
                entity.Property(c => c.DataAbertura).HasDefaultValueSql("GETDATE()");
                entity.Property(c => c.Saldo).HasColumnType("decimal(18,2)");

                entity.HasOne(c => c.Cliente)
                    .WithMany(c => c.Contas)
                    .HasForeignKey(c => c.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.Property(t => t.Data).HasDefaultValueSql("GETDATE()");
                entity.Property(t => t.Valor).HasColumnType("decimal(18,2)");

                entity.HasOne(t => t.Conta)
                    .WithMany(t => t.Transacoes)
                    .HasForeignKey(t => t.ContaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração para o enum TipoConta
            modelBuilder.Entity<Conta>()
                .Property(e => e.Tipo)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Configuração para o enum TipoTransacao
            modelBuilder.Entity<Transacao>()
                .Property(e => e.Tipo)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}