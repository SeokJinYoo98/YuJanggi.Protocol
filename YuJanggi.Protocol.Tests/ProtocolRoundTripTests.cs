using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using YuJanggi.Protocol.V2.Connection;
using YuJanggi.Protocol.V2.Framing;
using YuJanggi.Protocol.V2.Messages;
using YuJanggi.Protocol.V2.Messages.MessageFactory;
using YuJanggi.Protocol.V2.Serialization;

namespace YuJanggi.Protocol.V2.Tests
{
    [TestClass]
    public class ProtocolRoundTripTests
    {
        [TestMethod]
        public void ProtocolHandshakeRequest_RoundTrip_Success()
        {
            var request = new ProtocolHandshakeRequest
            {
                YuJanggiProtocolVersion = "0.1.0",
                YuJanggiCoreVersion = "1.0.0"
            };

            ClientMessage message =
                ClientMessageFactory.Create(
                    ClientMessageType.HandshakeRequest,
                    request);

            byte[] body =
                MessageSerializer.Serialize(message);

            byte[] packet =
                MessageFramer.Encode(body);

            int bodyLength =
                MessageFramer.DecodeBodyLength(
                    packet.AsSpan(0, MessageFramer.HeaderSize));

            ReadOnlySpan<byte> receivedBody =
                packet.AsSpan(
                    MessageFramer.HeaderSize,
                    bodyLength);

            ClientMessage receivedMessage =
                MessageSerializer.Deserialize<ClientMessage>(
                    receivedBody);

            ProtocolHandshakeRequest receivedRequest =
                receivedMessage.GetPayload<ProtocolHandshakeRequest>();

            Assert.AreEqual(
                ClientMessageType.HandshakeRequest,
                receivedMessage.Type);

            Assert.AreEqual(
                request.YuJanggiProtocolVersion,
                receivedRequest.YuJanggiProtocolVersion);

            Assert.AreEqual(
                request.YuJanggiCoreVersion,
                receivedRequest.YuJanggiCoreVersion);

            Assert.IsFalse(
                string.IsNullOrWhiteSpace(receivedMessage.RequestId));
        }

        [TestMethod]
        public void ProtocolHandshakeResponse_RoundTrip_Success()
        {
            string requestId = Guid.NewGuid().ToString();

            var response = new ProtocolHandshakeResponse
            {
                Result = ProtocolHandshakeResult.Success
            };

            ServerMessage message =
                ServerMessageFactory.CreateResponse(
                    ServerMessageType.ProtocolHandshake,
                    requestId,
                    response);

            byte[] body =
                MessageSerializer.Serialize(message);

            byte[] packet =
                MessageFramer.Encode(body);

            int bodyLength =
                MessageFramer.DecodeBodyLength(
                    packet.AsSpan(0, MessageFramer.HeaderSize));

            ReadOnlySpan<byte> receivedBody =
                packet.AsSpan(
                    MessageFramer.HeaderSize,
                    bodyLength);

            ServerMessage receivedMessage =
                MessageSerializer.Deserialize<ServerMessage>(
                    receivedBody);

            ProtocolHandshakeResponse receivedResponse =
                receivedMessage.GetPayload<ProtocolHandshakeResponse>();

            Assert.AreEqual(
                ServerMessageType.ProtocolHandshake,
                receivedMessage.Type);

            Assert.AreEqual(
                requestId,
                receivedMessage.RequestId);

            Assert.AreEqual(
                ProtocolHandshakeResult.Success,
                receivedResponse.Result);
        }
    }
}