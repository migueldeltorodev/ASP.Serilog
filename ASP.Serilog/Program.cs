using ASP.Serilog.Services;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("starting server.");
            var builder = WebApplication.CreateBuilder(args);
            //using Serilog
            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.WriteTo.Console();
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //add custom services
            builder.Services.AddTransient<IDummyService, DummyService>();

            var app = builder.Build();

            //minimal endpoints
            app.MapGet("/", (IDummyService svc) => svc.DoSomething());

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "server terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}