namespace VeilNodeProxy.Configs;

public class ProxyConfig
{
    public List<string>? AllowedMethods { get; set; }
    public Node? Node { get; set; }
}

public class Node
{
    public string? Url { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}