using CompanyEmployees;
using CompanyEmployees.Extensions;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NLog;
using Presentation;
using Microsoft.AspNetCore.Mvc.Formatters;
using Presentation.ActionFilters;
using Service.DataShaping;
using Shared.DataTransferObjects;
using CompanyEmployees.Utility;


var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureRateLimitingOptions();
//builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureOutputCaching();
builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
    //config.CacheProfiles.Add("120SecondsDuration", new CacheProfile{Duration = 120});

})
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(AssemblyReference).Assembly);
builder.Services.AddCustomMediaTypes();
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.ConfigureSwagger();

var app = builder.Build();

//var logger = app.Services.GetRequiredService<ILoggerManager>();
//app.ConfigureExceptionHandler(logger);
app.UseExceptionHandler(opt => { });
if (app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
});
app.UseRateLimiter();
app.UseCors("CorsPolicy");
//app.UseResponseCaching();
app.UseOutputCache();
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
.Services.BuildServiceProvider()
.GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
.OfType<NewtonsoftJsonPatchInputFormatter>().First();