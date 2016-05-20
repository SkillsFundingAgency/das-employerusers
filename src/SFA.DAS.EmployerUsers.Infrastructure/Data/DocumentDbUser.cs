using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    internal class DocumentDbUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        internal static DocumentDbUser FromDomainUser(User user)
        {
            return new DocumentDbUser
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                IsActive = user.IsActive,
                LastName = user.LastName,
                Password = user.Password
            };
        }

        internal User ToDomainUser()
        {
            return new User
            {

                Id = Id,
                Email = Email,
                FirstName = FirstName,
                IsActive = IsActive,
                LastName = LastName,
                Password = Password
            };
        }
    }
}
