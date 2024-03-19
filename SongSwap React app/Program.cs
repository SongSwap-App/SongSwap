using MassTransit;
using Microsoft.OpenApi.Models;
using SongSwap_React_app.Infrastructure;
using SongSwap_React_app.Models.Services;
using SongSwap_React_app.Startup;
using Startup;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddMassTransit(x =>
//{
//    x.UsingRabbitMq();
//});
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddCofiguredHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Songswap", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:44418", "http://localhost:3000")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        );
});
builder.Services.AddConfiguredJwtAuthentication();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if(app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseLogging();
app.MapHub<ProgressHub>("/hub");

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
