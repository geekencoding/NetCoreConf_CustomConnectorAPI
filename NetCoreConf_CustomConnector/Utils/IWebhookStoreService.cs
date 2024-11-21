using NetCoreConf_CustomConnector.Model.Utils;

namespace NetCoreConf_CustomConnector.Utils
{
    public interface IWebhookStoreService
    {
        WebhookStore GetStore();
        void Save(WebhookStore actualStore);
    }
}
