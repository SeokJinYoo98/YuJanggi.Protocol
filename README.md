# YuJanggi.Protocol

YuJanggi 클라이언트와 서버가 공유하는 네트워크 메시지 및 DTO 패키지입니다.

## 프로젝트 개요

- 클라이언트 / 서버 공용 프로토콜 정의
- Request / Response 메시지 정의
- 서버 Event 메시지 정의
- 매치메이킹 DTO 정의
- 포진 선택 DTO 정의
- 게임 진행 DTO 정의
- NuGet / UPM 패키지 제공
- 동일 소스를 .NET과 Unity에서 공유

## 기술 스택

- C#
- .NET 10
- .NET Standard 2.1
- NuGet
- Unity Package Manager
- GitHub Actions

## 프로토콜 구조

    Client
        ↓
    ClientMessage
        ↓
    YuJanggi.Protocol
        ↓
    ServerMessage
        ↓
    Server

## 메시지 구조

    ClientMessage
    ├── Type
    ├── RequestId
    └── Payload

    ServerMessage
    ├── Type
    ├── RequestId
    └── Payload

## 주요 메시지

### Client → Server

- Handshake
- MatchingRequest
- MatchingCancelRequest
- ConfirmMatch
- FormationSubmit
- GameCommand

### Server → Client

- HandshakeResponse
- MatchingResponse
- MatchingFound
- FormationAccepted
- GameReady
- GameState
- Error

## 주요 DTO

| 구성 요소 | 역할 |
| --- | --- |
| `MatchingPlayer` | 매칭된 플레이어 정보 |
| `MatchingFound` | 매칭 결과 정보 |
| `FormationSubmit` | 선택한 포진 정보 |
| `GameReady` | 게임 시작에 필요한 초기 정보 |
| `GameCommand` | 게임 진행 명령 |
| `GameState` | 게임 상태 전달 |

## 패키지 구조

    YuJanggi.Protocol/
    ├── Messages/
    ├── Matching/
    ├── Game/
    ├── Common/
    └── upm/
        ├── package.json
        └── Runtime/

## NuGet 사용

    dotnet add package YuJanggi.Protocol

## Unity UPM 사용

    "com.seokjinyoo.yujanggi.protocol": "Git Repository URL"

## 관련 프로젝트

- [YuJanggi.Unity](링크)
- [YuJanggi.Server](링크)
- [YuJanggi.Engine](링크)

## 포트폴리오

- [YuJanggi 포트폴리오](노션 링크)