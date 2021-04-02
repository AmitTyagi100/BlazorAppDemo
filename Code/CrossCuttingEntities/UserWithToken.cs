using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingEntities
{
   public class UserWithToken : User
    {

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

     //   public UserInfo UserInfo { get; set; } = new UserInfo();

        public UserWithToken(User user, UserInfo userInfo)
        {
            this.UserId = user.UserId;
            this.EmailAddress = user.EmailAddress;
     
            this.IsActive  = user.IsActive;
            this.CreatedDate = user.CreatedDate;
            this.ModifiedDate = user.ModifiedDate;
            this.User_Info = userInfo;
           // this.Role = user.Role;
        }

      
    }
}
