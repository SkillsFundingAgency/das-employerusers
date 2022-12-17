using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;
using SFA.DAS.EmployerProfiles.Data.Users;
using SFA.DAS.EmployerProfiles.Domain.RequestHandlers;
using SFA.DAS.EmployerProfiles.Domain.UserProfiles;

namespace SFA.DAS.EmployerProfiles.Api.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services)
    {
        services.AddTransient<IUserProfileRepository, UserProfileRepository>();
        services.AddTransient<IValidator<UpsertUserRequest>, UpsertUserRequestValidator>();
    }
}