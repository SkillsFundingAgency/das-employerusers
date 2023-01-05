using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.EmployerProfiles.Api.AppStart;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;
using SFA.DAS.EmployerProfiles.Data;
using SFA.DAS.EmployerProfiles.Domain.Configuration;

var builder = WebApplication.CreateBuilder(args);

var rootConfiguration = builder.Configuration.LoadConfiguration();

builder.Services.AddServiceRegistration();
var employerProfilesConfiguration = rootConfiguration
    .GetSection(nameof(EmployerProfilesConfiguration))
    .Get<EmployerProfilesConfiguration>();
builder.Services.AddDatabaseRegistration(employerProfilesConfiguration, rootConfiguration["EnvironmentName"]);
builder.Services.AddMediatR(typeof(GetUserByEmailQuery));

if (rootConfiguration["EnvironmentName"] != "DEV")
{
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<EmployerProfilesDataContext>();

}
builder.Services
    .AddMvc(o =>
    {
        if (!(rootConfiguration["EnvironmentName"]!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
              rootConfiguration["EnvironmentName"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase)))
        {
            o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string> { "" }));
        }
        o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmployerProfilesApi", Version = "v1" });
    c.OperationFilter<SwaggerVersionHeaderFilter>();
});
            
builder.Services.AddApiVersioning(opt => {
    opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmployerProfilesApi v1");
    c.RoutePrefix = string.Empty;
});
            
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();

if (!app.Configuration["EnvironmentName"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
{
    app.UseHealthChecks();
}

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpointBuilder =>
{
    endpointBuilder.MapControllerRoute(
        name: "default",
        pattern: "api/{controller=Users}/{action=Index}/{id?}");
});

app.Run();
