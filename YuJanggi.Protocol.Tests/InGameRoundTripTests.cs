using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YuJanggi.Protocol.InGame;
using YuJanggi.Protocol.Matching;
using YuJanggi.Protocol.Messages;
using YuJanggi.Protocol.Serialization;

namespace YuJanggi.Protocol.Tests
{
    [TestClass]
    public class InGameRoundTripTests
    {
        [TestMethod]
        public void GameStart_RoundTrip_PreservesTheServerTurnAndDeadline()
        {
            var startedAt = new DateTimeOffset(2026, 10, 17, 12, 0, 0, TimeSpan.FromHours(9));
            var payload = new GameStartEvent
            {
                StartedAt = startedAt,
                TurnTimeSeconds = 60f,
                TurnNumber = 1,
                CurrentTeam = ProtocolPlayerTeam.Cho,
                TurnDeadlineAt = startedAt.AddSeconds(60)
            };
            var message = ServerMessageFactory.CreateEvent(ServerMessageType.GameStartEvent, payload);

            var received = MessageSerializer.Deserialize<ServerMessage>(MessageSerializer.Serialize(message));

            Assert.IsNull(received.RequestId);
            Assert.AreEqual(payload, received.GetPayload<GameStartEvent>());
        }

        [TestMethod]
        [DataRow(ProtocolTurnAction.Move, ProtocolPassReason.None, false)]
        [DataRow(ProtocolTurnAction.Pass, ProtocolPassReason.UserRequest, false)]
        [DataRow(ProtocolTurnAction.Pass, ProtocolPassReason.Timeout, false)]
        [DataRow(ProtocolTurnAction.Move, ProtocolPassReason.None, true)]
        public void TurnCompleted_RoundTrip_PreservesPassReasonAndNullableNextTurn(
            ProtocolTurnAction action, ProtocolPassReason reason, bool ended)
        {
            var completedAt = new DateTimeOffset(2026, 10, 17, 3, 1, 0, TimeSpan.Zero);
            var payload = new TurnCompletedEvent
            {
                Team = ProtocolPlayerTeam.Cho,
                Action = action,
                TurnNumber = 3,
                CompletedAt = completedAt,
                NextTeam = ended ? ProtocolPlayerTeam.None : ProtocolPlayerTeam.Han,
                NextTurnDeadlineAt = ended ? null : completedAt.AddSeconds(60),
                PassReason = reason,
                FromX = 0, FromZ = 4, ToX = 0, ToZ = 5
            };
            var message = ServerMessageFactory.CreateEvent(ServerMessageType.TurnCompletedEvent, payload);

            var received = MessageSerializer.Deserialize<ServerMessage>(MessageSerializer.Serialize(message));

            Assert.AreEqual(payload, received.GetPayload<TurnCompletedEvent>());
        }

        [TestMethod]
        public void MoveAndPass_RoundTrip_PreserveTheRequestedTurnNumber()
        {
            var move = new MovePieceRequest
            {
                Team = ProtocolPlayerTeam.Cho, TurnNumber = 3,
                FromX = 0, FromZ = 4, ToX = 0, ToZ = 5
            };
            var moveMessage = ClientMessageFactory.CreateRequest(ClientMessageType.MovePieceRequest, move);
            var receivedMove = MessageSerializer.Deserialize<ClientMessage>(MessageSerializer.Serialize(moveMessage));
            Assert.AreEqual(moveMessage.RequestId, receivedMove.RequestId);
            Assert.AreEqual(move, receivedMove.GetPayload<MovePieceRequest>());

            var pass = new GamePlayerRequest { Requester = ProtocolPlayerTeam.Han, TurnNumber = 4 };
            var passMessage = ClientMessageFactory.CreateRequest(ClientMessageType.PassTurnRequest, pass);
            var receivedPass = MessageSerializer.Deserialize<ClientMessage>(MessageSerializer.Serialize(passMessage));
            Assert.AreEqual(passMessage.RequestId, receivedPass.RequestId);
            Assert.AreEqual(pass, receivedPass.GetPayload<GamePlayerRequest>());
        }
    }
}
