using ASP.Serilog.Services;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;

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

            //middleware exceptions
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    if (exception != null)
                    {
                        var message = $"{exception.Error.Message}";
                        await context.Response.WriteAsync(message).ConfigureAwait(false);
                    }
                });
            });

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