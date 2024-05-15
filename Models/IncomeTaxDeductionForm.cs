using System.ComponentModel.DataAnnotations;

namespace AspAssignment.Data
{
    public class IncomeTaxDeductionForm
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string FinancialYear {  get; set; }
        public decimal? OtherIncome { get; set; }

        public decimal? SukanyaSamriddhi { get; set; }

        public decimal? PPF_NS_ULIP { get; set; }

        public decimal? LifeInsurancePremium { get; set; }

        public decimal? TuitionFeeForChild { get; set; }

        public decimal? BankFixedDeposit { get; set; }

        public decimal? PrincipalAmountHousingLoan { get; set; }

        [Range(0, 50000, ErrorMessage = "Limit of 50K Exceeding")]
        public decimal? NationalPensionScheme { get; set; }

        public decimal? HigherEducationLoanInterest { get; set; }

        public decimal? InterestPaidOnHousingLoan { get; set; }

        public decimal? HouseRentPaid { get; set; }

        public decimal? TaxDeductedAtSource { get; set; }

        public decimal? MediClaimInsurancePolicyPremium { get; set; }

        [Range(0, 5000, ErrorMessage ="Limit of 5K Exceeding")]

        public decimal? PreventiveHealthCheckup { get; set; }

       
        public bool LTA { get; set; }=false;

        public bool EducationAllowance { get; set; } = false;

        public bool Freez { get; set;}=false;
        public bool Complete { get; set; }=false;
        public bool Req { get; set; } = false;

        public string userId { get; set; } = null;
        public bool Reject {  get; set; } = false;
        public string ? ReqMessage {  get; set; } = null;
        public bool Flag { get; set;} = false;
        public DateTime? DeclarationDate { get; set; }
        
    }
}
