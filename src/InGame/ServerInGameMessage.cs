#nullable enable
using System;

namespace YuJanggi.Protocol.InGame
{
    using Matching;
    public sealed record GameStartEvent
    {
        public DateTimeOffset StartedAt { get; init; }
    }

    public sealed record MovePieceEvent
    {
        public ProtocolPlayerTeam Team { get; init; }

        public byte FromX { get; init; }
        public byte FromZ { get; init; }

        public byte ToX { get; init; }
        public byte ToZ { get; init; }
    }
}
