# YuJanggi.Protocol

Unity 클라이언트와 .NET 서버가 공유하는 유장기 네트워크 프로토콜 라이브러리입니다.
메시지 계약, JSON 직렬화, 길이 헤더를 사용하는 프레이밍을 제공하여 양쪽에서 같은 통신 형식을 사용하도록 합니다.

## 제공 기능

- 클라이언트 요청과 서버 응답·이벤트를 표현하는 메시지 모델
- 요청 ID 생성 및 응답 연결을 위한 메시지 팩토리
- 프로토콜·Core 버전 확인에 사용하는 핸드셰이크 DTO
- 매칭 신청·취소 요청과 응답, 매칭 완료 이벤트 DTO
- System.Text.Json 기반 UTF-8 직렬화와 payload 복원
- 4바이트 길이 헤더와 본문 크기 검증

TCP 연결 관리, 핸드셰이크 판정, 매칭 대기열과 게임 규칙은 소비자 측에서 구현합니다.

## 변경 이력

### 2026-09-22 — 매칭 프로토콜 추가

- `MatchingRequest` / `MatchingResponse`: 매칭 신청과 접수 결과. `Accepted`는 대기열 접수이며 매칭 완료가 아닙니다.
- `MatchingCancelRequest` / `MatchingCancelResponse`: 매칭 대기 취소와 처리 결과. 신청·취소 응답은 요청의 `RequestId`를 유지합니다.
- `MatchingFound`: 매칭 완료 이벤트(`RequestId = null`). `MatchId`와 초·한 플레이어의 ID·이름을 전달합니다.
- 매칭 계약만 제공하며 대기열 처리, 대국 시작과 초기 보드 구성은 소비자 측에서 구현합니다.

## 요구 환경

| 항목 | 설정 |
| --- | --- |
| 소스 빌드·테스트 | .NET SDK 10 |
| 라이브러리 타깃 | `net10.0`, `netstandard2.1` |
| 라이브러리 C# 버전 | 9.0 |
| NuGet 패키지 | `YuJanggi.Protocol` 0.2.0 |
| Unity 패키지 | `com.seokjinyoo.yujanggi.protocol.v2` 0.2.0, Unity 6 대상 |
| .NET Standard JSON 의존성 | System.Text.Json 8.0.5 |

Unity에서는 JSON 의존성을 별도로 제공해야 합니다. Unity Editor 및 IL2CPP 실행 검증은 아직 완료하지 않았습니다.

## 빠른 시작

### .NET — 로컬 NuGet 사용

이 저장소 루트에서 패키지를 생성합니다. 다음 절차는 nuget.org 공개 배포를 전제로 하지 않습니다.

```powershell
dotnet pack src/YuJanggi.Protocol.csproj -c Release -o artifacts/nuget
```

결과는 `artifacts/nuget/YuJanggi.Protocol.0.2.0.nupkg`입니다.
같은 저장소 루트에서 예제 콘솔 프로젝트를 만들 수 있습니다.

```powershell
dotnet new console -n ProtocolDemo -o artifacts/ProtocolDemo -f net10.0
dotnet add artifacts/ProtocolDemo/ProtocolDemo.csproj package YuJanggi.Protocol --version 0.2.0 --source ./artifacts/nuget
```

아래 최소 예제를 `artifacts/ProtocolDemo/Program.cs`에 넣고 실행합니다.

```powershell
dotnet run --project artifacts/ProtocolDemo/ProtocolDemo.csproj
```

기존 프로젝트에서는 프로젝트 경로와 `--source`를 실제 경로로 바꿉니다.
.NET Standard 소비자는 System.Text.Json 전이 의존성 복원을 위한 NuGet 소스도 필요합니다.

### Unity — 로컬 UPM 사용

1. Unity 6 프로젝트의 API Compatibility Level을 .NET Standard 2.1로 설정합니다.
2. 프로젝트에 System.Text.Json 8.0.5와 호환되는 전이 의존성을 제공합니다.
3. 이 저장소 루트에서 다음 명령을 실행합니다. PowerShell과 .NET SDK 10이 필요합니다.

```powershell
./scripts/Prepare-Upm.ps1
```

4. Unity Package Manager의 **Install package from disk**에서 `upm/package.json`을 선택합니다.
5. 사용자 asmdef에서 `YuJanggi.Protocol`를 참조합니다.

원본은 `src/`에만 작성합니다. 스크립트가 프로젝트의 Compile 항목을
`upm/Runtime/Generated/`에 복사하고 `.meta`를 생성하므로 원본 변경 후 다시 실행합니다.
생성물은 Git에서 제외되어 있어 `?path=/upm` Git URL 직접 설치는 지원하지 않습니다.
의존성 준비와 배포 방법은 [UPM 안내](upm/README.md)를 참고하세요.

## 최소 사용 예제

네트워크 없이 요청 생성부터 프레임 복원까지 왕복하는 콘솔 예제입니다.
Core 버전 `1.0.0`은 예제 입력값이며 실제 연동에서는 사용하는 Core 버전을 전달합니다.

```csharp
using System;
using YuJanggi.Protocol.V2;
using YuJanggi.Protocol.V2.Connection;
using YuJanggi.Protocol.V2.Framing;
using YuJanggi.Protocol.V2.Messages;
using YuJanggi.Protocol.V2.Messages.MessageFactory;
using YuJanggi.Protocol.V2.Serialization;

var request = new ProtocolHandshakeRequest
{
    YuJanggiProtocolVersion = ProtocolVersion.Current,
    YuJanggiCoreVersion = "1.0.0"
};
ClientMessage message = ClientMessageFactory.Create(
    ClientMessageType.ProtocolHandshake, request);
byte[] body = MessageSerializer.Serialize(message);
byte[] packet = MessageFramer.Encode(body);

int bodyLength = MessageFramer.DecodeBodyLength(
    packet.AsSpan(0, MessageFramer.HeaderSize));
ClientMessage received = MessageSerializer.Deserialize<ClientMessage>(
    packet.AsSpan(MessageFramer.HeaderSize, bodyLength));
ProtocolHandshakeRequest payload = received.GetPayload<ProtocolHandshakeRequest>();

Console.WriteLine(received.Type);
Console.WriteLine(payload.YuJanggiProtocolVersion);
Console.WriteLine(message.RequestId == received.RequestId);
```

예상 결과:

```text
ProtocolHandshake
0.1.0
True
```

서버 응답은 `ServerMessageFactory.CreateResponse(type, requestId, payload)`로 요청 ID를
그대로 전달합니다. 서버 이벤트는 `CreateEvent(type, payload)`로 만들며 `RequestId`가 null입니다.
팩토리는 null payload를 거부하고, `CreateResponse`는 비어 있는 요청 ID도 거부합니다.

## 통신 계약

| 항목 | 규칙 |
| --- | --- |
| 프레임 | 4바이트 헤더 + UTF-8 JSON 본문 |
| 헤더 | 본문의 바이트 수를 나타내는 big-endian Int32, 헤더 길이는 제외 |
| 본문 크기 | 1~4096바이트 |
| 메시지 공통 필드 | `Type`, `RequestId`, `Payload` |
| JSON 설정 | System.Text.Json 기본 설정: 속성 이름 유지, enum은 숫자 |
| 클라이언트 종류 | `ProtocolHandshake = 0`, `MatchingRequest = 1`, `MatchingCancelRequest = 2` |
| 서버 종류 | `ProtocolHandshake = 0`, `MatchingResponse = 1`, `MatchingCancelResponse = 2`, `MatchingFound = 3`, `Error = 100` |

TCP에서는 한 번의 읽기로 메시지 전체가 도착한다고 가정하지 않습니다. 소비자는 헤더
4바이트를 모두 읽고 길이를 검증한 다음 본문을 정확히 그 길이만큼 읽어 역직렬화해야 합니다.
`MessageFramer`는 소켓 읽기나 수신 버퍼 누적을 수행하지 않습니다.

`DecodeBodyLength`는 헤더가 4바이트가 아니면 `ArgumentException`, 본문 길이가 범위를
벗어나면 `InvalidDataException`을 발생시킵니다. `Encode`도 같은 본문 길이 제한을 적용합니다.
`GetPayload<T>()`는 payload가 없거나 복원 결과가 null이면 `InvalidDataException`을 발생시키며,
잘못된 JSON이나 타입 변환 오류는 System.Text.Json 예외가 전달될 수 있습니다.

핸드셰이크 결과는 플래그입니다. `Success = 0`, `ProtocolVersionMismatch = 1`,
`CoreVersionMismatch = 2`이며 두 불일치는 조합할 수 있습니다.
DTO는 결과를 표현할 뿐 버전 비교나 접속 차단을 직접 수행하지 않습니다.
`ProtocolVersion.Current`는 통신 버전이고 `.csproj`의 `Version`은 패키지 버전입니다.
현재 통신 버전은 `0.1.0`, 패키지 버전은 `0.2.0`입니다.

## 개발 및 검증

저장소 루트에서 실행합니다.

```powershell
dotnet build -c Release
dotnet test -c Release
dotnet pack src/YuJanggi.Protocol.csproj -c Release -o artifacts/nuget
```

라이브러리 산출물은 `src/bin/Release/` 아래 타깃별 디렉터리에 생성됩니다.
기존 테스트는 핸드셰이크 요청·응답 왕복 및 프레임 길이 검증을 다룹니다.
.NET 테스트 통과는 Unity 런타임 검증을 대체하지 않습니다.

```text
src/       # 계약·직렬화·프레이밍의 단일 원본
YuJanggi.Protocol.Tests/ # .NET 테스트
scripts/Prepare-Upm.ps1    # UPM 소스와 메타데이터 생성
upm/                      # Unity 패키지 템플릿과 설치 안내
```

## 제한과 관련 문서

- Unity UPM은 `.csproj`의 NuGet 의존성을 자동 복원하지 않습니다.
- IL2CPP/AOT와 stripping 환경에서는 실제 사용하는 DTO에 대한 보존 설정과 왕복 검증이 필요합니다.
- 프로토콜 DLL과 UPM 소스를 같은 Unity 프로젝트에 동시에 설치하지 않습니다.
- `Error` 메시지 종류는 정의되어 있지만 전용 오류 payload 계약은 아직 없습니다.

[Unity 설치 안내](upm/README.md) · [메시지 계약](src/Messages)
· [서버 V2](https://github.com/SeokJinYoo98/YuJanggi.Server.V2)
