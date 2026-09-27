using System;
using System.Collections.Generic;
using System.Text;

namespace YuJanggi.Protocol.Connection
{

    [Flags]
    public enum ProtocolHandshakeResult
    {
        Success                     = 0,
        ProtocolVersionMismatch     = 1 << 0,
        CoreVersionMismatch         = 1 << 1
    }
    public sealed record ProtocolHandshakeRequest
    {
        public string YuJanggiProtocolVersion { get; init; } = string.Empty;
        public string YuJanggiCoreVersion { get; init; } = string.Empty;
    }
    public sealed record ProtocolHandshakeResponse
    {
        public ProtocolHandshakeResult Result { get; init; }
            = ProtocolHandshakeResult.Success;
    }
}