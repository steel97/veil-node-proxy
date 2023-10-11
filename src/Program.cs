using Serilog;
using Serilog.Events;
using VeilNodeProxy.Configs;
using VeilNodeProxy.Core;
using VeilNodeProxy.Services;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Async(a => a.Console())
            .WriteTo.File(new Serilog.Formatting.Compact.CompactJsonFormatter(), "")
            .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());

// Configuration
builder.Services.Configure<ProxyConfig>(builder.Configuration.GetSection("Proxy"));

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<INodeRequester, NodeRequester>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORSPolicies.NodeProxyPolicy, corsBuilder => corsBuilder
                 .AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());
});

var app = builder.Build();
app.UseRouting();
app.UseCors(CORSPolicies.NodeProxyPolicy);
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
