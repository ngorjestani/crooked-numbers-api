using CrookedNumbers.Api.Features.Teams;
using CrookedNumbers.Api.Infrastructure;
using CrookedNumbers.Api.Integrations.Mlb;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton(TimeProvider.System);

builder
    .Services.AddOptions<MlbApiOptions>()
    .Bind(builder.Configuration.GetSection(MlbApiOptions.SectionName))
    .Validate(options => options.BaseUrl.IsAbsoluteUri, "MlbApi:BaseUrl must be absolute.")
    .ValidateOnStart();

builder.Services.AddHttpClient<IMlbApiClient, MlbApiClient>(
    (services, client) =>
    {
        var options = services.GetRequiredService<IOptions<MlbApiOptions>>().Value;
        client.BaseAddress = options.BaseUrl;
        client.Timeout = TimeSpan.FromSeconds(15);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("CrookedNumbers/1.0");
    }
);

builder.Services.AddScoped<GetTeamsHandler>();
builder.Services.AddScoped<MlbTeamLookup>();
builder.Services.AddScoped<GetTeamHandler>();
builder.Services.AddScoped<GetTeamRosterHandler>();
builder.Services.AddScoped<GetTeamStatsHandler>();
builder.Services.AddScoped<GetTeamScheduleHandler>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();

public partial class Program;
