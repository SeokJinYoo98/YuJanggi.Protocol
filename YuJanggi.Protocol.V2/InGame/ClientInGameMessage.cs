using System;
using System.Collections.Generic;
using System.Text;
using YuJanggi.Protocol.V2.Matching;

namespace YuJanggi.Protocol.V2.InGame
{
    public sealed record GameSceneReadyRequest { };

    public sealed record MovePieceRequest
    {
        public ProtocolPlayerTeam Team { get; init; }

        public byte FromX { get; init; }
        public byte FromZ { get; init; }

        public byte ToX { get; init; }
        public byte ToZ { get; init; }
    }

}
