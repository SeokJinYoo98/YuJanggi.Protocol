#nullable enable
namespace YuJanggi.Protocol.Matching
{
    public enum ProtocolFormation
    {
        HEHE = 0,
        EHEH = 1,
        EHHE = 2,
        HEEH = 3
    }
    public enum MatchingResult
    {
        Accepted            = 0,
        AlreadyMatching     = 1,
        AlreadyMatched      = 2,
        HandshakeRequired   = 3,
        ServerError         = 4
    }
    public enum MatchingCancelResult
    {
        Cancelled = 0,
        NotMatching = 1,
        AlreadyMatched = 2,
        HandshakeRequired = 3,
        ServerError = 4
    }
    public enum FormationSubmitResult
    {
        Accepted = 0,
        AlreadySubmitted = 1,
        NotMatched = 2,
        InvalidFormation = 3,
        HandshakeRequired = 4,
        ServerError = 5
    }
    public enum ProtocolPlayerTeam
    {
        None, Cho, Han
    }
    /// <summary>
    /// 신청 요청의 RequestId를 유지하는 응답입니다.
    /// Accepted는 대기열 접수이며 매칭 완료를 의미하지 않습니다.
    /// </summary>
    public sealed record MatchingStartResponse
    {
        public MatchingResult   Result { get; init; }
        public string?          Message { get; init; }
    }



    /// <summary>취소 요청의 RequestId를 유지하는 응답입니다.</summary>
    public sealed record MatchingCancelResponse
    {
        public MatchingCancelResult Result { get; init; }
        public string?              Message { get; init; }
    }

    /// <summary>매칭된 플레이어의 식별 정보입니다. 진영은 이를 담는 필드로 구분합니다.</summary>
    public sealed record MatchingPlayer
    {
        public string PlayerId { get; init; }
            = string.Empty;
        public string PlayerNickname { get; init; }
            = string.Empty;
        public ProtocolPlayerTeam PlayerTeam { get; init; }
            = ProtocolPlayerTeam.None;
    }
    /// <summary>
    /// 매칭 완료 시 양쪽 클라이언트에 보내는 서버 이벤트입니다(RequestId는 null).
    /// 서버가 MatchId와 초·한 플레이어 정보를 채워 전송합니다.
    /// 대국 시작 및 초기 보드 데이터는 별도 게임 메시지에서 처리합니다.
    /// </summary>
    public sealed record MatchingFoundEvent
    {
        public string MatchId { get; init; }
            = string.Empty;

        public ProtocolPlayerTeam MyTeam { get; init; }
            = ProtocolPlayerTeam.None;

        public MatchingPlayer Opponent { get; init; }
            = new MatchingPlayer();
    }

    /// <summary>
    /// 양측 포진 접수와 GameRoom 생성이 완료된 뒤 최종 포진을 전달하는 서버 이벤트입니다.
    /// 실제 게임 시작을 의미하지 않습니다.
    /// </summary>
    public sealed record GameReadyEvent
    {
        public string MatchId { get; init; } = string.Empty;

        public ProtocolFormation ChoFormation { get; init; }

        public ProtocolFormation HanFormation { get; init; }
    }
    /// <summary>포진 접수 결과입니다. RoomCreated는 게임 시작을 의미하지 않습니다.</summary>
    public sealed record FormationSubmitResponse
    {
        public string MatchId { get; init; }
            = string.Empty;
        public FormationSubmitResult Result { get; init; }
        public bool RoomCreated { get; init; }
    }





}

