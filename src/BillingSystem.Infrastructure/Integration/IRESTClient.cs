namespace BillingSystem.Infrastructure.Integration
{
    public interface IRESTClient
    {
        Task<Tresponse> PostAsync<Trequest, Tresponse>(Trequest trequest, string baseAddress, string suffixAddress, Dictionary<string, string>? headers = null, bool requireAuth = false, string authToken = "") where Trequest : class, new();
        Task<Tresponse> GetAsync<Tresponse>(Dictionary<string, string> parameters, string baseAddress, string suffixAddress, Dictionary<string, string>? headers = null, bool requireAuth = false, string authToken = "");
    }
}
