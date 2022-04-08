using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebSite.Repositories.LineLogin;
using WebSite.Setting;

namespace WebSite.Controllers
{
    public class LogoutController : Controller
    {
        private readonly LineLoginSetting _lineLoginSetting;
        private readonly ILineLoginApi _lineLoginApi;

        public LogoutController(LineLoginSetting lineLoginSetting, ILineLoginApi lineLoginApi)
        {
            _lineLoginSetting = lineLoginSetting;
            _lineLoginApi = lineLoginApi;
        }

        public async Task<IActionResult> Line()
        {
            var values = new Dictionary<string, string>
            {
                { "access_token", HttpContext.User.FindFirstValue("AccessToken") },
                { "client_id", _lineLoginSetting.ClientId },
                { "client_secret", _lineLoginSetting.ClientSecret }
            };

            var content = new FormUrlEncodedContent(values);
            var url = "https://api.line.me/oauth2/v2.1/revoke";
            var response = await url
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .PostAsync(content);
            if (HttpContext.User.Identity is { IsAuthenticated: true } && response.StatusCode == StatusCodes.Status200OK)
            {
                await HttpContext.SignOutAsync();
            }

            return Redirect("/");
        }
    }
}