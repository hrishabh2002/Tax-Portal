using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace AspAssignment.Data
{
    [Index(nameof(EmployeeId), IsUnique = true)]
    [Index(nameof(Pan), IsUnique = true)]
    public class ApplicationUser : IdentityUser
    {
        public String FirstName { get; set; }
        public String LastName {  get; set; }
        
        public String EmployeeId{ get; set; }

       
        public String Pan {  get; set; }
        public String Address { get; set; }
    }
}
