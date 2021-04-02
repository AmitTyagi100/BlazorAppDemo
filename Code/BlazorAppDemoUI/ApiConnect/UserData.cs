using BlazorAppDemo.Data;
using CrossCuttingEntities;
using CrossCuttingUtility;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAppDemo.ApiConnect
{
    public class UserData : IUserData
    {

        public HttpClient _httpClient { get; }
        public AppSettings _appSettings { get; }

        public UserData(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            httpClient.BaseAddress = new Uri(_appSettings.BlazorDemoAPIAddress);
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");

            _httpClient = httpClient;
        }
        public async Task<User> GetUserByAccessTokenAsync(string accessToken)
        {
            string serializedRefreshRequest = JsonConvert.SerializeObject(accessToken);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/GetUserByAccessToken");
            requestMessage.Content = new StringContent(serializedRefreshRequest);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

            return await Task.FromResult(returnedUser);
        }

        public async Task<User> LoginAsync(User user)
        {
            user.Password = Utility.Encrypt(user.Password,user.EmailAddress ,_appSettings.SaltString);

            user.User_Info = new UserInfo() { FirstName="-", LastName="-",DeptId=0,EmailAddress=user.EmailAddress };

            string serializedUser =  JsonConvert.SerializeObject(user);



            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/Login");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

            return await Task.FromResult(returnedUser);
        }

        public Task<User> RefreshTokenAsync(RefreshTokenRequest refreshRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            user.Password = Utility.Encrypt(user.Password, user.EmailAddress, _appSettings.SaltString);
            string serializedUser = JsonConvert.SerializeObject(user);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/RegisterUser");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

            return await Task.FromResult(returnedUser);
        }
    }
}
