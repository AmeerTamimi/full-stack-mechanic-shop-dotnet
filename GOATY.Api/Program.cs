using GOATY.Api;
using GOATY.Application;
using GOATY.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentation()
                .AddApplication(builder.Configuration)
                .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.Run();
