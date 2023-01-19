using System.Net.Mail;
using SFA.DAS.EmployerProfiles.Domain.RequestHandlers;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

public class UpsertUserRequestValidator : IValidator<UpsertUserRequest>
{
    public Task<ValidationResult> ValidateAsync(UpsertUserRequest item)
    {
        var result = new ValidationResult();
        if (string.IsNullOrEmpty(item.Email))
        {
            result.AddError(nameof(item.Email));
        }
        else
        {
            try
            {
                var emailAddress = new MailAddress(item.Email);
                if (!emailAddress.Address.Equals(item.Email, StringComparison.CurrentCultureIgnoreCase))
                {
                    result.AddError(nameof(item.Email));
                }
            }
            catch (FormatException)
            {
                result.AddError(nameof(item.Email));
            }
        }

        if (item.Id == Guid.Empty)
        {
            result.AddError(nameof(item.Email));
        }
        return Task.FromResult(result);
    }
}