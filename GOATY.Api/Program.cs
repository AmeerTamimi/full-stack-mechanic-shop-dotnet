using FluentValidation;
using GOATY.Application.Behaviours;
using GOATY.Application.Features.Common;
using GOATY.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter());
});

builder.Services.AddMediatR(cfg =>
cfg.RegisterServicesFromAssembly(typeof(GOATY.Application.AssemblyMarker).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(GOATY.Application.AssemblyMarker).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionString").Value);
});

builder.Services.AddScoped<IAppDbContext, AppDbContext>();

var app = builder.Build();

app.MapControllers();

app.Run();
