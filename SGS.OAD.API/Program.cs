using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using Serilog;

namespace SGS.OAD.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ���o�����ܼ�
            string environment = builder.Environment.EnvironmentName;
            string appName = builder.Environment.ApplicationName;
            string basePath = builder.Environment.ContentRootPath;

            // �]�w�պA��
            builder.Configuration.SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>();

            // �]�w Serilog
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.WithProperty("Application", appName) //���ε{���W�٤��g��]�w��
                    .CreateLogger();

            Log.Information("{Application} �� {EnvironmentName} �Ұ�", appName, environment);

            try
            {
                // �M���w�]����x���Ѫ�
                builder.Logging.ClearProviders();
                // �ϥ� Serilog ���N���ت���x����
                builder.Logging.AddSerilog();

                builder.Services.AddControllers();
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();

                builder.Services.AddHealthChecks()
                    .AddCheck("Self", () => HealthCheckResult.Healthy("API is running"), tags: new[] { "self" });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.MapOpenApi();
                app.MapScalarApiReference(option =>
                {
                    option.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "{Application} ���`", appName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
