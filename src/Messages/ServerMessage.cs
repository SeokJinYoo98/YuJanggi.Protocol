#nullable enable
using System.IO;
using System.Text.Json;

/*
    잘못된 MessageType
    Payload 역직렬화 실패
    지원하지 않는 요청
    서버 내부 프로토콜 오류 
*/
namespace YuJanggi.Protocol.Messages
{
    // Response  = Request/Submit 처리 결과
    // Event = 서버에서 발생한 사실 전달
    // Command = 서버가 클라이언트에게 행동 요구
    // Snapshot = 현재 상태 전체 전달
    public enum ServerMessageType
    {
        // Connecting
        HandshakeResponse = 0,

        // Matching
        MatchingStartResponse = 1, MatchingCancelResponse = 2,
        MatchingFoundEvent = 3,
        FormationSubmitCommand = 4,
        GameReadyEvent = 5,

        // InGame
        GameStartEvent = 6,
        MovePieceEvent = 7,

        Error = 100,

    }
    public sealed record ServerMessage
    {
        public ServerMessageType    Type { get; init; }

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
