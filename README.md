<h3 align="center">Tech Stack</h3>

<p align="center">
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/csharp/csharp-original.svg" height="40" alt="C#" title="C#" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/dotnetcore/dotnetcore-original.svg" height="40" alt=".NET" title=".NET" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/nuget/nuget-original.svg" height="40" alt="NuGet" title="NuGet" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/unity/unity-original.svg" height="40" alt="Unity UPM" title="Unity UPM" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/githubactions/githubactions-original.svg" height="40" alt="GitHub Actions" title="GitHub Actions" />
</p>

<p align="center">
  C# 9 / .NET 10 / .NET Standard 2.1 / System.Text.Json / MSTest<br />
  Length-Prefixed JSON / NuGet / UPM / npm / PowerShell / GitHub Actions
</p>

# YuJanggi.Protocol

Unity 클라이언트와 .NET 서버가 공유하는 메시지 계약 라이브러리입니다.
DTO / 메시지 생성 / JSON 직렬화 / 길이 헤더 프레이밍을 하나의 소스에서 관리합니다.

## Highlights

- **공용 메시지 계약**: 연결 / 매칭 / 대국 / 채팅 메시지와 결과 타입 정의
- **요청과 상태 변화 구분**: RequestId로 Request / Response를 연결하고 Event는 별도로 생성
- **턴 동기화 계약**: 턴 번호 / 다음 차례 / 마감 시각 / 한 수 쉼 사유 전달
- **두 가지 패키지 형식**: .NET용 NuGet과 Unity용 UPM 소스 패키지를 같은 버전으로 생성
- **배포 자동화**: Release 실행에서 만든 Artifact를 CD가 GitHub Packages / npm에 발행

## 설치

현재 버전은 **1.8.0**입니다.
[.NET 프로젝트](src/YuJanggi.Protocol.csproj)는 `net10.0` / `netstandard2.1`을 대상으로 하며, [UPM 패키지](upm/package.json)의 최소 Unity 버전은 **6000.0**입니다.

### .NET / NuGet

GitHub Packages 읽기 인증을 설정한 뒤 소비 프로젝트에서 설치합니다.

```bash
dotnet add package YuJanggi.Protocol --version 1.8.0 --source https://nuget.pkg.github.com/SeokJinYoo98/index.json
```

### Unity / UPM

Project Settings → Package Manager에서 Scoped Registry를 등록합니다.

- **Name**: `YuJanggi`
- **URL**: `https://registry.npmjs.org`
- **Scope**: `com.yujanggi`

Package Manager의 My Registries에서 **YuJanggi Protocol**을 설치합니다.
패키지 ID는 `com.yujanggi.protocol`입니다.

소비 프로젝트에서 **System.Text.Json 8.0.5와 관련 의존성**을 제공해야 합니다.
UPM은 .csproj의 NuGet 의존성을 자동 복원하지 않습니다.
수동 설치와 Unity 의존성 설정은 [UPM 안내](upm/README.md)를 참고합니다.

[GitHub Releases](https://github.com/SeokJinYoo98/YuJanggi.Protocol/releases)에서도 `.nupkg` / `.tgz`를 제공합니다.
Git 저장소의 `upm` 경로에는 생성 소스가 포함되지 않으므로, npm 또는 생성된 패키지를 사용합니다.

## 메시지 계약

[ClientMessage](src/Messages/ClientMessage.cs)와 [ServerMessage](src/Messages/ServerMessage.cs)는 다음 필드를 사용합니다.

- **`Type`**: ClientMessageType / ServerMessageType으로 메시지 종류 식별
- **`RequestId`**: 요청과 응답을 연결하는 문자열 ID. Event와 단방향 메시지는 null
- **`Payload`**: DTO를 담는 `JsonElement?`. `GetPayload<T>()`로 역직렬화

메시지 영역은 번호 대역으로 구분합니다.

- **[Connection / 100번대](src/Connection/ProtocolHandshake.cs)**: Engine / Protocol 버전 Handshake
- **[Matching / 200번대](src/Matching/ServerMatchingMessages.cs)**: 매칭 시작과 취소 / 상대 정보 / 포진 제출 결과 / 게임 준비
- **[InGame / 300번대](src/InGame/ServerInGameMessage.cs)**: 씬 준비 / 대국 시작 / 이동 / 턴 완료 / 종료 / 기권 / 한 수 쉼 / 리매치
- **[Chat / 400번대](src/Chat/ServerChatMessage.cs)**: 채팅 요청 / 응답 / 수신 이벤트. 공용 길이 상한은 `ChatRules.MaxLen = 20`

### Request / Response / Event

- **Request**: `ClientMessageFactory.CreateRequest`가 새 GUID를 RequestId로 생성합니다.
- **Response**: `ServerMessageFactory.CreateResponse`가 원본 요청의 RequestId를 유지합니다.
- **Event**: `ServerMessageFactory.CreateEvent`는 RequestId 없이 상태 변화를 전달합니다.
- **단방향 Client 메시지**: `ClientMessageFactory.Create`를 사용합니다. 게임 씬 준비 메시지 `GameSceneReady`가 해당합니다.

예를 들어 `RematchResponse`는 개별 요청의 접수 결과이고, `RematchConfirmedEvent`는 양측 동의 후의 재대결 확정입니다.
요청 접수와 실제 대국 시작은 구분하며, 재대결 시작에도 `GameSceneReady / GameStartEvent`를 사용합니다.

### 대국 턴 정보

[클라이언트 요청 DTO](src/InGame/ClientInGameMessage.cs)와 [서버 이벤트 DTO](src/InGame/ServerInGameMessage.cs)에 턴 번호와 시간 정보를 정의합니다.

- **`GameStartEvent`**: 첫 턴 번호 / 현재 진영 / 턴 제한시간 / 마감 시각. 재대결도 턴 번호 1부터 시작
- **`MovePieceRequest` / `PassTurnRequest`**: 요청할 TurnNumber 포함. 기권 / 리매치에서는 공용 GamePlayerRequest의 TurnNumber를 사용하지 않음
- **`TurnCompletedEvent`**: 완료된 턴 번호 / Move 또는 Pass / 행동 진영 / 다음 진영 / 다음 마감 시각 전달
- **`ProtocolPassReason`**: 사용자의 한 수 쉼은 UserRequest, 시간 초과는 Timeout으로 구분
- **종료 턴 / 결과**: 종료 턴은 NextTeam=None / NextTurnDeadlineAt=null. 승자 / 종료 사유 / 총 수순은 `GameEndedEvent`로 전달

대국이 계속되면 다음 요청에는 완료된 TurnNumber + 1을 사용합니다.
이전 턴 요청의 거절 결과는 `StaleTurn`으로 정의하며, 실제 판정과 입력 제어는 Server / Unity가 수행합니다.

## 직렬화 / 프레이밍

메시지를 UTF-8 JSON으로 직렬화하고 본문 바이트 길이를 4바이트 Big Endian 헤더에 기록합니다.

```text
[본문 길이: 4 bytes, Big Endian][본문: UTF-8 JSON]
```

- **[MessageSerializer](src/Serialization/MessageSerializer.cs)**: 메시지 객체와 JSON 바이트 간 변환
- **[MessageFramer.Encode](src/Framing/MessageFramer.cs)**: 길이 헤더와 본문 결합
- **`MessageFramer.DecodeBodyLength`**: 헤더 크기와 본문 길이 검사
- **본문 크기**: 1~4,096바이트. 빈 본문 / 상한 초과 / 잘못된 헤더 크기는 거부

TCP 연결과 스트림 읽기 / 쓰기는 소비 프로젝트의 Transport가 담당합니다.
헤더를 먼저 읽고 길이를 확인한 뒤 본문을 확보해야 하며, JSON 역직렬화에는 헤더를 제외한 본문을 전달합니다.

### 사용 예시

```csharp
using YuJanggi.Protocol.Chat;
using YuJanggi.Protocol.Framing;
using YuJanggi.Protocol.Messages;
using YuJanggi.Protocol.Serialization;

var request = ClientMessageFactory.CreateRequest(
    ClientMessageType.ChatRequest,
    new ChatRequest { Message = "안녕하세요." });

byte[] body = MessageSerializer.Serialize(request);
byte[] packet = MessageFramer.Encode(body);

// 수신 측에서 헤더를 제외한 본문을 확보한 뒤 수행하는 처리입니다.
var received = MessageSerializer.Deserialize<ClientMessage>(body);
var chat = received.GetPayload<ChatRequest>();

var response = ServerMessageFactory.CreateResponse(
    ServerMessageType.ChatResponse,
    received.RequestId!,
    new ChatResponse { Result = ChatResult.Accepted });
```

Protocol은 메시지 형식을 제공합니다.
방 참가 여부 / 이동 합법성 / 채팅 입력과 전송 간격 / 응답 대기와 타임아웃은 소비 프로젝트에서 처리합니다.

## CI/CD

- **[CI](.github/workflows/ci.yml)**: `main` / `release/**` 대상 PR에서 Restore → Release Build → MSTest
- **[Release](.github/workflows/package-release.yml)**: `v*.*.*` 태그에서 버전 적용 → 빌드와 테스트 → NuGet / UPM 패키징 → Artifact 업로드 및 GitHub Release 생성
- **[CD](.github/workflows/cd.yml)**: 성공한 Protocol Release 실행의 `.nupkg` / `.tgz`를 받아 GitHub Packages / npm에 발행

CD는 Release의 정확한 커밋과 같은 실행의 Artifact를 사용하며, 패키지를 다시 빌드하지 않습니다.
npm 발행은 Trusted Publishing을 사용하며 해당 저장소와 Workflow를 npm에 Trusted Publisher로 등록해야 합니다.

### 스크립트 역할

- **[SetVersion.ps1](scripts/SetVersion.ps1)**: Version.cs / .csproj / upm/package.json의 버전을 통일
- **[Prepare-Upm.ps1](scripts/Prepare-Upm.ps1)**: 실제 Compile 소스를 Runtime/Generated로 복사하고 경로 기반의 고정 GUID로 .meta 생성
- **[BuildPackage.bat](scripts/BuildPackage.bat)**: dotnet pack으로 `.nupkg` 생성 → Prepare-Upm 실행 → npm pack으로 `.tgz` 생성
- **[Publish-Upm.ps1](scripts/Publish-Upm.ps1)**: 생성된 `.tgz`를 npm에 발행. 정식 버전은 latest, 사전 배포 버전은 next 태그 사용
- **[LocalPackage.bat](scripts/LocalPackage.bat)**: Target / Version을 입력받아 SetVersion과 BuildPackage를 같은 순서로 실행
- **[Read-WorkflowConfig.ps1](scripts/Read-WorkflowConfig.ps1)**: [workflowConfig.json](workflowConfig.json)의 대상 / 솔루션 경로 / 도구 버전 / Artifact 이름을 Workflow에 전달

로컬 패키징에는 .NET 10 SDK / Node.js와 npm / PowerShell이 필요합니다.
Windows에서 `scripts/LocalPackage.bat`을 실행하고 Target에는 `Protocol`, Version에는 원하는 버전을 입력합니다.
결과는 `artifacts/nuget` / `artifacts/upm`에 생성되며, 로컬 실행 시 버전 파일도 변경됩니다.

UPM 생성 소스는 직접 수정하지 않고 `src/`를 원본으로 유지합니다.
Release Runner에서 적용한 버전과 생성 소스는 저장소에 자동 커밋하지 않습니다.

## 테스트 / 호환성

[YuJanggi.Protocol.Tests](YuJanggi.Protocol.Tests)에 다음 MSTest를 작성했습니다.

- **메시지 왕복**: Handshake / 단방향 준비 메시지 / RequestId와 Payload 보존
- **매칭 계약**: 요청 결과 / 참가자 정보 / 포진의 전송 값 보존
- **대국 계약**: 요청 턴 번호 / 서버 시작 시각과 마감 / 한 수 쉼 사유 / 종료 턴의 nullable 정보 보존
- **프레이밍**: 헤더와 본문 왕복 / 빈 본문 / 크기 상한 초과 / 잘못된 헤더 크기 거부

```bash
dotnet test YuJanggi.Protocol.slnx --configuration Release
```

게임 규칙과 실제 TCP 연결 동작은 소비 프로젝트의 검증 범위입니다.
계약을 변경할 때는 Unity / Server의 DTO와 메시지 번호 호환성, Handshake에 사용하는 버전을 함께 확인합니다.

## 관련 프로젝트

- [YuJanggi.Unity](https://github.com/SeokJinYoo98/YuJanggi.Unity) — 요청 전송 / 응답 대기 / 서버 이벤트 검증과 화면 반영
- [YuJanggi.Server](https://github.com/SeokJinYoo98/YuJanggi.Server) — 요청 검증 / 서버 권위 대국 / 매칭과 방 관리
- [YuJanggi.Engine](https://github.com/SeokJinYoo98/YuJanggi.Engine) — 장기 규칙 / 대국 상태 / 턴 처리 / 기보
