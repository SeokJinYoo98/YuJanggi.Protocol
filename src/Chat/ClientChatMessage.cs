#nullable enable

using YuJanggi.Protocol.Matching;

namespace YuJanggi.Protocol.Chat
{
    public sealed record ChatRequest
    {
        public ProtocolPlayerTeam Requester { get; init; } = ProtocolPlayerTeam.None;
        public string Message { get; init; } = string.Empty;
    }
}
