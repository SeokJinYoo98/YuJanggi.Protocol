#nullable enable
using System;

namespace YuJanggi.Protocol.InGame
{
    using Matching;
    public sealed record GameStartEvent
    {
        public DateTimeOffset StartedAt { get; init; }
    }
    public enum MovePieceResult
    { Accepted, NotYourTurn, PieceNotFound, IllegalMove, OutOfRange }
    public sealed record MovePieceResponse
    {
        public MovePieceResult Result { get; init; }
    }

    public sealed record MovePieceEvent
    {
        public ProtocolPlayerTeam Team { get; init; }

        public byte FromX { get; init; }
        public byte FromZ { get; init; }

        public byte ToX { get; init; }
        public byte ToZ { get; init; }
    }

    public enum GameEndResult
    {
        Accepted,
        Mismatch,
        Failed
    }
    public sealed record GameEndResponse
    {
        public GameEndResult Result { get; init; }
    }
    public sealed record GameEndedEvent
    {
        public ProtocolPlayerTeam Winner { get; init; }
        public int TotalMoves { get; init; }
    }
}
