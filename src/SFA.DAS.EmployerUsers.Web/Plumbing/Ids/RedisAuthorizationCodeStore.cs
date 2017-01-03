using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.Azure;
using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Web.Plumbing.Serialization;
using StackExchange.Redis;

namespace SFA.DAS.EmployerUsers.Web.Plumbing.Ids
{
    public class RedisAuthorizationCodeStore : IAuthorizationCodeStore
    {
        private readonly IClientStore _clientStore;
        private readonly IScopeStore _scopeStore;

        private readonly IDatabase _cache;

        public RedisAuthorizationCodeStore(IClientStore clientStore, IScopeStore scopeStore)
        {
            _clientStore = clientStore;
            _scopeStore = scopeStore;

            _cache = GetDatabase();
        }

        public async Task StoreAsync(string key, AuthorizationCode value)
        {
            await _cache.StringSetAsync(key, SerializeCode(value));
        }

        public async Task<AuthorizationCode> GetAsync(string key)
        {
            return DeserializeCode(await _cache.StringGetAsync(key));
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var matches = new List<ITokenMetadata>();

            var server = GetServer();
            var keys = server.Keys();
            foreach (var key in keys)
            {
                var code = await GetAsync(key);
                if (code.SubjectId == subject)
                {
                    matches.Add(code);
                }
            }

            return matches;
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var toDelete = new List<string>();

            var server = GetServer();
            var keys = server.Keys();
            foreach (var key in keys)
            {
                var code = await GetAsync(key);
                if (code.SubjectId == subject && code.ClientId == client)
                {
                    toDelete.Add(key);
                }
            }

            foreach (var key in toDelete)
            {
                await RemoveAsync(key);
            }
        }


        private string SerializeCode(AuthorizationCode code)
        {
            return JsonConvert.SerializeObject(code, GetJsonSerializerSettings());
        }
        private AuthorizationCode DeserializeCode(string json)
        {
            return JsonConvert.DeserializeObject<AuthorizationCode>(json, GetJsonSerializerSettings());
        }
        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ClaimConverter());
            settings.Converters.Add(new ClaimsPrincipalConverter());
            settings.Converters.Add(new ClientConverter(_clientStore));
            settings.Converters.Add(new ScopeConverter(_scopeStore));
            return settings;
        }

        private IDatabase GetDatabase()
        {
            var connectionMultiplexer = GetConnectionMultiplexer();
            return connectionMultiplexer.GetDatabase();
        }
        private IServer GetServer()
        {
            var connectionMultiplexer = GetConnectionMultiplexer();
            var connectionString = GetConnectionString();
            var serverAndPort = connectionString.Substring(0, connectionString.IndexOf(','));
            return connectionMultiplexer.GetServer(serverAndPort);
        }
        private static ConnectionMultiplexer GetConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(GetConnectionString());
        }
        private static string GetConnectionString()
        {
            return CloudConfigurationManager.GetSetting("AuthCodeCacheConnectionString");
        }
    }
}