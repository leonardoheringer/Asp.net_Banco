using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Banco.Data;
using Banco.Models.AccountModels;
using Banco.Models.ViewModels;
using Banco.Services;

namespace Banco.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public TransactionController(
            ApplicationDbContext context, 
            IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var transactions = await _context.Transactions
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Where(t => t.FromAccount.UserId == userId || 
                           t.ToAccount.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Take(50)
                .Select(t => new TransactionViewModel
                {
                    Id = t.Id,
                    Amount = t.FromAccount.UserId == userId ? -t.Amount : t.Amount,
                    Date = t.Date,
                    Description = t.Description,
                    FromAccount = t.FromAccount?.AccountNumber,
                    ToAccount = t.ToAccount?.AccountNumber,
                    Type = t.Type.ToString()
                })
                .ToListAsync();

            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            var userId = GetCurrentUserId();
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);
                
            ViewBag.FromAccount = account?.AccountNumber;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(TransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var fromAccount = await _context.Accounts
                        .FirstOrDefaultAsync(a => a.UserId == userId);
                        
                    if (fromAccount == null)
                        return NotFound();

                    await _accountService.TransferFunds(
                        fromAccount.Id,
                        model.ToAccount!,
                        model.Amount,
                        model.Description!);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var userIdForView = GetCurrentUserId();
            var accountForView = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userIdForView);
                
            ViewBag.FromAccount = accountForView?.AccountNumber;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Deposit()
        {
            var userId = GetCurrentUserId();
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
                
            ViewBag.UserAccounts = accounts;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(TransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _context.Accounts
                        .FirstOrDefaultAsync(a => a.AccountNumber == model.ToAccount);
                        
                    if (account == null)
                        return NotFound();

                    await _accountService.Deposit(account.Id, model.Amount);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var userId = GetCurrentUserId();
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
                
            ViewBag.UserAccounts = accounts;
            return View(model);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}