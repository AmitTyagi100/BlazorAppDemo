using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using BlazorAppDemo.Data;
 
using System.Net.Http;
 
using Blazored.LocalStorage;

namespace BlazorAppDemo.ApiConnect
{
	public class APIServices<T> :IAPIServices<T>
	{


		public HttpClient _httpClient { get; }
		public AppSettings _appSettings { get; }
		public ILocalStorageService _localStorageService { get; }

		public APIServices(HttpClient httpClient
			, IOptions<AppSettings> appSettings
			, ILocalStorageService localStorageService)
		{
			_appSettings = appSettings.Value;
			_localStorageService = localStorageService;

			httpClient.BaseAddress = new Uri(_appSettings.BlazorDemoAPIAddress);
		   // httpClient.DefaultRequestHeaders.Add("User-Agent", "BlazorServer");

			_httpClient = httpClient;
		}

		public async Task<bool> DeleteAsync(string requestUri, int Id)
		{
			requestUri = requestUri.EndsWith("/") ? requestUri + Id : requestUri + "/" + Id;
			var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri  );

			var token = await _localStorageService.GetItemAsync<string>("accessToken");
			requestMessage.Headers.Authorization
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var response = await _httpClient.SendAsync(requestMessage);

			var responseStatusCode = response.StatusCode;

			return await Task.FromResult(true);
		}

		public async Task<List<T>> GetAllAsync(string requestUri)
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

			var token = await _localStorageService.GetItemAsync<string>("accessToken");
			requestMessage.Headers.Authorization
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var response = await _httpClient.SendAsync(requestMessage);

			var responseStatusCode = response.StatusCode;

			if (responseStatusCode.ToString() == "OK")
			{
				var responseBody = await response.Content.ReadAsStringAsync();
				return await Task.FromResult(JsonConvert.DeserializeObject<List<T>>(responseBody));
			}
			else
				return null;
		}

		public async Task<T> GetByIdAsync(string requestUri, int Id)
		{
			requestUri = requestUri.EndsWith("/") ? requestUri + Id : requestUri + "/" + Id;
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri  );

			var token = await _localStorageService.GetItemAsync<string>("accessToken");
			requestMessage.Headers.Authorization
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var response = await _httpClient.SendAsync(requestMessage);

			var responseStatusCode = response.StatusCode;
			var responseBody = await response.Content.ReadAsStringAsync();

			return await Task.FromResult(JsonConvert.DeserializeObject<T>(responseBody));
		}

		public async Task<T> SaveAsync(string requestUri, T obj)
		{
			string serializedUser = JsonConvert.SerializeObject(obj);

			var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

			var token = await _localStorageService.GetItemAsync<string>("accessToken");
			requestMessage.Headers.Authorization
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			requestMessage.Content = new StringContent(serializedUser);

			requestMessage.Content.Headers.ContentType
				= new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

			var response = await _httpClient.SendAsync(requestMessage);

			var responseStatusCode = response.StatusCode;
			var responseBody = await response.Content.ReadAsStringAsync();

			var returnedObj = JsonConvert.DeserializeObject<T>(responseBody);

			return await Task.FromResult(returnedObj);
		}

		public async Task<T> UpdateAsync(string requestUri, int Id, T obj)
		{
			string serializedUser = JsonConvert.SerializeObject(obj);

			requestUri = requestUri.EndsWith("/") ? requestUri + Id : requestUri + "/" + Id;

			var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri   );
			var token = await _localStorageService.GetItemAsync<string>("accessToken");
			requestMessage.Headers.Authorization
				= new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			requestMessage.Content = new StringContent(serializedUser);

			requestMessage.Content.Headers.ContentType
				= new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

			var response = await _httpClient.SendAsync(requestMessage);

			var responseStatusCode = response.StatusCode;
			var responseBody = await response.Content.ReadAsStringAsync();

			var returnedObj = JsonConvert.DeserializeObject<T>(responseBody);

			return await Task.FromResult(returnedObj);
		}

	}
}
