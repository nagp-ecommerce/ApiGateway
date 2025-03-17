using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using SharedService.Lib.DI;
using ApiGateway.Presentation.Middleware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var env = builder.Environment.EnvironmentName ?? "Development";

builder.Configuration.AddJsonFile($"ocelot.{env}.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());

JwtAuthenticationScheme.AddJwtAuthenticationScheme(builder.Services, builder.Configuration);

builder.Services.AddCors(options=>
   {
       options.AddDefaultPolicy(builder =>
       {
           builder.AllowAnyHeader()
           .AllowAnyMethod().AllowAnyOrigin();
       });
   }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AttachSignatureToRequest>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors();

app.UseOcelot().Wait();

app.Run();
