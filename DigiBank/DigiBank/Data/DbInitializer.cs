using DigiBank.Models;
using System;
using System.Linq;

namespace DigiBank.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DigiBankContext context)
        {
            context.Database.EnsureCreated();

            // Verificar se já existem clientes no banco
            if (context.Clientes.Any())
            {
                return; // DB já foi populado
            }

            // Criar clientes iniciais
            var clientes = new Cliente[]
            {
                new Cliente
                {
                    Nome = "João Silva",
                    CPF = "12345678901",
                    Email = "joao@email.com",
                    DataNascimento = new DateTime(1980, 1, 1),
                    Endereco = "Rua A, 123",
                    Telefone = "11999999999"
                },
                new Cliente
                {
                    Nome = "Maria Souza",
                    CPF = "98765432109",
                    Email = "maria@email.com",
                    DataNascimento = new DateTime(1985, 5, 15),
                    Endereco = "Av. B, 456",
                    Telefone = "11988888888"
                }
            };

            context.Clientes.AddRange(clientes);
            context.SaveChanges();

            // Criar contas iniciais
            var contas = new Conta[]
            {
                new Conta
                {
                    Tipo = TipoConta.Corrente,
                    Saldo = 1000.00m,
                    ClienteId = clientes[0].Id
                },
                new Conta
                {
                    Tipo = TipoConta.Poupanca,
                    Saldo = 5000.00m,
                    ClienteId = clientes[0].Id
                },
                new Conta
                {
                    Tipo = TipoConta.Corrente,
                    Saldo = 2500.00m,
                    ClienteId = clientes[1].Id
                }
            };

            context.Contas.AddRange(contas);
            context.SaveChanges();

            // Criar transações iniciais
            var transacoes = new Transacao[]
            {
                new Transacao
                {
                    Tipo = TipoTransacao.Deposito,
                    Valor = 1000.00m,
                    Descricao = "Depósito inicial",
                    ContaId = contas[0].Id
                },
                new Transacao
                {
                    Tipo = TipoTransacao.Deposito,
                    Valor = 5000.00m,
                    Descricao = "Depósito inicial",
                    ContaId = contas[1].Id
                },
                new Transacao
                {
                    Tipo = TipoTransacao.Deposito,
                    Valor = 2500.00m,
                    Descricao = "Depósito inicial",
                    ContaId = contas[2].Id
                }
            };

            context.Transacoes.AddRange(transacoes);
            context.SaveChanges();
        }
    }
}