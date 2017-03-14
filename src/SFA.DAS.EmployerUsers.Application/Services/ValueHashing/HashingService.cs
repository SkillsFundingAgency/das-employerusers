using System;
using System.Linq;
using HashidsNet;
using NLog;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Services.ValueHashing
{
    public class HashingService : IHashingService
    {
        private readonly ILogger _logger;

        private readonly Hashids _hashIds;
        private const string Hashstring = "SFA: digital apprenticeship service";
        private const string AllowedCharacters = "46789BCDFGHJKLMNPRSTVWXY";

        public HashingService(EmployerUsersConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            var hashstring = string.IsNullOrEmpty(configuration.Hashstring)
                    ? Hashstring
                    : configuration.Hashstring;

            _hashIds = new Hashids(hashstring, 6, AllowedCharacters);
        }

        public string HashValue(Guid id)
        {
            return _hashIds.Encode(id.ToByteArray().Select(Convert.ToInt32).ToArray());
        }

        public Guid DecodeValue(string id)
        {
            try
            {
                return new Guid(_hashIds.Decode(id).Select(Convert.ToByte).ToArray());
            }
            catch (ArgumentException ex)
            {
                _logger.Warn(ex);
            }
            return Guid.Empty;
        }
    }
}
