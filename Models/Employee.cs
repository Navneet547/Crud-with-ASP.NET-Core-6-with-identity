using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASPIdentityApp.Models
{
    public class Employee
    {
        [Key]
        public int EmpId { get; set; }
        [MaxLength(20)]
        [Required(ErrorMessage ="Please Enter Name")]
        [RegularExpression("^([a-zA-Z '])+$", ErrorMessage = "Enter a valid Name")]
        [DisplayName(" Name")]
        public String EmpName { get; set; }

        [Required]
        [DisplayName("Age")]
        [Range(18, int.MaxValue, ErrorMessage = "Age should be 18 or greater")]
        public int EmpAge { get; set; }

        [Required]
        [DisplayName("Gender")]
        [MaxLength(10)]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Please enter 'Male' or 'Female'")]
        public String EmpGender { get; set; }

        [Required]
        [DisplayName("Address")]
        [MinLength(3, ErrorMessage = "Address should be at least 3 characters")]
        public String EmpAddress { get; set; }

       
    }
}