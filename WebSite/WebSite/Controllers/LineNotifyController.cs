using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        /// <summary>
        /// The line profile session id
        /// </summary>
        private const string LineProfileSessionId = "LineProfile";

        /// <summary>
        /// Indexes this instance
        /// </summary>
        /// <returns>The action result</returns>
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.TryGetValue(LineProfileSessionId, out _) == false)
            {
                return Redirect($"/Login?redirectUrl={HttpContext.Request.GetDisplayUrl()}");
            }

            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = false
            };
            if (HttpContext.Session.TryGetValue(LineProfileSessionId, out _))
            {
                var json = HttpContext.Session.GetString(LineProfileSessionId);
                var lineProfileResult = json.FromJson<LineProfileResult>();
                var list = await _lineNotifySubscriberRepository.GetAllAsync();
                vm.HasSubscribed = lineProfileResult.UserId == list?.FirstOrDefault()?.LineUserId;
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
        public async Task<ActionResult> Subscribe(LineNotifyAuthorize lineNotifyAuthorize)
        {
            var oauth = await _lineNotifyApi.GetOauthTokenAsync(new OauthTokenParameter
            {
                Code = lineNotifyAuthorize.Code,
                RedirectUri = HttpContext.Request.GetDisplayUrl()
            });
            var json = HttpContext.Session.GetString(LineProfileSessionId);
            var lineProfileResult = json.FromJson<LineProfileResult>();
            var affectRows = await _lineNotifySubscriberRepository.InsertAsync(new LineNotifySubscriber() { LineUserId = lineProfileResult.UserId, AccessToken = oauth.AccessToken });
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
            var lineProfileResult = HttpContext.Session.GetString(LineProfileSessionId).FromJson<LineProfileResult>();
            var lineNotifySubscribers = (await _lineNotifySubscriberRepository.GetAllAsync()).Where(x => x.LineUserId == lineProfileResult.UserId);
            var accessToken = lineNotifySubscribers.FirstOrDefault().AccessToken;
            await _lineNotifySubscriberRepository.DeleteByAccessTokenAsync(accessToken);
            var revokeResult = await _lineNotifyApi.RevokeAsync(accessToken);
            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = revokeResult.Status != StatusCodes.Status200OK
            };
            return View("Index", vm);
        }
    }
}