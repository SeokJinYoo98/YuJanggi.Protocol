using System;
using System.Text.Json;

namespace YuJanggi.Protocol.Messages
{
    public static class ServerMessageFactory
    {
        /// <summary>
        /// CreateResponse
        /// 클라이언트 요청 → 서버 응답
        /// RequestId = 클라이언트가 보낸 RequestId 그대로
        /// </summary>
        public static ServerMessage CreateResponse<TPayload>(
            ServerMessageType type,
            string            requestId,
            TPayload          payload)
        {
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new ArgumentException(
                    "RequestId는 비어 있을 수 없습니다.",
                    nameof(requestId));
            }

            if (payload is null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return new ServerMessage
            {
                Type            = type,
                RequestId       = requestId,
                Payload         = JsonSerializer.SerializeToElement(payload)
            };
        }
        /// <summary>
        /// CreateEvent
        /// 서버가 먼저 보내는 이벤트
        /// RequestId = null
        /// </summary>
        public static ServerMessage CreateEvent<TPayload>(
            ServerMessageType type,
            TPayload payload)
        {
            if (payload is null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            return new ServerMessage
            {
                Type = type,
                RequestId = null,
                Payload = JsonSerializer.SerializeToElement(payload)
            };
        }
    }
}