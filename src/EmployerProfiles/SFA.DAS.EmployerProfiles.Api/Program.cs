using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.EmployerProfiles.Api.AppStart;
using SFA.DAS.EmployerProfiles.Api.Controllers;
using SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;
using SFA.DAS.EmployerProfiles.Data;
using SFA.DAS.EmployerProfiles.Domain.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.LoadConfiguration();

builder.Services.AddServiceRegistration();
var coursesConfiguration = builder.Configuration
    .GetSection(nameof(EmployerProfilesConfiguration))
    .Get<EmployerProfilesConfiguration>();
builder.Services.AddDatabaseRegistration(coursesConfiguration, builder.Configuration["Environment"]);
builder.Services.AddMediatR(typeof(GetUserByEmailQuery));

if (builder.Configuration["Environment"] != "DEV")
{
    builder.Services.AddHealthChecks();
        //.AddDbContextCheck<EmployerProfilesDataContext>();

}
builder.Services
    .AddMvc(o =>
    {
        if (!(builder.Configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
              builder.Configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase)))
        {
            o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string> { PolicyNames.DataLoad }));
        }
        o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//builder.Services.AddApplicationInsightsTelemetry(_configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

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

//app.ConfigureExceptionHandler(logger);

app.UseAuthentication();

if (!app.Configuration["Environment"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
{
    app.UseHealthChecks();
}

app.UseRouting();
app.UseEndpoints(endpointBuilder =>
{
    endpointBuilder.MapControllerRoute(
        name: "default",
        pattern: "api/{controller=Users}/{action=Index}/{id?}");
});
app.Run();
