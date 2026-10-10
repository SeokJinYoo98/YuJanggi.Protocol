using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YuJanggi.Protocol.Framing;
using YuJanggi.Protocol.Matching;
using YuJanggi.Protocol.Messages;
using YuJanggi.Protocol.Serialization;

namespace YuJanggi.Protocol.Tests
{
    [TestClass]
    public class MatchingRoundTripTests
    {
        [TestMethod]
        public void MatchingStartRequest_RoundTrip_PreservesRequestIdAndPayload()
        {
            var message = ClientMessageFactory.CreateRequest(
                ClientMessageType.MatchingStartRequest, new MatchingStartRequest());

            var received = RoundTrip(message);

            Assert.AreEqual(ClientMessageType.MatchingStartRequest, received.Type);
            Assert.IsFalse(string.IsNullOrWhiteSpace(received.RequestId));
            Assert.AreEqual(message.RequestId, received.RequestId);
            Assert.IsNotNull(received.GetPayload<MatchingStartRequest>());
        }

        [TestMethod]
        public void MatchingCancelRequest_RoundTrip_PreservesRequestIdAndPayload()
        {
            var message = ClientMessageFactory.CreateRequest(
                ClientMessageType.MatchingCancelRequest, new MatchingCancelRequest());

            var received = RoundTrip(message);

            Assert.AreEqual(ClientMessageType.MatchingCancelRequest, received.Type);
            Assert.IsFalse(string.IsNullOrWhiteSpace(received.RequestId));
            Assert.AreEqual(message.RequestId, received.RequestId);
            Assert.IsNotNull(received.GetPayload<MatchingCancelRequest>());
        }

        [TestMethod]
        [DataRow(MatchingResult.Accepted, null)]
        [DataRow(MatchingResult.AlreadyMatching, "이미 매칭 중입니다.")]
        [DataRow(MatchingResult.AlreadyMatched, "매칭이 완료되었습니다.")]
        [DataRow(MatchingResult.HandshakeRequired, "핸드셰이크가 필요합니다.")]
        [DataRow(MatchingResult.ServerError, "서버 오류")]
        public void MatchingStartResponse_RoundTrip_PreservesResultAndRequestId(
            MatchingResult result, string? detail)
        {
            var request = ClientMessageFactory.CreateRequest(
                ClientMessageType.MatchingStartRequest, new MatchingStartRequest());
            var response = new MatchingStartResponse { Result = result, Message = detail };
            var message = ServerMessageFactory.CreateResponse(
                ServerMessageType.MatchingStartResponse, request.RequestId!, response);

            var received = RoundTrip(message);
            var payload = received.GetPayload<MatchingStartResponse>();

            Assert.AreEqual(ServerMessageType.MatchingStartResponse, received.Type);
            Assert.AreEqual(request.RequestId, received.RequestId);
            Assert.AreEqual(result, payload.Result);
            Assert.AreEqual(detail, payload.Message);
        }

        [TestMethod]
        [DataRow(MatchingCancelResult.Cancelled, null)]
        [DataRow(MatchingCancelResult.NotMatching, "매칭 대기 중이 아닙니다.")]
        [DataRow(MatchingCancelResult.AlreadyMatched, "매칭이 이미 완료되었습니다.")]
        [DataRow(MatchingCancelResult.HandshakeRequired, "핸드셰이크가 필요합니다.")]
        [DataRow(MatchingCancelResult.ServerError, "서버 오류")]
        public void MatchingCancelResponse_RoundTrip_PreservesResultAndRequestId(
            MatchingCancelResult result, string? detail)
        {
            var request = ClientMessageFactory.CreateRequest(
                ClientMessageType.MatchingCancelRequest, new MatchingCancelRequest());
            var response = new MatchingCancelResponse { Result = result, Message = detail };
            var message = ServerMessageFactory.CreateResponse(
                ServerMessageType.MatchingCancelResponse, request.RequestId!, response);

            var received = RoundTrip(message);
            var payload = received.GetPayload<MatchingCancelResponse>();

            Assert.AreEqual(ServerMessageType.MatchingCancelResponse, received.Type);
            Assert.AreEqual(request.RequestId, received.RequestId);
            Assert.AreEqual(result, payload.Result);
            Assert.AreEqual(detail, payload.Message);
        }

        [TestMethod]
        public void MatchingFound_RoundTrip_PreservesPlayersAndHasNoRequestId()
        {
            var match = new MatchingFoundEvent
            {
                MatchId = "match-001",
                MyTeam = ProtocolPlayerTeam.Cho,
                Opponent = new MatchingPlayer
                {
                    PlayerId = "player-han", PlayerNickname = "한 플레이어",
                    PlayerTeam = ProtocolPlayerTeam.Han
                }
            };
            var message = ServerMessageFactory.CreateEvent(ServerMessageType.MatchingFoundEvent, match);

            var received = RoundTrip(message);
            var payload = received.GetPayload<MatchingFoundEvent>();

            Assert.AreEqual(ServerMessageType.MatchingFoundEvent, received.Type);
            Assert.IsNull(received.RequestId);
            Assert.AreEqual(match.MatchId, payload.MatchId);
            Assert.AreEqual(match.MyTeam, payload.MyTeam);
            Assert.AreEqual(match.Opponent.PlayerId, payload.Opponent.PlayerId);
            Assert.AreEqual(match.Opponent.PlayerNickname, payload.Opponent.PlayerNickname);
            Assert.AreEqual(match.Opponent.PlayerTeam, payload.Opponent.PlayerTeam);
        }

        [TestMethod]
        [DataRow(ProtocolFormation.HEHE, 0, ProtocolFormation.HEEH, 3)]
        [DataRow(ProtocolFormation.EHEH, 1, ProtocolFormation.EHHE, 2)]
        [DataRow(ProtocolFormation.EHHE, 2, ProtocolFormation.EHEH, 1)]
        [DataRow(ProtocolFormation.HEEH, 3, ProtocolFormation.HEHE, 0)]
        public void GameReady_RoundTrip_PreservesMatchAndFormationWireValues(
            ProtocolFormation cho, int choValue, ProtocolFormation han, int hanValue)
        {
            var ready = new GameReadyEvent
            {
                MatchId = "match-ready-001", ChoFormation = cho, HanFormation = han
            };
            var message = ServerMessageFactory.CreateEvent(ServerMessageType.GameReadyEvent, ready);
            var received = RoundTrip(message);
            var payload = received.GetPayload<GameReadyEvent>();

            Assert.AreEqual(ServerMessageType.GameReadyEvent, received.Type);
            Assert.AreEqual(203, (int)received.Type);
            Assert.IsNull(received.RequestId);
            Assert.AreEqual(ready.MatchId, payload.MatchId);
            Assert.AreEqual(cho, payload.ChoFormation);
            Assert.AreEqual(han, payload.HanFormation);
            Assert.AreEqual(choValue, received.Payload!.Value.GetProperty("ChoFormation").GetInt32());
            Assert.AreEqual(hanValue, received.Payload!.Value.GetProperty("HanFormation").GetInt32());
        }

        private static TMessage RoundTrip<TMessage>(TMessage message)
        {
            byte[] body = MessageSerializer.Serialize(message);
            byte[] packet = MessageFramer.Encode(body);
            int bodyLength = MessageFramer.DecodeBodyLength(
                packet.AsSpan(0, MessageFramer.HeaderSize));

            Assert.AreEqual(body.Length, bodyLength);
            return MessageSerializer.Deserialize<TMessage>(
                packet.AsSpan(MessageFramer.HeaderSize, bodyLength));
        }
    }
}
