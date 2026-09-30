#nullable enable
using System.IO;
using System.Text.Json;

namespace YuJanggi.Protocol.Messages
{
    // Request   = 어떤 처리를 요청
    // Submit = 클라이언트가 선택/데이터 제출
    // Snapshot = 현재 상태 전체 전달
    // Ready = 다음 핸들러로 넘어갈 준비 완료
    public enum ClientMessageType
    {
        // Connect
        HandshakeRequest = 0,
        // Matching
        MatchingStartRequest = 1, MatchingCancelRequest = 2, FormationSubmit = 3,
        // InGame
        GameSceneReady = 4, MovePieceRequest = 5
    }
    /// <summary>
    /// [Type]
    /// : 메시지 종류를 구분합니다.
    /// [RequestId]
    /// : 요청에 대한 응답인 경우 원본 요청 ID를 담습니다.
    ///  서버 이벤트처럼 특정 요청과 관계없으면 null입니다.
    /// [Payload]
    /// : 실제 메시지 데이터입니다.
    /// </summary>
    public sealed record ClientMessage
    {

        public ClientMessageType    Type { get; init; }
        public string?              RequestId { get; init; }
        public JsonElement?         Payload { get; init; }

        public TPayload GetPayload<TPayload>()
        {
            if (Payload is null)
            {
                throw new InvalidDataException(
                    "Payload가 존재하지 않습니다.");
            }

            return Payload.Value.Deserialize<TPayload>()
                ?? throw new InvalidDataException($"{typeof(TPayload).Name} Payload를 역직렬화할 수 없습니다.");
        }
    }
}
