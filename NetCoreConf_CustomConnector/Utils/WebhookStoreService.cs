using System.Collections.Generic;
using System;
using NetCoreConf_CustomConnector.Model.Utils;

namespace NetCoreConf_CustomConnector.Utils
{
    public class WebhookStoreService : IWebhookStoreService
    {
        public WebhookStore GetStore()
        {
            string mainPath = $"{AppDomain.CurrentDomain.BaseDirectory}store.json";

            return System.Text.Json.JsonSerializer.Deserialize<WebhookStore>(System.IO.File.ReadAllText(mainPath));
        }

        public void Save(WebhookStore actualStore)
        {
            string mainPath = $"{AppDomain.CurrentDomain.BaseDirectory}store.json";

            System.IO.File.WriteAllText(mainPath, System.Text.Json.JsonSerializer.Serialize(actualStore));
        }
    }
}
