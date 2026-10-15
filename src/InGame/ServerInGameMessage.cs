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

    public enum ProtocolTurnAction
    {
        Move,
        Pass
    }

    public sealed record TurnCompletedEvent
    {
        public ProtocolPlayerTeam Team   { get; init; }
        public ProtocolTurnAction Action { get; init; }

        // Action이 Move일 때만 사용합니다.
        public byte FromX   { get; init; }
        public byte FromZ   { get; init; }
        public byte ToX     { get; init; }
        public byte ToZ     { get; init; }
    }

    public enum GameRequestResult
    {
        Accepted,
        Mismatch,
        Failed
    }
    public sealed record GameRequestResponse
    {
        public GameRequestResult Result { get; init; }
    }
    public sealed record GameEndedEvent
    {
        public ProtocolPlayerTeam Winner { get; init; }
        public ProtocolGameEndReason EndReason { get; init; }
        public int TotalMoves { get; init; }
    }

    // 양쪽이 재대결을 신청하면 전송합니다. 대국 시작은 기존 GameSceneReady/GameStartEvent를 사용합니다.
    public sealed record RematchConfirmedEvent { }

}
