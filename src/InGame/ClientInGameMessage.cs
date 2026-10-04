#nullable enable
namespace YuJanggi.Protocol.InGame
{
    using Matching;
    public sealed record GameSceneReady { };

    public sealed record MovePieceRequest
    {
        public ProtocolPlayerTeam Team { get; init; }

        public byte FromX { get; init; }
        public byte FromZ { get; init; }

        public byte ToX { get; init; }
        public byte ToZ { get; init; }
    }
    public enum ProtocolGameEndReason
    {
        Draw, CheckMate, GiveUp, Score
    }
    public sealed record GameEndRequest
    {
        public ProtocolGameEndReason EndReason { get; init; }
        public ProtocolPlayerTeam Winner { get; init; }
        public int TotalMoves { get; init; }
    }
}
