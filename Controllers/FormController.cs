using AspAssignment.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.NetworkInformation;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace AspAssignment.Controllers
{
    [Authorize(Roles = "User")]
    public class FormController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public FormController(ApplicationDbContext db,UserManager<ApplicationUser>userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {  
            return View();
        }
        public async Task<IActionResult> Dashboard()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            ViewBag.Name = user.FirstName;
            ViewBag.Last = user.LastName;
            ViewBag.EmployeeId = user.EmployeeId;
            ViewBag.Pan = user.Pan;
            ViewBag.Address = user.Address;
            return View();
        }

        [HttpPost]
        public IActionResult Index(string year)
        {
            // Redirect to FormPage view with the selected year
            return RedirectToAction("FormPage", new { year = year });
        }



        [HttpGet]
        public async Task<IActionResult> FormPage(string year)
        {
            ApplicationUser user = await GetCurrentUserAsync();
            ViewBag.Name = user.FirstName+" "+ user.LastName;
            //ViewBag.Last = user.LastName;
            ViewBag.EmployeeId=user.EmployeeId;
            ViewBag.Pan=user.Pan;
            ViewBag.Address=user.Address;
           
            // Check if there is existing data for the selected year
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.FinancialYear == year && tax.userId == userId);

            

            if (existingRecord != null)
            {
                // If data exists, populate the model with existing values
                var model = new IncomeTaxDeductionForm
                {
                    Id = existingRecord.Id,
                    FinancialYear = existingRecord.FinancialYear,
                    Name = existingRecord.Name,
                    OtherIncome=existingRecord.OtherIncome,
                    SukanyaSamriddhi=existingRecord.SukanyaSamriddhi,
                    PPF_NS_ULIP=existingRecord.PPF_NS_ULIP,
                    LifeInsurancePremium=existingRecord.LifeInsurancePremium,
                    TuitionFeeForChild=existingRecord.TuitionFeeForChild,
                    BankFixedDeposit=existingRecord.BankFixedDeposit,
                    PrincipalAmountHousingLoan=existingRecord.PrincipalAmountHousingLoan,
                    NationalPensionScheme=existingRecord.NationalPensionScheme,
                    HigherEducationLoanInterest=existingRecord.HigherEducationLoanInterest,
                    InterestPaidOnHousingLoan=existingRecord.InterestPaidOnHousingLoan,
                    HouseRentPaid=existingRecord.HouseRentPaid,
                    TaxDeductedAtSource=existingRecord.TaxDeductedAtSource,
                    MediClaimInsurancePolicyPremium=existingRecord.MediClaimInsurancePolicyPremium,
                    PreventiveHealthCheckup=existingRecord.PreventiveHealthCheckup,
                    Flag=existingRecord.Flag,
                    LTA = existingRecord.LTA,
                    EducationAllowance = existingRecord.EducationAllowance
                };
               
                if (existingRecord.Complete)
                {
                    ViewBag.Status = "Approved";
                   
                }
                else if(existingRecord.Freez && !existingRecord.Complete){
                    ViewBag.Status = "Submitted";
                }
                else
                {
                    ViewBag.Status = "Submission Pending";
                }
                return View(model);
            }
            else
            {
                // If no existing data, create a new model with the selected year
                ViewBag.Status = "No Previous Record";
                var model = new IncomeTaxDeductionForm
                {
                    FinancialYear = year
                };

                return View(model);
            }
        }



        [HttpPost]
        public IActionResult FormPage(string submitButton, IncomeTaxDeductionForm model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                if (submitButton == "Save")
                {
                    model.userId = userId;

                    var existingRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == model.Id && tax.userId == userId && !tax.Freez && !tax.Complete);

                    if (existingRecord != null)
                    {
                        
                        _db.Entry(existingRecord).CurrentValues.SetValues(model);
                    }
                    else
                    {
                        _db.IncomeTaxes.Add(model);
                    }
                    _db.SaveChanges();

                  
                }
                else if (submitButton == "Submit")
                {
                    model.userId = userId;
                    model.Freez = true;
                    model.Flag = true;
                    model.DeclarationDate = DateTime.Now;
                    var existingRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.FinancialYear == model.FinancialYear && tax.Id == model.Id && tax.userId == userId);
                    if (existingRecord != null)
                    {

                        existingRecord.Freez = true;
                        existingRecord.Reject = false;
                        
                        _db.Entry(existingRecord).CurrentValues.SetValues(model);
                    }
                    else
                    {
                       
                        _db.IncomeTaxes.Add(model);
                    }
                    _db.SaveChanges();
                }
                else if (submitButton == "Edit")
                {

                    model.userId = userId;

                    var existingRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == model.Id && tax.userId == userId && !tax.Freez && !tax.Complete);

                    if (existingRecord != null)
                    {
                        
                        _db.Entry(existingRecord).CurrentValues.SetValues(model);
                    }
                    else
                    {
                        _db.IncomeTaxes.Add(model);
                    }
                    _db.SaveChanges();

                    
                }



                return RedirectToAction("FormList");
            }
            catch (Exception ex)
            {
             
                Console.WriteLine(ex.Message);

               
                return View(model);
            }
        }


        public IActionResult FormList()
        {
            // Get the logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch data from the database for the logged-in user
            var taxList = _db.IncomeTaxes.Where(tax => tax.userId == userId).ToList();

            return View(taxList);
        }
        public IActionResult FormStatus()
        {
            // Get the logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch data from the database for the logged-in user
            var taxList = _db.IncomeTaxes.Where(tax => tax.userId == userId && tax.Freez ).ToList();

            return View(taxList);
        }
        

       
        [HttpPost]
        public IActionResult RequestChange(int id, string message)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == id && tax.userId == userId && tax.Complete == false && tax.Freez == true);

            if (existingRecord != null)
            {
                existingRecord.Req = true;
                existingRecord.ReqMessage = message; 
               
                _db.SaveChanges();
            }

            return RedirectToAction("FormList");
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var taxRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == id && tax.userId == userId);

            if (taxRecord != null)
            {
                return View(taxRecord);
            }

            return RedirectToAction("FormList");
        }

        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingRecord = _db.IncomeTaxes.FirstOrDefault(tax => tax.Id == id && tax.userId == userId);

            if (existingRecord != null)
            {
                _db.IncomeTaxes.Remove(existingRecord);
                _db.SaveChanges();
            }

            return RedirectToAction("FormList");
        }



    }

}
