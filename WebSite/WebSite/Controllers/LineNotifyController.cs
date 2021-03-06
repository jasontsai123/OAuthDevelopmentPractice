using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using WebSite.Models;
using WebSite.Repositories.LineLogin;
using WebSite.Repositories.LineNotify;
using WebSite.Repositories.LineNotifySubscriber;
using WebSite.Setting;

namespace WebSite.Controllers
{
    /// <summary>
    /// The line notify controller class
    /// </summary>
    /// <seealso cref="Controller"/>
    public class LineNotifyController : Controller
    {
        /// <summary>
        /// LineNotify設定
        /// </summary>
        private readonly LineNotifySetting _lineNotifySetting;

        /// <summary>
        /// The line notify api
        /// </summary>
        private readonly ILineNotifyApi _lineNotifyApi;

        /// <summary>
        /// The line notify subscriber repository
        /// </summary>
        private readonly ILineNotifySubscriberRepository _lineNotifySubscriberRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineNotifyController"/> class
        /// </summary>
        public LineNotifyController(
            LineNotifySetting lineNotifySetting,
            ILineNotifyApi lineNotifyApi,
            ILineNotifySubscriberRepository lineNotifySubscriberRepository)
        {
            _lineNotifySetting = lineNotifySetting;
            _lineNotifyApi = lineNotifyApi;
            _lineNotifySubscriberRepository = lineNotifySubscriberRepository;
        }

        private const string NotifyAccessTokenSessionKey = "NotifyAccessToken";

        /// <summary>
        /// Indexes this instance
        /// </summary>
        /// <returns>The action result</returns>
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity is { IsAuthenticated: false })
            {
                return Redirect($"/Login?redirectUrl={HttpContext.Request.GetDisplayUrl()}");
            }

            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = false
            };
            if (HttpContext.User.Identity is { IsAuthenticated: true })
            {
                var list = await _lineNotifySubscriberRepository.GetAllAsync();
                vm.HasSubscribed = HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) == list?.FirstOrDefault()?.LineUserId;
            }

            return View(vm);
        }

        /// <summary>
        /// Subscribes this instance
        /// </summary>
        /// <returns>The action result</returns>
        [HttpGet]
        public async Task<IActionResult> Subscribe()
        {
            if (UserHasHasSubscribedNotify())
            {
                return RedirectToAction("Index");
            }

            var url = "https://notify-bot.line.me/oauth/authorize".SetQueryParams(new
            {
                response_type = "code",
                client_id = _lineNotifySetting.ClientId,
                redirect_uri = HttpContext.Request.GetDisplayUrl(),
                scope = "notify",
                state = new Random().Next(1, 999999),
                response_mode = "form_post"
            });
            return Redirect(url);
        }

        /// <summary>
        /// Subscribes the line notify authorize
        /// </summary>
        /// <param name="lineNotifyAuthorize">The line notify authorize</param>
        /// <returns>A task containing the action result</returns>
        [HttpPost]
        public async Task<IActionResult> Subscribe(LineNotifyAuthorize lineNotifyAuthorize)
        {
            if (UserHasHasSubscribedNotify())
            {
                return RedirectToAction("Index");
            }

            var oauth = await _lineNotifyApi.GetOauthTokenAsync(new OauthTokenParameter
            {
                Code = lineNotifyAuthorize.Code,
                RedirectUri = HttpContext.Request.GetDisplayUrl()
            });
            HttpContext.Session.SetString(NotifyAccessTokenSessionKey, oauth.AccessToken);
            var affectRows = await _lineNotifySubscriberRepository.InsertAsync(new LineNotifySubscriber()
                { LineUserId = HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), AccessToken = oauth.AccessToken });
            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = affectRows > 0
            };
            return View("Index", vm);
        }

        /// <summary>
        /// Unsubscribes this instance
        /// </summary>
        /// <returns>The action result</returns>
        public async Task<IActionResult> Unsubscribe()
        {
            var lineProfileUserId = HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var lineNotifySubscribers = (await _lineNotifySubscriberRepository.GetAllAsync()).Where(x => x.LineUserId == lineProfileUserId);
            var accessToken = lineNotifySubscribers.FirstOrDefault()?.AccessToken;
            if (accessToken is null)
            {
                return RedirectToAction("Index");
            }

            var deleteTokenAffectRows = await _lineNotifySubscriberRepository.DeleteByAccessTokenAsync(accessToken);
            if (UserHasHasSubscribedNotify() && deleteTokenAffectRows > 0)
            {
                HttpContext?.Session.Remove(NotifyAccessTokenSessionKey);
            }

            var revokeResult = await _lineNotifyApi.RevokeAsync(accessToken);
            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = revokeResult.Status != StatusCodes.Status200OK
            };
            return View("Index", vm);
        }

        /// <summary>
        /// 使用者已訂閱 LINE Notify
        /// </summary>
        /// <returns></returns>
        private bool UserHasHasSubscribedNotify()
        {
            return HttpContext.Session.TryGetValue(NotifyAccessTokenSessionKey, out _);
        }
    }
}