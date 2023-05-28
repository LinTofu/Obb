using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Obb.Models
{
    public class ObbUser
    {
        [Key]
        public string UserID { get; set; }
        public string UserNMC { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string RegistrationDateTime { get; set; }
        public string LoginDateTime { get; set; }
        public string LastLoginDateTime { get; set; }
        public string PassSalt { get; set; }
        public string ErrorMsg { get; set; }
    }
}
