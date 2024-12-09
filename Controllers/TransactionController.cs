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
    
    public class TransactionController : Controller
    {
        private readonly TransactionDbContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(TransactionController));

        public TransactionController(TransactionDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            try
            {
                log.Info("TransactionController ==> Index ==> Retrieve All Transactions");

            }
            catch(Exception ex)
            {
                log.Error("TransactionController ==> Index ==> Retrieve All Transactions" + ex.Message.ToString());
            }

            var result = from customer in _context.Transactions
                         join order in _context.Accounts on customer.SourceAccountNumber equals Convert.ToString(order.AccountId)
                         join table2 in _context.Accounts on customer.DestinationAccountNumber equals Convert.ToString(table2.AccountId)
                         //join order in _context.Accounts on customer.SourceAccountNumber equals order.AccountId
                         select new AccountMgmt.Models.Transaction
                         {
                             SourceAccountNumber = order.AccountNumber,
                             Date =customer.Date,
                             Amount = customer.Amount,
                             DestinationAccountNumber = table2.AccountNumber,
                             TransactionId = customer.TransactionId
                         };



            //            var fromAccount = _context.Transactions
            //.Where(p => p.AccountNumber == sanitizedSenderAccountId).ToList();
            return View(result);
           // return View(await _context.Transactions.ToListAsync());
        }


        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Transaction());
            }
               
            else

            {
                var result = from customer in _context.Transactions
                             join order in _context.Accounts on customer.SourceAccountNumber equals Convert.ToString(order.AccountId)
                             join table2 in _context.Accounts on customer.DestinationAccountNumber equals Convert.ToString(table2.AccountId)
                             where customer.TransactionId == id
                             //join order in _context.Accounts on customer.SourceAccountNumber equals order.AccountId
                             select new AccountMgmt.Models.Transaction
                             {
                                 SourceAccountNumber = order.AccountNumber,
                                 Date = customer.Date,
                                 Amount = customer.Amount,
                                 DestinationAccountNumber = table2.AccountNumber,
                                 Narration = customer.Narration
                             };

               // return View(_context.Transactions.Find(id));
                return View(result.SingleOrDefault());
            }
                
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,SourceAccountNumber,DestinationAccountNumber,Amount,Narration,Date")] Transaction transaction)
        {
            // Server Side Validation
            if (ModelState.IsValid)
            {
                log.Info("TransactionController ==> AddOrEdit ==> Generate Transactions with Inputs:-" + JsonConvert.SerializeObject(transaction));
                if (transaction.TransactionId == 0)
                {
                    var sanitizer = new HtmlSanitizer();
                    var sanitizedSenderAccountId = sanitizer.Sanitize(transaction.SourceAccountNumber);
                    var sanitizedReceiverAccountId = sanitizer.Sanitize(transaction.DestinationAccountNumber);
                    var sanitizedAmount = sanitizer.Sanitize(Convert.ToString( transaction.Amount));
                    var sanitizedNarration = sanitizer.Sanitize(transaction.Narration);

                    bool isValidFrom = isValid_Bank_Acc_Number(transaction.SourceAccountNumber);
                    bool isValidTo = isValid_Bank_Acc_Number(transaction.DestinationAccountNumber);

                    if(!isValidFrom)
                    {
                        ModelState.AddModelError("", "Sender Account is Invalid.");
                        ViewBag.Accounts = new SelectList(_context.Accounts, "Id", "AccountHolderName");
                        return View();
                    }

                    if (!isValidTo)
                    {
                        ModelState.AddModelError("", "Receiver Account is Invalid.");
                        ViewBag.Accounts = new SelectList(_context.Accounts, "Id", "AccountHolderName");
                        return View();
                    }

                   

                    var fromAccount = _context.Accounts
.Where(p => p.AccountNumber == sanitizedSenderAccountId).ToList();

                    var toAccount = _context.Accounts
.Where(p => p.AccountNumber == sanitizedReceiverAccountId).ToList();

                    //var fromAccount1 = await _context.Accounts.FindAsync(1);

                    //var fromAccount = await _context.Accounts.FindAsync(Convert.ToInt32(transaction.SourceAccountNumber));
                    //var toAccount = await _context.Accounts.FindAsync(Convert.ToInt32(transaction.DestinationAccountNumber));

                    if (fromAccount.Count == 0)
                    {
                        ModelState.AddModelError("", "Sender Account is Invalid.");
                        ViewBag.Accounts = new SelectList(_context.Accounts, "Id", "AccountHolderName");
                        return View();
                    }

                    if (toAccount.Count == 0)
                    {
                        ModelState.AddModelError("", "Receiver Account is Invalid.");
                        ViewBag.Accounts = new SelectList(_context.Accounts, "Id", "AccountHolderName");
                        return View();
                    }

                    if (fromAccount[0].Balance < Convert.ToDouble(sanitizedAmount))
                    {
                        ModelState.AddModelError("", "Insufficient funds.");
                        ViewBag.Accounts = new SelectList(_context.Accounts, "Id", "AccountHolderName");
                        return View();
                    }

                    transaction.SourceAccountNumber = Convert.ToString( fromAccount[0].AccountId);
                    transaction.DestinationAccountNumber = Convert.ToString(toAccount[0].AccountId);

                    transaction.Date = DateTime.Now;

                    fromAccount[0].Balance = fromAccount[0].Balance - Convert.ToDouble(transaction.Amount);
                    toAccount[0].Balance = toAccount[0].Balance + Convert.ToDouble(transaction.Amount);
                    fromAccount[0].CreatedAt = DateTime.Now;
                    toAccount[0].CreatedAt = DateTime.Now;

                    //_context.Update(fromAccount);
                    //_context.Update(toAccount);

                    _context.Add(transaction);
                }
                else
                    _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }


        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public static bool isValid_Bank_Acc_Number(string str)
        {
            string strRegex = @"^[0-9]{12}$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(str))
                return (true);
            else
                return (false);
        }
    }
}
