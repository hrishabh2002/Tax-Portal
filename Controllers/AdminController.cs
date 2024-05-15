using AspAssignment.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.NetworkInformation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace AspAssignment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult TaxList(int page = 1,string selectedFinancialYear = null, string name = null, DateTime? selectedDeclarationdate=null)
        {
            const int pageSize = 3;

            var financialYears = _db.IncomeTaxes.Select(t => t.FinancialYear).Distinct().ToList();
            ViewBag.FinancialYears = financialYears;

            var declarationdate=_db.IncomeTaxes.Select(t=>t.DeclarationDate).Distinct().ToList();
            List<string> shortStringList = declarationdate.Select(dt => dt?.ToShortDateString()).Distinct().ToList();
            ViewBag.DeclarationDate = shortStringList;

            var taxQuery = _db.IncomeTaxes.Where(t => t.Freez && !t.Complete);
            

            if (!string.IsNullOrWhiteSpace(selectedFinancialYear))
            {
                taxQuery = taxQuery.Where(t => t.FinancialYear.Contains(selectedFinancialYear));
            }
            var selectedDateStr = selectedDeclarationdate?.ToShortDateString();

            if (!string.IsNullOrEmpty(selectedDateStr))
            {
                taxQuery = taxQuery.Where(i => i.DeclarationDate.HasValue &&
                    i.DeclarationDate.Value.Year == selectedDeclarationdate.Value.Year &&
                    i.DeclarationDate.Value.Month == selectedDeclarationdate.Value.Month &&
                    i.DeclarationDate.Value.Day == selectedDeclarationdate.Value.Day);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                taxQuery = taxQuery.Where(t => t.Name.Contains(name));
            }

            var taxList = taxQuery.OrderBy(t => t.FinancialYear)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)taxQuery.Count() / pageSize);

            return View(taxList);
        }


        public IActionResult FinalList(int page = 1, string financialYear = null, string name = null, DateTime? selectedDeclarationdate = null)
        {
            const int pageSize = 3;

            var financialYears = _db.IncomeTaxes.Select(t => t.FinancialYear).Distinct().ToList();
            ViewBag.FinancialYears = financialYears;

            var declarationdate = _db.IncomeTaxes.Select(t => t.DeclarationDate).Distinct().ToList();
            List<string> shortStringList = declarationdate.Select(dt => dt?.ToShortDateString()).Distinct().ToList();
            ViewBag.DeclarationDate = shortStringList;

            var taxQuery = _db.IncomeTaxes.Where(t => t.Complete);


            if (!string.IsNullOrWhiteSpace(financialYear))
            {
                taxQuery = taxQuery.Where(t => t.FinancialYear.Contains(financialYear));
            }

            var selectedDateStr = selectedDeclarationdate?.ToShortDateString();
            if (!string.IsNullOrEmpty(selectedDateStr))
            {
                taxQuery = taxQuery.Where(i => i.DeclarationDate.HasValue &&
                    i.DeclarationDate.Value.Year == selectedDeclarationdate.Value.Year &&
                    i.DeclarationDate.Value.Month == selectedDeclarationdate.Value.Month &&
                    i.DeclarationDate.Value.Day == selectedDeclarationdate.Value.Day);
            }


            if (!string.IsNullOrWhiteSpace(financialYear))
            {
                taxQuery = taxQuery.Where(t => t.FinancialYear.Contains(financialYear));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                taxQuery = taxQuery.Where(t => t.Name.Contains(name));
            }

            var taxList = taxQuery.OrderBy(t => t.FinancialYear)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)taxQuery.Count() / pageSize);

            return View(taxList);
        }



        [HttpPost]
        public IActionResult Unfreeze(int id)
        {
            var taxRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == id);
            if (taxRecord != null)
            {
                taxRecord.Freez = false;
                taxRecord.Req = false;
                _db.SaveChanges();
            }

            return RedirectToAction("TaxList");
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            var taxRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == id);
            if (taxRecord != null)
            {
                taxRecord.Freez = true; 
                taxRecord.Complete = true; 
                _db.SaveChanges();
            }

            return RedirectToAction("TaxList");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var taxRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == id);
            if (taxRecord != null)
            {
                taxRecord.Freez = false;
                taxRecord.Complete = false; 
                taxRecord.Req = false;
                taxRecord.Reject = true;
                _db.SaveChanges();
            }

            return RedirectToAction("TaxList");
        }

        [HttpGet]
        public IActionResult ViewDetails(int id)
        {
            var taxRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == id);
            if (taxRecord != null)
            {
                return View(taxRecord);
            }
            return RedirectToAction("TaxList");
        }



    }

}
