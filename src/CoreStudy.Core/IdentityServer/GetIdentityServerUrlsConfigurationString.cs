using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreStudy.Core.IdentityServer
{
    public static class IdentityServerUrlsConfigurationString
    {
        private const string CONFIGURATION_CONFIG_SECTION_KEY = "IdentityServer:Url";

        public static string GetIdentityServerUrlsConfigurationString(this IConfiguration configuration)
        {
            var configurationString = configuration.GetSection(CONFIGURATION_CONFIG_SECTION_KEY).Value;

            if (String.IsNullOrEmpty(configurationString))
            {
                throw new ArgumentNullException(CONFIGURATION_CONFIG_SECTION_KEY, "IdentityServer 的 Configuration 未配置，请检查 appsettings.json 文件");
            }

            return configurationString;
        }

    }
}
