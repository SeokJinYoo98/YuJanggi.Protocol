#nullable enable
using System;
using System.Text.Json;

namespace YuJanggi.Protocol.Messages
{
    public static class ClientMessageFactory
    {
        public static ClientMessage Create<TPayload>(
            ClientMessageType   type,
            TPayload            payload)
        {
            if (payload is null)
                throw new ArgumentNullException(nameof(payload));

            return new ClientMessage
            {
                Type        = type,
                RequestId   = Guid.NewGuid().ToString(),
                Payload     = JsonSerializer.SerializeToElement(payload)
            };
        }
    }
}
