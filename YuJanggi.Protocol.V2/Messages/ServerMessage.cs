using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

/*
    잘못된 MessageType
    Payload 역직렬화 실패
    지원하지 않는 요청
    서버 내부 프로토콜 오류 
*/
namespace YuJanggi.Protocol.V2.Messages
{

    public enum ServerMessageType
    {
        // Connecting
        ProtocolHandshake = 0,

        // Matching
        MatchingResponse = 1,
        MatchingCancelResponse = 2,
        MatchingFound = 3,
        FormationSubmitResponse = 4,
        GameReady = 5,

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
