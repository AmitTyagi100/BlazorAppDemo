using BlazorAppDemo.Data;
using CrossCuttingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppDemo.ApiConnect
{
  public  interface IUserData
    {


        public Task<User> LoginAsync(User user);
        public Task<User> RegisterUserAsync(User user);
        public Task<User> GetUserByAccessTokenAsync(string accessToken);
        public Task<User> RefreshTokenAsync(RefreshTokenRequest refreshRequest);
    }
}
