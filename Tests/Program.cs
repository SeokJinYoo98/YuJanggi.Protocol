using YuJanggiCommon;
using System.Buffers.Binary;
var id = Guid.NewGuid();
var start = new GameStartEvent(id, PlayerSide.Han, PlayerSide.Cho,
    new[] { new BoardPieceState(3, 4, 8, PlayerSide.Han, GamePieceType.King) })
    { ChoFormation = GameFormation.HEHE, HanFormation = GameFormation.HEEH };
var packet = MessageProtocol.Encode(ChatMessage.Create(MessageType.GameStart, "request", start));
if (BinaryPrimitives.ReadInt32BigEndian(packet.AsSpan(0,4)) != packet.Length - 4) throw new Exception("Header mismatch");
var decoded = MessageProtocol.DecodeBody(packet.AsSpan(4));
var result = decoded.GetPayload<GameStartEvent>();
if(result.GameId != id || result.HanFormation != GameFormation.HEEH || result.Pieces[0].PieceId != 3 || decoded.RequestId != "request") throw new Exception("Roundtrip failed");
foreach(var length in new[]{0,-1,MessageProtocol.MaxBodySize+1}) {
    var header = new byte[4]; BinaryPrimitives.WriteInt32BigEndian(header,length);
    try { MessageProtocol.DecodeBodyLength(header); } catch(InvalidDataException) { continue; }
    throw new Exception("Invalid length accepted");
}
if((int)MessageType.Error != 15-1 || (int)MessageType.SelectFormation != 15) throw new Exception("Wire enum changed");
Console.WriteLine("Protocol roundtrip, framing, invalid lengths and enum compatibility passed.");
