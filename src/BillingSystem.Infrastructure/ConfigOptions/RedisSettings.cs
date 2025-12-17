using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Infrastructure.ConfigOptions
{
    public class RedisSettings
    {
        public const string SectionName = "redis";
        public string server { get; set; }
        public string password { get; set; }
    }
}
