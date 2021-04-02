using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppDemo.Data
{
    public class RefreshTokenRequest
    {

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
