using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebSite.Repositories.LineLogin;
using WebSite.Setting;

namespace WebSite.Controllers
{
    public class LoginController : Controller
    {
        private readonly LineLoginSetting _lineLoginSetting;
        private readonly ILineLoginApi _lineLoginApi;

        public LoginController(LineLoginSetting lineLoginSetting, ILineLoginApi lineLoginApi)
        {
            _lineLoginSetting = lineLoginSetting;
            _lineLoginApi = lineLoginApi;
        }

        public ActionResult Index(string redirectUrl)
        {
            ViewBag.RedirectUrl = redirectUrl;
            return View();
        }

        public IActionResult Line(string redirectUrl)
        {
            TempData["RedirectUrl"] = redirectUrl;
            var url = @"https://access.line.me/oauth2/v2.1/authorize"
                .SetQueryParams(new
                {
                    response_type = "code",
                    client_id = _lineLoginSetting.ClientId,
                    redirect_uri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Login/LineCallback",
                    state = new Random().Next(1, 999999),
                    scope = "profile"
                });
            return Redirect(url);
        }

        public async Task<IActionResult> LineCallback(string code, string state, string? error)
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Login/LineCallback" },
                { "client_id", _lineLoginSetting.ClientId },
                { "client_secret", _lineLoginSetting.ClientSecret }
            };

            var content = new FormUrlEncodedContent(values);
            const string tokenUrl = "https://api.line.me/oauth2/v2.1/token";
            var response = await tokenUrl
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .PostAsync(content);
            var lineOAuth2TokenResult = await response.GetJsonAsync<LineOAuth2TokenResult>();

            var lineProfileResult = await _lineLoginApi.GetProfileAsync(lineOAuth2TokenResult.AccessToken);

            var redirectUrl = TempData["RedirectUrl"]?.ToString() ?? "/";
            return Redirect(redirectUrl);
        }
    }
}