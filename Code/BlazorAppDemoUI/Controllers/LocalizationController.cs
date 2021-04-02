using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
 

namespace BlazorAppDemo.Controllers
{
    [Route("[controller]/[action]")]
    public class LocalizationController : Controller
    {

        public IActionResult SetBrowserCulture(string culture, string redirectUri)
        {
            if (!string.IsNullOrWhiteSpace(culture))
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(
                        new RequestCulture(culture)));
            }

            return LocalRedirect(redirectUri);
        }
    }
}
