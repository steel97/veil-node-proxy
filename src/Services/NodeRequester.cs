using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using VeilNodeProxy.Configs;
using VeilNodeProxy.Models.Node;
using VeilNodeProxy.Models.Node.Response;

namespace VeilNodeProxy.Services;

public class NodeRequester : INodeRequester
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<ProxyConfig> _proxyConfig;
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public NodeRequester(IHttpClientFactory httpClientFactory, IOptionsMonitor<ProxyConfig> proxyConfig) =>
        (_proxyConfig, _httpClientFactory) = (proxyConfig, httpClientFactory);


    private void ConfigureHttpClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(_proxyConfig.CurrentValue.Node);
        ArgumentNullException.ThrowIfNull(_proxyConfig.CurrentValue.Node.Url);
        ArgumentNullException.ThrowIfNull(_proxyConfig.CurrentValue.Node.Username);
        ArgumentNullException.ThrowIfNull(_proxyConfig.CurrentValue.Node.Password);

        httpClient.BaseAddress = new Uri(_proxyConfig.CurrentValue.Node.Url);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_proxyConfig.CurrentValue.Node.Username}:{_proxyConfig.CurrentValue.Node.Password}")));
    }

    public async Task<string> NodeRequest(string? method, List<object>? parameters, CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = _httpClientFactory.CreateClient();

            ConfigureHttpClient(httpClient);

            var request = new JsonRPCRequest
            {
                Id = 1,
                Method = method,
                Params = parameters
            };
            var response = await httpClient.PostAsJsonAsync<JsonRPCRequest>("", request, serializerOptions, cancellationToken);
            return await response.Content.ReadAsStringAsync(cancellationToken);

        }
        catch
        {

        }

        var error = new GenericResult
        {
            Result = null,
            Error = new()
            {
                Code = -32603,
                Message = "Node failure"
            }
        };
        return JsonSerializer.Serialize<GenericResult>(error, serializerOptions); // RPC_INTERNAL_ERROR -32603
    }
}