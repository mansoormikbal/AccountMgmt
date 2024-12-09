#nullable disable
using AccountMgmt.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using log4net;
using Newtonsoft.Json;

namespace AccountMgmt.Controllers
{
    public class AccountController : Controller
    {
        private readonly TransactionDbContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountController));

        public AccountController(TransactionDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                log.Info("TransactionController ==> Index ==> Retrieve All Transactions");

            }
            catch (Exception ex)
            {
                log.Error("TransactionController ==> Index ==> Retrieve All Transactions" + ex.Message.ToString());
            }

             return View(await _context.Accounts.ToListAsync());
        }

        public IActionResult Manage(int id = 0)
        {
            if (id == 0)
            {
                return View(new Account());
            }

            else

            {

                 return View(_context.Accounts.Find(id));
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage([Bind("AccountId,AccountNumber,AccountHolderName,Balance,CreatedAt")] Account accounts)
        {
            // Server Side Validation
            if (ModelState.IsValid)
            {
                log.Info("TransactionController ==> AddOrEdit ==> Generate Transactions with Inputs:-" + JsonConvert.SerializeObject(accounts));
                if (accounts.AccountId == 0)
                {
                    var sanitizer = new HtmlSanitizer();
                    var sanitizedAccountNumber = sanitizer.Sanitize(accounts.AccountNumber);
                    var sanitizedAccountHolderName = sanitizer.Sanitize(accounts.AccountHolderName);
                    var sanitizedBalance = sanitizer.Sanitize(Convert.ToString(accounts.Balance));


                    accounts.CreatedAt = DateTime.Now;


                    _context.Add(accounts);
                }
                else
                    _context.Update(accounts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(accounts);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accounts = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(accounts);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
