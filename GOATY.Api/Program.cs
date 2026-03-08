using GOATY.Api;
using GOATY.Application;
using GOATY.Infrastructure;
using GOATY.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation()
                .AddApplication(builder.Configuration)
                .AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.InitilizedDatabaseAsync();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
