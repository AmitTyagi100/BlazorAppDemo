using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
