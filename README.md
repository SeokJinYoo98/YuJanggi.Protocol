# YuJanggi.Protocol

`YuJanggi.Unity`와 `YuJanggi.Server`가 공유하는 네트워크 메시지 계약 라이브러리입니다. 메시지 형식과 직렬화·패킷 경계 처리를 한 소스에서 관리하고, .NET용 NuGet과 Unity용 UPM 패키지로 제공합니다.

## Overview

- Client Request / Server Response / Server Event 계약 정의
- `RequestId` 기반 요청·응답 식별과 별도 Event 구분
- System.Text.Json 직렬화와 길이 기반 Packet Framing
- .NET 10 / .NET Standard 2.1 지원

## Architecture

Unity Client와 Server가 각각 Protocol을 참조합니다. 실제 TCP 연결·송수신, 요청 대기, 매칭 판단과 게임 상태 변경은 소비 프로젝트가 담당합니다.

메시지 → UTF-8 JSON → 4바이트 Big-Endian 길이 헤더 + 본문 → Client / Server Transport

Request는 ID를 생성하고 Response는 같은 ID를 유지합니다. Event와 단방향 알림에는 RequestId가 없습니다. Framing은 본문 크기를 1~4,096바이트로 제한합니다.

## Key Features

- **Connection**: Engine / Protocol 버전 Handshake 계약
- **Matching**: 매칭 시작·취소, 매치 정보 전달, 포진 제출과 GameReady
- **InGame**: 씬 준비·게임 시작, 이동과 게임 종료의 요청·응답·Event

MSTest는 메시지 직렬화·Framing 왕복, RequestId와 Payload 보존, 잘못된 패킷 크기 거부를 검증합니다. 서버 매칭 정책과 게임 규칙은 테스트 범위에 포함하지 않습니다.

## CI/CD

- **[CI](.github/workflows/ci.yml)**: `main` / `release/**` 대상 PR → Restore → Release Build → Test
- **[Release](.github/workflows/package-release.yml)**: `v*.*.*` Tag Push → Version 설정 → Restore → Build / Test → NuGet / UPM 패키징 → Artifact 업로드와 GitHub Release 생성
- **[CD](.github/workflows/cd.yml)**: 동일 Repository의 성공한 Release Push 실행 → 해당 Run ID의 NuGet Artifact 다운로드 → GitHub Packages Publish

CD는 Release에서 검증한 `.nupkg`를 그대로 사용하며 다시 빌드하지 않습니다. 인증은 `GITHUB_TOKEN`을 사용하고, 이미 존재하는 버전은 `--skip-duplicate`로 건너뜁니다.

[workflowConfig.json](workflowConfig.json)에서 프로젝트 경로, SDK와 Node.js 버전, Artifact 이름을 관리합니다. [SetVersion.ps1](scripts/SetVersion.ps1)은 Tag 버전을 `Version.cs`, `.csproj`, `upm/package.json`에 동일하게 적용합니다.

### 패키지 사용

- **NuGet**: `artifacts/nuget/*.nupkg` → `yujanggi-protocol-nuget` Artifact, GitHub Release 첨부, GitHub Packages 배포
- **UPM**: `artifacts/upm/*.tgz` → `yujanggi-protocol-upm` Artifact. 다운로드·압축 해제 후 Unity Package Manager의 **Install package from tarball**로 설치

UPM은 Registry나 GitHub Release Asset으로 배포하지 않습니다. [Prepare-Upm.ps1](scripts/Prepare-Upm.ps1)이 실제 Compile Item의 소스와 고정 GUID의 `.meta`를 생성하고, [BuildPackage.bat](scripts/BuildPackage.bat)이 NuGet과 UPM `.tgz`를 만듭니다.

버전 변경과 소스 생성은 Runner 내부에서 수행하며 저장소나 로컬 폴더에 자동 반영하지 않습니다. Unity 배포물은 Artifact의 `.tgz`이며, 소비 프로젝트에서 System.Text.Json 8.0.5와 관련 의존성을 제공해야 합니다.

## Project Structure

```text
src/                        # Connection·Matching·InGame 계약, 직렬화·Framing
upm/                        # Unity 패키지 설정과 Runtime 구성
scripts/                    # 버전 갱신과 NuGet / UPM 패키징
YuJanggi.Protocol.Tests/     # MSTest 계약 검증
.github/workflows/          # CI / Release / CD
workflowConfig.json         # Workflow 공통 설정
YuJanggi.Protocol.slnx       # 라이브러리와 테스트 솔루션
```

패키징 출력은 `artifacts/nuget`과 `artifacts/upm`에 생성합니다. 로컬에서는 `scripts/LocalPackage.bat`에 Target / Version을 입력해 버전 갱신과 패키징을 실행합니다.

## Related Projects

- [YuJanggi.Unity](https://github.com/SeokJinYoo98/YuJanggi.Unity) — 요청 전송과 서버 응답·Event를 처리하는 게임 Client
- [YuJanggi.Server](https://github.com/SeokJinYoo98/YuJanggi.Server) — 연결·매칭·게임 유스케이스를 처리하는 Server
- [YuJanggi.Engine](https://github.com/SeokJinYoo98/YuJanggi.Engine) — 장기 규칙과 게임 진행을 담당하는 라이브러리
