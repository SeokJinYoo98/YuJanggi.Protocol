#nullable enable
namespace YuJanggi.Protocol.InGame
{
    using Matching;
    public sealed record GameSceneReady { };

    public sealed record MovePieceRequest
    {
        public ProtocolPlayerTeam Team { get; init; }
        // GameStartEvent에서 받은 첫 턴은 1입니다. 누락된 값(0)은 서버에서 거절합니다.
        public int TurnNumber { get; init; }

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
        public ProtocolGameEndReason    EndReason   { get; init; }
        public ProtocolPlayerTeam       Winner      { get; init; }
        public int                      TotalMoves  { get; init; }
    }
    // GiveUpRequest, PassTurnRequest, RematchRequest가 공통으로 사용합니다.
    public sealed record GamePlayerRequest
    {
        public ProtocolPlayerTeam Requester { get; init; }
        // PassTurnRequest에서만 필수입니다. GiveUp / Rematch에서는 사용하지 않습니다.
        public int TurnNumber { get; init; }
    }
}
