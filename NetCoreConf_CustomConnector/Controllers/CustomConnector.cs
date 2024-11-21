using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetCoreConf_CustomConnector.Model.Response;
using NetCoreConf_CustomConnector.Model.Utils;
using NetCoreConf_CustomConnector.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetCoreConf_CustomConnector.Controllers
{
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    [ApiController]
    [Route("api/[controller]/[action]/")]

    public class CustomConnector : Controller
    {
        #region Notify Webhook Endpoints

        [HttpPost]
        public IActionResult WebhookDemoADetails(Model.Request.WebhookDetailRequest details)
        {

            string url = $"{_ngrokUrl}/api/CustomConnector/DeleteDemoADetails?id=1";

            string webhookIdentifier = "DemoA";

            StoreWebhook(webhookIdentifier, details.ActionUrl);

            // Add custom headers
            Response.Headers.Add("Location", url);

            // Return 201 Created status with headers
            return StatusCode(201);

        }

        [HttpPost]
        public IActionResult WebhookDemoBDetails(Model.Request.WebhookDetailRequest details)
        {

            string url = $"{_ngrokUrl}/api/CustomConnector/DeleteDemoBDetails?id=1";

            string webhookIdentifier = "DemoB";

            StoreWebhook(webhookIdentifier, details.ActionUrl);

            // Add custom headers
            Response.Headers.Add("Location", url);

            // Return 201 Created status with headers
            return StatusCode(201);

        }


        #endregion

        #region Delete Webhook

        [HttpDelete]
        public IActionResult DeleteDemoADetails(int? id)
        {
            DeleteWebhook("DemoA");

            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteDemoBDetails(int? id)
        {
            DeleteWebhook("DemoB");
            return Ok();
        }

        #endregion

        #region Action Endpoint

        [HttpPost]
        public async Task<IActionResult> ReceiveDemoAInfo(DemoAItems[] details)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveDemoBInfo(DemoBItems[] details)
        {
            return Ok();
        }

        #endregion

        #region Polling Endopoint

        [HttpGet]
        public async Task<JsonResult> PollingDemoAInfo(int requestId )
        {
            var newItems = new List<PollingAItem>
            {
                new PollingAItem
                {
                    Id = ++requestId,
                    Name = "Demo A",
                    Description = "Demo A Description"
                }
            };

            newItems.Reverse();

            return new JsonResult( new { items = newItems } );
        }

        #endregion

        #region Private methods

        private void StoreWebhook(string webhookIdentifier, string url)
        {
            var store = _webhookStoreService.GetStore();

            if (!store.Webhooks.Any(x => x.Id == webhookIdentifier))
            {
                store.Webhooks.Add(new StoreWebHookDetails
                {
                    Id = webhookIdentifier,
                    Url = url
                });

            }
            else
            {
                store.Webhooks.FirstOrDefault(x => x.Id == webhookIdentifier).Url = url;
            }

            _webhookStoreService.Save(store);
        }

        private void DeleteWebhook(string webhookIdentifier)
        {
            var store = _webhookStoreService.GetStore();

            var webhookToDelete = store.Webhooks.FirstOrDefault(x => x.Id == webhookIdentifier);

            if (webhookToDelete != null) 
            { 
                store.Webhooks.Remove(webhookToDelete);
            }

            _webhookStoreService.Save(store);
        }

        #endregion


        private readonly ILogger<CustomConnector> _logger;

        private readonly IWebhookStoreService _webhookStoreService; 

        private string _ngrokUrl = "https://insect-in-allegedly.ngrok-free.app";

        public CustomConnector(ILogger<CustomConnector> logger, IWebhookStoreService webhookStoreService)
        {
            _logger = logger;
            _webhookStoreService = webhookStoreService;
        }        
    }
}
