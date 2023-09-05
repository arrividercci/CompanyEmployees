using CompanyEmployee.Presentation;
using CompanyEmployees.Extensions;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NLog;
using CompanyEmployees.Presentation.ActionFilters;

namespace CompanyEmployees
{
    public class Program
    {
        
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
            new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
            .Services.BuildServiceProvider()
            .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>().First();

            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            
            builder.Services.ConfigureSqlContext(builder.Configuration);
            
            builder.Services.ConfigureCors();
            
            builder.Services.ConfigureIISIntegration();
            
            builder.Services.ConfigureLoggerService();
            
            builder.Services.ConfigureRepositoryManager();
            
            builder.Services.ConfigureServiceManager();

            builder.Services.AddScoped<ValidationFilterAttribute>();

            builder.Services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader= true;
                config.ReturnHttpNotAcceptable = true;
                config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            }).AddXmlDataContractSerializerFormatters()
            .AddApplicationPart(typeof(AssemplyReference).Assembly);
            
            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILoggerManager>();
            
            app.ConfigureExceptionHandler(logger);
            
            if(app.Environment.IsProduction())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}