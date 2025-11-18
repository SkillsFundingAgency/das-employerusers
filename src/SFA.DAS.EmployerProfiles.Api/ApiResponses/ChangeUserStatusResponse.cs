namespace SFA.DAS.EmployerProfiles.Api.ApiResponses;

public class ChangeUserStatusResponse(string id)
{
    public string Id { get; } = id;
    public IDictionary<string, string> Errors { get; } = new Dictionary<string, string>();

    public static ChangeUserStatusResponse Success(Guid id) => new(id.ToString());

    public static ChangeUserStatusResponse Failure(Guid id, string errorMessage)
    {
        var response = new ChangeUserStatusResponse(id.ToString());
        response.Errors.Add(errorMessage, string.Empty);
        return response;
    }
}

