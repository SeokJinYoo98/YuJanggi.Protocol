using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using YuJanggi.Protocol.V2.Connection;
using YuJanggi.Protocol.V2.Framing;
using YuJanggi.Protocol.V2.Messages;
using YuJanggi.Protocol.V2.Messages.MessageFactory;
using YuJanggi.Protocol.V2.Serialization;

namespace YuJanggi.Protocol.V2.Tests
{
    [TestClass]
    public class MessageFramerTests
    {
        [TestMethod]
        public void MessageFramer_Encode_EmptyBody_ThrowsInvalidDataException()
        {
            byte[] body = Array.Empty<byte>();

            Assert.ThrowsExactly<InvalidDataException>(
                () => MessageFramer.Encode(body));
        }

        [TestMethod]
        public void MessageFramer_Encode_TooLargeBody_ThrowsInvalidDataException()
        {
            byte[] body = new byte[MessageFramer.MaxBodySize + 1];

            Assert.ThrowsExactly<InvalidDataException>(
                () => MessageFramer.Encode(body));
        }

        [TestMethod]
        public void MessageFramer_DecodeBodyLength_InvalidHeaderSize_ThrowsArgumentException()
        {
            byte[] header = new byte[MessageFramer.HeaderSize - 1];

            Assert.ThrowsExactly<ArgumentException>(
                () => MessageFramer.DecodeBodyLength(header));
        }
    }
}