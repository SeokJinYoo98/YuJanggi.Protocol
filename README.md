# YuJanggi.Protocol

## Tech Stack

<p align="center">
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/csharp/csharp-original.svg" height="40" alt="C#" title="C#" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/dotnetcore/dotnetcore-original.svg" height="40" alt=".NET" title=".NET" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/nuget/nuget-original.svg" height="40" alt="NuGet" title="NuGet" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/unity/unity-original.svg" height="40" alt="Unity UPM" title="Unity UPM" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon@latest/icons/githubactions/githubactions-original.svg" height="40" alt="GitHub Actions" title="GitHub Actions" />
</p>

<p align="center">
  C# · .NET 10 / .NET Standard 2.1 · System.Text.Json · MSTest<br />
  Length-Prefixed JSON · CI/CD · GitHub Actions · NuGet / UPM / npm
</p>

## 라이브러리 소개

Unity 클라이언트와 .NET 서버가 공유하는 메시지 계약 라이브러리입니다.<br />
DTO·메시지 생성·JSON 직렬화·패킷 프레이밍을 한 소스에서 관리합니다.

실제 TCP 연결·송수신, 응답 대기와 게임 상태 변경은 소비 프로젝트가 담당합니다.

## 설치 방법

### .NET — NuGet

GitHub Packages 읽기 인증과 패키지 소스를 설정해 `YuJanggi.Protocol`을 참조합니다.

```bash
dotnet add package YuJanggi.Protocol --source https://nuget.pkg.github.com/SeokJinYoo98/index.json
```

### Unity — UPM

Project Settings → Package Manager에서 Scoped Registry를 등록합니다.

| 항목 | 값 |
| --- | --- |
| Name | YuJanggi |
| URL | `https://registry.npmjs.org` |
| Scope | `com.yujanggi` |

Package Manager의 My Registries에서 **YuJanggi Protocol**을 설치합니다.<br />
패키지 ID는 `com.yujanggi.protocol`이며 최소 Unity 버전은 **6000.0**입니다.

Unity 소비 프로젝트에서 **System.Text.Json 8.0.5와 관련 의존성**을 제공해야 합니다.<br />
[GitHub Releases](https://github.com/SeokJinYoo98/YuJanggi.Protocol/releases)의 `.nupkg` / `.tgz`로도 설치할 수 있습니다.

## 메시지 구조

`ClientMessage`와 `ServerMessage`가 공통으로 다음 필드를 사용합니다.

| 필드 | 역할 |
| --- | --- |
| `Type` | ClientMessageType / ServerMessageType으로 메시지 종류 식별 |
| `RequestId` | 요청·응답 연결에 사용하는 ID. Event와 단방향 메시지는 null |
| `Payload` | 메시지별 DTO를 담는 JsonElement |

`GetPayload<T>()`로 Payload를 해당 DTO로 역직렬화합니다.

| 영역 | 계약 범위 |
| --- | --- |
| Connection · 100번대 | Engine / Protocol 버전 Handshake |
| Matching · 200번대 | 매칭 시작·취소 · 매치 정보 · 포진 제출 · 게임 준비 |
| InGame · 300번대 | 씬 준비·시작 · 이동 · 종료 · 기권 · 한 수 쉼 · 리매치 |
| Chat · 400번대 | 채팅 요청·응답·수신 Event |

채팅 길이 제한은 공용 `ChatRules.MaxLen = 20`으로 정의합니다. 입력 검사와 전송 간격 제한은 Client / Server가 수행합니다.

## Request · Response · Event

| 구분 | 생성 API | RequestId |
| --- | --- | --- |
| Request | `ClientMessageFactory.CreateRequest` | 새로운 GUID 생성 |
| Response | `ServerMessageFactory.CreateResponse` | 원본 요청 ID 유지 |
| Event | `ServerMessageFactory.CreateEvent` | null |
| 단방향 Client 메시지 | `ClientMessageFactory.Create` | null |

Response는 개별 요청의 처리 결과, Event는 참가자에게 전달할 상태 변화를 나타냅니다.<br />
예를 들어 `RematchResponse`는 요청 접수 결과이며, `RematchConfirmedEvent`는 양측 동의 후 재대결 확정을 알립니다.

요청 ID별 응답 대기·타임아웃·취소는 Protocol이 아닌 소비 프로젝트의 책임입니다.

## 직렬화 · 패킷 프레이밍

메시지를 UTF-8 JSON으로 직렬화하고, 본문 바이트 길이를 4바이트 Big Endian 헤더에 기록합니다.

```text
[본문 길이: 4 bytes, Big Endian][본문: UTF-8 JSON]
```

- **MessageSerializer**: 메시지 객체와 UTF-8 JSON 바이트 간 변환
- **MessageFramer.Encode**: 길이 헤더와 본문을 결합
- **MessageFramer.DecodeBodyLength**: 헤더를 읽어 본문 길이 검증
- **본문 제한**: 1~4,096바이트. 빈 본문·상한 초과·잘못된 헤더 크기 거부

TCP 스트림에서 헤더와 본문을 필요한 길이만큼 읽는 처리는 Client / Server의 Transport가 담당합니다.

## 사용 예시

```csharp
using System;
using YuJanggi.Protocol.Chat;
using YuJanggi.Protocol.Framing;
using YuJanggi.Protocol.Messages;
using YuJanggi.Protocol.Serialization;

var request = ClientMessageFactory.CreateRequest(
    ClientMessageType.ChatRequest,
    new ChatRequest { Message = "안녕하세요." });

byte[] body = MessageSerializer.Serialize(request);
byte[] packet = MessageFramer.Encode(body);

// 완전한 패킷이 확보된 경우의 역변환 예시입니다.
int length = MessageFramer.DecodeBodyLength(
    packet.AsSpan(0, MessageFramer.HeaderSize));

var received = MessageSerializer.Deserialize<ClientMessage>(
    packet.AsSpan(MessageFramer.HeaderSize, length));

var chat = received.GetPayload<ChatRequest>();

var response = ServerMessageFactory.CreateResponse(
    ServerMessageType.ChatResponse,
    received.RequestId!,
    new ChatResponse { Result = ChatResult.Accepted });
```

실제 서버에서는 방 참가 여부·메시지 내용·전송 간격을 검증한 뒤 응답을 생성합니다.

## 테스트

[YuJanggi.Protocol.Tests](YuJanggi.Protocol.Tests)에 MSTest를 작성했습니다.

- 메시지 직렬화·역직렬화와 RequestId / Payload 보존
- 매칭 메시지 왕복
- 길이 헤더 인코딩·디코딩과 잘못된 패킷 크기 거부

```bash
dotnet test YuJanggi.Protocol.slnx --configuration Release
```

매칭 정책·게임 규칙·실제 TCP 연결 동작은 소비 프로젝트의 테스트 범위입니다.

## 버전 관리 · 패키지 배포

| 단계 | GitHub Actions 구성 |
| --- | --- |
| [CI](.github/workflows/ci.yml) | main / release 대상 PR에서 Restore → Release Build → MSTest |
| [Release](.github/workflows/package-release.yml) | 버전 Tag에서 검증 · NuGet / UPM 생성 · Artifact와 GitHub Release 제공 |
| [CD](.github/workflows/cd.yml) | 성공한 Release Run의 패키지를 GitHub Packages와 npm에 배포 |

CD는 Release Artifact를 다시 빌드하지 않고 사용합니다. NuGet은 GitHub Packages, UPM은 npm 레지스트리로 배포하며 npm에는 Trusted Publishing을 사용합니다.

[SetVersion.ps1](scripts/SetVersion.ps1)이 Tag 버전을 `Version.cs`, `.csproj`, `upm/package.json`에 함께 적용합니다.<br />
프로젝트 경로·도구 버전·Artifact 이름은 [workflowConfig.json](workflowConfig.json)에서 관리합니다.

[Prepare-Upm.ps1](scripts/Prepare-Upm.ps1)은 Compile Item의 소스와 고정 GUID의 `.meta`를 UPM Runtime으로 구성합니다.<br />
로컬 패키징은 `scripts/LocalPackage.bat`에서 Target / Version을 입력하며, 결과는 `artifacts/nuget`과 `artifacts/upm`에 생성합니다.

Runner의 버전 갱신·소스 생성은 저장소에 자동 커밋하지 않습니다. 계약 변경 시 Unity / Server의 메시지 호환성을 함께 확인합니다.

## 관련 저장소 · 문서

- [YuJanggi.Unity](https://github.com/SeokJinYoo98/YuJanggi.Unity) — 요청 전송·응답 대기·서버 Event의 게임 화면 반영
- [YuJanggi.Server](https://github.com/SeokJinYoo98/YuJanggi.Server) — AWS EC2 기반 TCP 서버·요청 검증·게임룸 메시지 전달
- [YuJanggi.Engine](https://github.com/SeokJinYoo98/YuJanggi.Engine) — 장기 규칙·게임 상태·기보 관리
