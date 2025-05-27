using System;
using System.Threading.Tasks;
using Banco.Data;
using Banco.Models.AccountModels;
using Banco.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Banco.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUser(RegisterViewModel model)
        {
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<Account> CreateAccount(int userId)
        {
            var account = new Account
            {
                AccountNumber = GenerateAccountNumber(),
                UserId = userId,
                Balance = 0
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<Transaction> TransferFunds(int fromAccountId, string toAccountNumber, decimal amount, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor da transferência deve ser positivo");

            var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
            if (fromAccount == null)
                throw new Exception("Conta de origem não encontrada");

            var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber);
            if (toAccount == null)
                throw new Exception("Conta de destino não encontrada");

            if (fromAccount.Balance < amount)
                throw new Exception("Saldo insuficiente");

            var transaction = new Transaction
            {
                Amount = amount,
                Description = description ?? "Transferência entre contas",
                FromAccountId = fromAccountId,
                ToAccountId = toAccount.Id,
                Type = TransactionType.Transfer,
                Date = DateTime.UtcNow
            };

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> Deposit(int toAccountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor do depósito deve ser positivo");

            var account = await _context.Accounts.FindAsync(toAccountId);
            if (account == null)
                throw new Exception("Conta não encontrada");

            var transaction = new Transaction
            {
                Amount = amount,
                Description = "Depósito em conta",
                ToAccountId = toAccountId,
                Type = TransactionType.Deposit,
                Date = DateTime.UtcNow
            };

            account.Balance += amount;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction> Withdraw(int fromAccountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor do saque deve ser positivo");

            var account = await _context.Accounts.FindAsync(fromAccountId);
            if (account == null)
                throw new Exception("Conta não encontrada");

            if (account.Balance < amount)
                throw new Exception("Saldo insuficiente");

            var transaction = new Transaction
            {
                Amount = amount,
                Description = "Saque em conta",
                FromAccountId = fromAccountId,
                Type = TransactionType.Withdrawal,
                Date = DateTime.UtcNow
            };

            account.Balance -= amount;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<decimal> GetBalance(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            return account?.Balance ?? throw new Exception("Conta não encontrada");
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            return random.Next(10000000, 99999999).ToString();
        }
    }
}