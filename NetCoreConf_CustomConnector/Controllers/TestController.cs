using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreConf_CustomConnector.Model.Utils;
using NetCoreConf_CustomConnector.Utils;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NetCoreConf_CustomConnector.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/")]

    public class TestController : Controller
    {
        #region Invoke Webhook

        [HttpPost]
        public async Task<IActionResult> InvokeWebhookA(Model.Request.DemoARequest request)
        {
            try
            {
                return await InvokeWebhook("DemoA", request);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InvokeWebhookB()
        {
            try
            {
                return await InvokeWebhook("DemoB");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Private Methods

        private async Task<IActionResult> InvokeWebhook(string webhookIdentifier, object requestbody = null)
        {
            StoreWebHookDetails hook = _webhookStoreService.GetStore().Webhooks.FirstOrDefault(x => x.Id == webhookIdentifier);

            if (hook == null)
                return NotFound("Webhook not found");

            using (var client = new HttpClient())
            {
                var contenttype = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json");
                client.BaseAddress = new Uri(hook.Url);
                client.DefaultRequestHeaders.Accept.Add(contenttype);

                StringContent content = null;

                if (requestbody != null)
                {
                    var data = System.Text.Json.JsonSerializer.Serialize(requestbody);
                    content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                }

                var response = await client.PostAsync(hook.Url, content);

                if (!response.IsSuccessStatusCode)
                    return BadRequest("Error on webhook call");
            }

            return Ok();

        }

        #endregion


        private readonly ILogger<TestController> _logger;
        private readonly IWebhookStoreService _webhookStoreService;

        public TestController(ILogger<TestController> logger, IWebhookStoreService webhookStoreService )
        {
            _logger = logger;
            _webhookStoreService = webhookStoreService;
        }
    }
}
