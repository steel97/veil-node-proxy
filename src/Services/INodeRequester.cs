namespace VeilNodeProxy.Services;

public interface INodeRequester
{
    Task<string> NodeRequest(string? method, List<object>? parameters, CancellationToken cancellationToken);
}