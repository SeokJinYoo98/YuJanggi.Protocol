using System;
using System.Buffers.Binary;
using System.IO;

namespace YuJanggi.Protocol.Framing
{

    public static class MessageFramer
    {
        public const int HeaderSize = sizeof(int);

        public const int MaxBodySize = 4 * 1024;

        public static byte[] Encode(ReadOnlySpan<byte> body)
        {
            if (body.Length <= 0 || body.Length > MaxBodySize)
            {
                throw new InvalidDataException(
                    $"유효하지 않은 본문 크기입니다: {body.Length}");
            }

            byte[] packet = new byte[HeaderSize + body.Length];

            BinaryPrimitives.WriteInt32BigEndian(
                packet.AsSpan(0, HeaderSize),
                body.Length);

            body.CopyTo(packet.AsSpan(HeaderSize));

            return packet;
        }

        public static int DecodeBodyLength(ReadOnlySpan<byte> header)
        {
            if (header.Length != HeaderSize)
            {
                throw new ArgumentException(
                    $"헤더 크기는 반드시 {HeaderSize} bytes여야 합니다.");
            }

            int bodyLength =
                BinaryPrimitives.ReadInt32BigEndian(header);

            if (bodyLength <= 0 || bodyLength > MaxBodySize)
            {
                throw new InvalidDataException(
                    $"유효하지 않은 본문 크기입니다: {bodyLength}");
            }

            return bodyLength;
        }
    }
}