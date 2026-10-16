#nullable enable

namespace YuJanggi.Protocol.Chat
{
    public sealed record ChatRequest
    {
        public string Message { get; init; } = string.Empty;
    }
}
