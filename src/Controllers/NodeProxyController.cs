using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VeilNodeProxy.Models.Node;
using VeilNodeProxy.Models.Node.Response;
using VeilNodeProxy.Configs;
using VeilNodeProxy.Services;
using Microsoft.AspNetCore.Cors;
using VeilNodeProxy.Core;

namespace VeilNodeProxy.Controllers;

[ApiController]
[EnableCors(CORSPolicies.NodeProxyPolicy)]
[Route("/")]
public class NodeProxyController : ControllerBase
{
    private readonly static JsonSerializerOptions serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly static List<string> emptyList = new();


    private readonly IOptions<ProxyConfig> _proxyConfig;
    private readonly INodeRequester _nodeRequester;

    public NodeProxyController(IOptions<ProxyConfig> proxyConfig, INodeRequester nodeRequester)
    {
        _proxyConfig = proxyConfig;
        _nodeRequester = nodeRequester;
    }

    [HttpPost]
    public async Task<IActionResult> Post(JsonRPCRequest model, CancellationToken cancellationToken)
    {
        // verify method (and parameters?)
        if (!(_proxyConfig.Value.AllowedMethods?.Contains(model.Method ?? "") ?? false))
        {
            var error = new GenericResult
            {
                Result = null,
                Id = model.Id,
                Error = new()
                {
                    Code = -2,
                    Message = "Forbidden by safe mode" // RPC_FORBIDDEN_BY_SAFE_MODE
                }
            };
            return Content(JsonSerializer.Serialize<GenericResult>(error, serializeOptions), "application/json");
        }

        var res = await _nodeRequester.NodeRequest(model.Method, model.Params, cancellationToken);
        return Content(res, "application/json");
    }
}