using System;
using System.IO;
using System.Text.Json;

namespace YuJanggi.Protocol.Serialization
{

    public static class MessageSerializer
    {
        public static byte[] Serialize<TMessage>(TMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            return JsonSerializer.SerializeToUtf8Bytes(message);
        }

        public static TMessage Deserialize<TMessage>(ReadOnlySpan<byte> data)
        {
            return JsonSerializer.Deserialize<TMessage>(data)
                ?? throw new InvalidDataException(
                    $"{typeof(TMessage).Name} 메시지를 역직렬화할 수 없습니다.");
        }
    }
}