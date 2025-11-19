using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpsertUser;

namespace SFA.DAS.EmployerProfiles.Application.UnitTests.Users.Handlers.Commands;

public class WhenValidatingUpsertingUser
{
    [Test, MoqAutoData]
    public async Task Then_If_No_Email_Then_Invalid(
        UpsertUserRequest request,
        UpsertUserRequestValidator validator)
    {
        request.Email = string.Empty;

        var actual = await validator.ValidateAsync(request);

        actual.IsValid().Should().BeFalse();
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Email_Is_No_In_The_Correct_Format_Then_Invalid(
        UpsertUserRequest request,
        UpsertUserRequestValidator validator)
    {
        request.Email = "test";

        var actual = await validator.ValidateAsync(request);

        actual.IsValid().Should().BeFalse();
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Empty_Guid_For_Id_Then_Invalid(
        UpsertUserRequest request,
        UpsertUserRequestValidator validator)
    {
        request.Email = "tes't@test.com";
        request.Id = Guid.Empty;
        
        var actual = await validator.ValidateAsync(request);

        actual.IsValid().Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task Then_Valid_If_Email_And_Id_Present(
        UpsertUserRequest request,
        UpsertUserRequestValidator validator)
    {
        request.Email = "tes't@test.com";
        
        var actual = await validator.ValidateAsync(request);

        actual.IsValid().Should().BeTrue();
    }
}