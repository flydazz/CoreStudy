using CoreStudy.Core.Redis.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreStudy.Core.Redis
{
    public class RedisClient
    {
        private readonly ILogger<RedisClient> _logger;
        private readonly IConfiguration _configuration;

        private readonly ConnectionMultiplexer _connection;

        public RedisClient(
            ILogger<RedisClient> logger,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _configuration = configuration;

            _connection = ConnectionMultiplexer.Connect(_configuration.GetRedisConfigurationString());
        }

        public IDatabase Database => _connection.GetDatabase();
    }
}
