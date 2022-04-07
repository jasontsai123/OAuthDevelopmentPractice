using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using WebSite.Models;
using WebSite.Repositories.LineNotify;
using WebSite.Repositories.LineNotifySubscriber;

namespace WebSite.Controllers
{
    /// <summary>
    /// The line notify controller class
    /// </summary>
    /// <seealso cref="Controller"/>
    public class LineNotifyController : Controller
    {
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
        /// <param name="lineNotifyApi">The line notify api</param>
        /// <param name="lineNotifySubscriberRepository">The line notify subscriber repository</param>
        public LineNotifyController(ILineNotifyApi lineNotifyApi,
            ILineNotifySubscriberRepository lineNotifySubscriberRepository)
        {
            _lineNotifyApi = lineNotifyApi;
            _lineNotifySubscriberRepository = lineNotifySubscriberRepository;
        }

        /// <summary>
        /// Indexes this instance
        /// </summary>
        /// <returns>The action result</returns>
        public ActionResult Index()
        {
            var vm = new LineNotifySubscriberViewModel();
            return View(vm);
        }

        /// <summary>
        /// Subscribes this instance
        /// </summary>
        /// <returns>The action result</returns>
        [HttpGet]
        public ActionResult Subscribe()
        {
            var url = "https://notify-bot.line.me/oauth/authorize".SetQueryParams(new
            {
                response_type = "code",
                client_id = "7u1Lu4cdIldcwOx9ueDBaJ",
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
            var affectRows = await _lineNotifySubscriberRepository.InsertAsync(new LineNotifySubscriber() { AccessToken = oauth.AccessToken });
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
        public ActionResult Unsubscribe()
        {
            var vm = new LineNotifySubscriberViewModel
            {
                HasSubscribed = false
            };
            return View("Index", vm);
        }
    }
}