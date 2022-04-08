using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using WebSite.Repositories.LineLogin;
using WebSite.Setting;

namespace WebSite.Controllers
{
    public class LoginController : Controller
    {
        private readonly LineLoginSetting _lineLoginSetting;
        private readonly ILineLoginApi _lineLoginApi;
        private const string AuthenticationScheme = ".OauthDevelopmentPractice.Auth";

        public LoginController(
            LineLoginSetting lineLoginSetting,
            ILineLoginApi lineLoginApi)
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
            if (response.StatusCode == StatusCodes.Status200OK)
            {
                var lineOAuth2TokenResult = await response.GetJsonAsync<LineOAuth2TokenResult>();
                var lineProfileResult = await _lineLoginApi.GetProfileAsync(lineOAuth2TokenResult.AccessToken);
                var claims = new List<Claim>()
                {
                    new("AccessToken", lineOAuth2TokenResult.AccessToken),
                    new(ClaimTypes.NameIdentifier, lineProfileResult.UserId),
                    new(ClaimTypes.Name, lineProfileResult.DisplayName),
                    new("PictureUrl", lineProfileResult.PictureUrl.OriginalString),
                    new("StatusMessage", lineProfileResult.StatusMessage ?? string.Empty)
                };
                var claimsIdentity = new ClaimsIdentity(claims, AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
            }


            var redirectUrl = TempData["RedirectUrl"]?.ToString() ?? "/";
            return Redirect(redirectUrl);
        }
    }
}