namespace VeilNodeProxy.Models.Node.Response;

public class GenericResult : JsonRPCResponse
{
    public object? Result { get; set; }
}