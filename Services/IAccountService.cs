using System.Threading.Tasks;
using Banco.Models.AccountModels;
using Banco.Models.ViewModels;

namespace Banco.Services
{
    public interface IAccountService
    {
        Task<User> RegisterUser(RegisterViewModel model);
        Task<Account> CreateAccount(int userId);
        Task<Transaction> TransferFunds(int fromAccountId, string toAccountNumber, decimal amount, string description);
        Task<Transaction> Deposit(int toAccountId, decimal amount);
        Task<Transaction> Withdraw(int fromAccountId, decimal amount);
        Task<decimal> GetBalance(int accountId);
    }
}