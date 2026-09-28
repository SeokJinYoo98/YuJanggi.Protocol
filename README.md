# YuJanggi.Protocol

`YuJanggi.Unity`와 `YuJanggi.Server.V2`가 공유하는 네트워크 메시지 계약입니다. 매칭과 대국 메시지의 형식, JSON 직렬화 및 패킷 길이 처리를 제공합니다.

## 프로젝트 개요

- 클라이언트 요청·서버 응답·서버 이벤트 타입과 DTO
- 요청과 응답을 연결하는 `RequestId`
- .NET 라이브러리와 Unity Package Manager 패키지 구성

## 기술 스택

- C# 9
- .NET 10 / .NET Standard 2.1
- System.Text.Json
- Unity Package Manager
- MSTest

## 프로토콜 구조

```text
ClientMessage / ServerMessage
    └─ Type + RequestId + Payload
         ↓
MessageSerializer (JSON)
         ↓
MessageFramer (4바이트 Big-Endian 길이 + 본문)
```

`ClientMessageFactory`는 요청 ID를 생성합니다. `ServerMessageFactory`는 응답에 해당 ID를 유지하고, 서버 이벤트에는 `null`을 사용합니다.

## 주요 메시지

| 방향 | 메시지 |
| --- | --- |
| Client → Server | `HandshakeRequest`, `MatchingRequest`, `MatchingCancelRequest`, `FormationSubmit`, `GameSceneReadyRequest`, `MovePieceRequest` |
| Server → Client | `ProtocolHandshake`, `MatchingResponse`, `MatchingCancelResponse`, `MatchingFound`, `FormationSubmitResponse`, `GameReady`, `GameStartEvent`, `MovePieceEvent`, `Error` |

## 주요 DTO

| 구성 요소 | 역할 |
| --- | --- |
| [`ProtocolHandshakeRequest`](src/Connection/ProtocolHandshake.cs) | Protocol·Engine 버전 전달 |
| [`MatchingFound`](src/Matching/ServerMatchingMessages.cs) | 매치 ID, 자신의 진영, 상대 정보 전달 |
| [`FormationSubmitRequest`](src/Matching/ClientMatchingMessages.cs) | 선택한 포진 제출 |
| [`GameReadyEvent`](src/Matching/ServerMatchingMessages.cs) | 확정된 초·한 포진 전달 |
| [`MovePieceRequest`](src/InGame/ClientInGameMessage.cs) | 이동 진영과 출발·도착 좌표 전달 |
| [`MovePieceEvent`](src/InGame/ServerInGameMessage.cs) | 서버의 기물 이동 전달 |

## 사용 방법

.NET 프로젝트에서는 [`src/YuJanggi.Protocol.csproj`](src/YuJanggi.Protocol.csproj)을 참조합니다. Unity용 패키지 설정은 [`upm/package.json`](upm/package.json)에 있습니다. 전송할 때는 `MessageSerializer`로 JSON 바이트를 만들고 `MessageFramer`로 길이 헤더를 붙입니다. 수신할 때는 헤더에서 본문 길이를 확인한 뒤 역직렬화합니다.

## 테스트

[`YuJanggi.Protocol.Tests`](YuJanggi.Protocol.Tests)는 MSTest로 메시지 왕복과 패킷 길이 처리를 검사합니다.

## 관련 프로젝트

- `YuJanggi.Unity`: 게임 클라이언트
- `YuJanggi.Server.V2`: 게임 서버
- `YuJanggi.Engine`: 장기 규칙과 게임 진행

## 포트폴리오

[메시지 설계와 문제 해결 과정을 소개할 때, 포트폴리오 링크]
