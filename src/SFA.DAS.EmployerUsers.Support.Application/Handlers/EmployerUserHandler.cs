using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;
using SFA.DAS.EmployerUsers.Support.Infrastructure;
using SFA.DAS.Support.Shared.SearchIndexModel;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerUsers.Support.Application.Handlers
{
    public class EmployerUserHandler : IEmployerUserHandler
    {
        private readonly IEmployerUserRepository _userRepository;
        private readonly ILog _log;

        public EmployerUserHandler(IEmployerUserRepository userRepository, ILog log)
        {
            _userRepository = userRepository;
            _log = log;
        }

        public async Task<IEnumerable<UserSearchModel>> FindSearchItems(int pagesize, int pageNumber)
        {
            try
            {

                var models = await _userRepository.FindAllDetails( pagesize,  pageNumber);
                return models?.Select(m => Map(m)).ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error while trying to load users");

                throw;
            }

        }

        public async Task<int> TotalUserRecords(int pagesize)
        {
            return await _userRepository.TotalUserRecords(pagesize);
        }

        public UserSearchModel Map(EmployerUser employerUser)
        {
            return new UserSearchModel
            {
                Id = employerUser.Id,
                Email = string.IsNullOrEmpty(employerUser.Email) ? "NA" : employerUser.Email,
                FirstName = employerUser.FirstName,
                LastName = employerUser.LastName,
                Status = Enum.GetName(typeof(UserStatus), employerUser.Status),
                SearchType = SearchCategory.User
            };
        }

    }
}