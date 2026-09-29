#nullable enable
namespace YuJanggi.Protocol.Matching
{
    /// <summary>매칭 대기열 참가 요청입니다. 플레이어 식별은 서버 세션을 사용합니다.</summary>
    public sealed record MatchingRequest
    {
    }

    /// <summary>현재 매칭 대기열에서 나가기 위한 요청입니다.</summary>
    public sealed record MatchingCancelRequest
    {
    }

    public sealed record FormationSubmitRequest
    {
        public string MatchId { get; init; } 
            = string.Empty;
        public ProtocolFormation Formation { get; init; }
    }
}
