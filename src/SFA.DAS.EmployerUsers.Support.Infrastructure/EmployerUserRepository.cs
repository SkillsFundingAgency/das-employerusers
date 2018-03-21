using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.EmployerUsers.Support.Core.Domain.Model;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerUsers.Support.Infrastructure
{
    public sealed class EmployerUserRepository : IEmployerUserRepository
    {
        private readonly IEmployerUsersApiClient _employerUsersApiClient;
        private readonly ILog _logger;
        private int _usersPerPage = 1000;
        private readonly IAccountApiClient _employerAccountsApiClient;

        public EmployerUserRepository(ILog logger, IEmployerUsersApiClient employerUsersApiClient, IAccountApiClient employerAccountsApiClient)
        {
            _logger = logger;
            _employerUsersApiClient = employerUsersApiClient;
            _employerAccountsApiClient = employerAccountsApiClient;
        }

        public async Task<IEnumerable<EmployerUser>> FindAllDetails(int pagesize, int pageNumber)
        {
            var results = new List<UserSummaryViewModel>();

            try
            {
                var users = await _employerUsersApiClient.GetPageOfEmployerUsers(pageNumber, pagesize);
                if (users?.Data?.Count > 0)
                {
                    results.AddRange(users.Data);
                }

                return results.Select(x => MapToEmployerUser(x));
            }
            catch (HttpRequestException e)
            {
                _logger.Warn($"The Employer User API Http request threw an exception while fetching Page {pageNumber} - Exception :\r\n{e}");
                throw;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"A general exception has been thrown while requesting employer users details");
                throw;
            }

        }


        public async Task<int> TotalUserRecords(int pagesize)
        {
            try
            {
                var userFirstPageModel = await _employerUsersApiClient.GetPageOfEmployerUsers(1, pagesize);
                if (userFirstPageModel == null)
                {
                    return 0;
                }

                return (userFirstPageModel.TotalPages * pagesize);

            }
            catch (HttpRequestException e)
            {
                _logger.Warn($"The Employer User API Http request threw an exception while fetching  Total User Records- Exception :\r\n{e}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception while fetching all user record count");
                throw;
            }
        }


        public async Task<EmployerUser> Get(string id)
        {

            try
            {
                _logger.Debug($"{nameof(IEmployerUsersApiClient)}.{nameof(_employerUsersApiClient.GetResource)}<{nameof(UserViewModel)}>(\"/api/users/{id}\");");
                var response = await _employerUsersApiClient.GetResource<UserViewModel>($"/api/users/{id}");

                if (response != null)
                    return MapToEmployerUser(response);
                else
                {
                    return null as EmployerUser;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while trying to load user id {id}");
                throw;
            }

        }

        public async Task<ICollection<AccountDetailViewModel>> GetAccounts(string id)
        {
            try
            {
                _logger.Debug($"{nameof(IAccountApiClient)}.{nameof(_employerAccountsApiClient.GetUserAccounts)}(\"{id}\");");
                var response = await _employerAccountsApiClient.GetUserAccounts(id);
                return response ?? new Collection<AccountDetailViewModel>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while trying to get User Accounts for user id {id}");

                throw;
            }

        }

        private EmployerUser MapToEmployerUser(UserViewModel data)
        {
            return new EmployerUser
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                IsActive = data.IsActive,
                FailedLoginAttempts = data.FailedLoginAttempts,
                IsLocked = data.IsLocked
            };
        }

        private EmployerUser MapToEmployerUser(UserSummaryViewModel data)
        {
            return new EmployerUser
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                IsActive = data.IsActive,
                IsLocked = data.IsLocked
            };
        }
    }
}