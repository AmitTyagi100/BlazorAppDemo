using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingEntities
{
    [Table("tblUser", Schema = "dbo")]
    public partial class User
    {

        [Key]
        public int UserId { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        public string EmailAddress { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string Password { get; set; }

        [Display(Name = "IsActive")]
        public bool? IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
    }

    public partial class User
    {

        public User()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        [Display(Name = "Confirm Password")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        [NotMapped]
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        // used in UI

        [NotMapped]
        public string AccessToken { get; set; }

        [NotMapped]
        public string RefreshToken { get; set; }


        [NotMapped]
        public UserInfo User_Info { get; set; } = new UserInfo();
	}
}
