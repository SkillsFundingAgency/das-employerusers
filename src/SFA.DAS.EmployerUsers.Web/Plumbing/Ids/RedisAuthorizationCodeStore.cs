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

            var connectionMultiplexer = ConnectionMultiplexer.Connect(CloudConfigurationManager.GetSetting("AuthCodeCacheConnectionString"));
            _cache = connectionMultiplexer.GetDatabase();
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

        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            throw new NotImplementedException();
        }

        public Task RevokeAsync(string subject, string client)
        {
            throw new NotImplementedException();
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
    }
}