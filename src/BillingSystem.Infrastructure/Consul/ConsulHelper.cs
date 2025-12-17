using BillingSystem.SharedKernel;
using Consul;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using System.Text;

namespace BillingSystem.Infrastructure.Consul
{
    public static class ConsulHelper
    {
        public static async Task<T> ReadKeyValueFromConsulAsync<T>(
            IConsulClient consulClient,
            string fullKeyPath,
            ILogger logger,
            IConfiguration configuration,
            IDatabase redisDb)
        {
            var redisValue = await redisDb.StringGetAsync(fullKeyPath);
            if (!redisValue.IsNullOrEmpty)
            {
                var deserialized = JsonConvert.DeserializeObject<T>(redisValue);
                return deserialized;
            }

            else
            {
                var getResult = await consulClient.KV.Get(fullKeyPath);
                if (getResult != null && getResult.Response != null)
                {
                    var settingValue = Encoding.UTF8.GetString(getResult.Response.Value);
                    try
                    {
                        await redisDb.StringSetAsync(fullKeyPath, settingValue);
                        var json = JsonConvert.DeserializeObject<T>(settingValue);
                        return json;
                    }
                    catch (Exception ex)
                    {
                        ex.HandleException(logger, configuration);
                        throw;
                    }
                }
                else
                    throw new Exception("Key not found in Consul server");
            }


        }
    }
}
