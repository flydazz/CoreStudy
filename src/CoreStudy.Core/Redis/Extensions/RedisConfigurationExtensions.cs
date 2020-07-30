using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreStudy.Core.Redis.Extensions
{
    public static class RedisConfigurationExtensions
    {
        private const string CONFIGURATION_CONFIG_SECTION_KEY = "Redis:Configuration";

        public static string GetRedisConfigurationString(this IConfiguration configuration)
        {
            var configurationString = configuration.GetSection(CONFIGURATION_CONFIG_SECTION_KEY).Value;

            if (String.IsNullOrEmpty(configurationString))
            {
                throw new ArgumentNullException(CONFIGURATION_CONFIG_SECTION_KEY, "Redis 的 Configuration 未配置，请检查 appsettings.json 文件");
            }

            return configurationString;
        }
    }
}
