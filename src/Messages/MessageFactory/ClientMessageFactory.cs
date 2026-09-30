#nullable enable
using System;
using System.Text.Json;

namespace YuJanggi.Protocol.Messages
{
    public static class ClientMessageFactory
    {
        public static ClientMessage Create<TPayload>(
            ClientMessageType type,
            TPayload payload)
        {
            return new ClientMessage
            {
                Type = type,
                RequestId = null,
                Payload = JsonSerializer.SerializeToElement(payload)
            };
        }

        public static ClientMessage CreateRequest<TPayload>(
            ClientMessageType type,
            TPayload payload)
        {
            return new ClientMessage
            {
                Type = type,
                RequestId = Guid.NewGuid().ToString(),
                Payload = JsonSerializer.SerializeToElement(payload)
            };
        }
    }
}
