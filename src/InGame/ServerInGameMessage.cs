#nullable enable
using System;

namespace YuJanggi.Protocol.InGame
{
    using Matching;
    public sealed record GameStartEvent
    {
        public DateTimeOffset StartedAt { get; init; }
        public float TurnTimeSeconds { get; init; }
        // 현재 진행할 턴 번호이며 재대결에서도 1부터 시작합니다.
        public int TurnNumber { get; init; }
        public ProtocolPlayerTeam CurrentTeam { get; init; }
        // 서버 시각 기준의 표시·보정 정보입니다. 실제 턴 확정은 서버 Event를 따릅니다.
        public DateTimeOffset? TurnDeadlineAt { get; init; }
    }
    public enum MovePieceResult
    { Accepted, NotYourTurn, PieceNotFound, IllegalMove, OutOfRange, StaleTurn }
    public sealed record MovePieceResponse
    {
        public MovePieceResult Result { get; init; }
    }

    public enum ProtocolTurnAction
    { Move, Pass }

    public enum ProtocolPassReason
    { None, UserRequest, Timeout }

    public sealed record TurnCompletedEvent
    {
        public ProtocolPlayerTeam Team   { get; init; }
        public ProtocolTurnAction Action { get; init; }

        // 이번에 완료된 턴 번호입니다. 게임이 계속되면 다음 요청은 TurnNumber + 1을 사용합니다.
        public int TurnNumber { get; init; }
        public DateTimeOffset CompletedAt { get; init; }
        // 정상 종료된 턴에서는 NextTeam=None, NextTurnDeadlineAt=null입니다.
        public ProtocolPlayerTeam NextTeam { get; init; }
        public DateTimeOffset? NextTurnDeadlineAt { get; init; }
        // Action이 Pass일 때만 UserRequest 또는 Timeout을 사용합니다.
        public ProtocolPassReason PassReason { get; init; }

        // Action이 Move일 때만 사용합니다.
        public byte FromX   { get; init; }
        public byte FromZ   { get; init; }
        public byte ToX     { get; init; }
        public byte ToZ     { get; init; }
    }

    public enum GameRequestResult
    { Accepted, Mismatch, Failed, StaleTurn }
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
