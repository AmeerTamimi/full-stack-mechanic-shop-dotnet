using GOATY.Application.Behaviours;
using GOATY.Application.Common;
using GOATY.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMediatR(config
    => config.RegisterServicesFromAssemblies(typeof(GOATY.Application.AssemblyMarker).Assembly));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionString").Value);
});

builder.Services.AddScoped<IAppDbContext, AppDbContext>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>) , typeof(ValidationBehaviour<,>));

var app = builder.Build();

app.MapControllers();

app.Run();
