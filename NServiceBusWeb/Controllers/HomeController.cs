using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UserService.Messages.Commands;

namespace NServiceBusWeb.Controllers
{
    public class HomeController : Controller
    {
        readonly IEndpointInstance endpointInstance;

        public HomeController(IEndpointInstance endpointInstance)
        {
            this.endpointInstance = endpointInstance;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> CreateUser(string name, string email)
        {
            var cmd = new CreateNewUserCmd
            {
                Name = name,
                EmailAddress = email
            };

            var sendOptions = new SendOptions();
            sendOptions.SetDestination("UserService");

            //sendOptions.DelayDeliveryWith(TimeSpan.FromSeconds(10));
            await endpointInstance.Send(cmd, sendOptions).ConfigureAwait(false);
            return Json(new { sent = cmd });
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding,
            JsonRequestBehavior.AllowGet);
        }
    }
}