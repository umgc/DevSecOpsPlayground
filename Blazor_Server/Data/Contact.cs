using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blazor_Server.Data
{
    public class Contact
    {
        [Required]
        [StringLength(255, ErrorMessage = "First Name is too long")]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(255, ErrorMessage = "Last Name is too long")]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;


        [DisplayName("Your Website")]
        public string Url { get; set; } = string.Empty;
    }
}
