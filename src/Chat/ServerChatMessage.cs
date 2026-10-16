#nullable enable

namespace YuJanggi.Protocol.Chat
{
    using Matching;

    public enum ChatResult
    {
        Accepted,
        Failed
    }

    public sealed record ChatResponse
    {
        public ChatResult Result { get; init; }
    }

    public sealed record ChatEvent
    {
        public ProtocolPlayerTeam Team { get; init; }
        public string Message { get; init; } = string.Empty;
    }
}
