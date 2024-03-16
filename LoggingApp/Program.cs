using LoggingApp.Infrastructure;
using LoggingApp.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(x => 
{
    x.AddConsumer<ActionLogDtoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:5000")
        .SetIsOriginAllowedToAllowWildcardSubdomains()// Add the origin(s) that you want to allow.
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        );
});
builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<LogsDbContext>(opt =>
    {
        opt.UseNpgsql(builder.Configuration.GetConnectionString("postgresql"));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI((options) => 
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}


app.UseCors();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action=Index}/{id?}");

app.Run();
