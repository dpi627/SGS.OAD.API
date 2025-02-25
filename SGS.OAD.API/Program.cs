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

            // 取得環境變數
            string environment = builder.Environment.EnvironmentName;
            string appName = builder.Environment.ApplicationName;
            string basePath = builder.Environment.ContentRootPath;

            // 設定組態檔
            builder.Configuration.SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>();

            // 設定 Serilog
            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.WithProperty("Application", appName) //應用程式名稱不寫於設定檔
                    .CreateLogger();

            Log.Information("{Application} 於 {EnvironmentName} 啟動", appName, environment);

            try
            {
                // 清除預設的日誌提供者
                builder.Logging.ClearProviders();
                // 使用 Serilog 取代內建的日誌機制
                builder.Logging.AddSerilog();

                builder.Services.AddControllers();

                builder.Services.AddOpenApi();

                // 以下程式可將名稱為 password 的欄位設定為 password 格式，但 scalar ui 無法正確顯示
                //builder.Services.AddOpenApi(o =>
                //{
                //    o.AddOperationTransformer(async (operation, context, cancellationToken) =>
                //    {
                //        if (operation.Parameters != null)
                //        {
                //            foreach (var param in operation.Parameters)
                //            {
                //                if (param.Name.Equals("password", StringComparison.OrdinalIgnoreCase))
                //                {
                //                    if (param.Schema.Type == "string")
                //                    {
                //                        param.Schema.Format = "password";
                //                    }
                //                }
                //            }
                //        }
                //        await Task.CompletedTask;
                //    });
                //});

                builder.Services.AddHealthChecks()
                    .AddCheck("Self", () => HealthCheckResult.Healthy("API is running"), tags: ["self"]);

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
                Log.Fatal(ex, "{Application} 異常", appName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
