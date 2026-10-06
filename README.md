# YuJanggi.Protocol

Unity Client와 .NET Server가 공유하는 네트워크 메시지 계약 라이브러리입니다. 같은 소스를 NuGet과 Unity UPM 패키지로 제공합니다.

## Highlights

- Request / Response / Event의 공용 메시지 계약
- `RequestId`로 요청과 응답 식별
- System.Text.Json 직렬화와 길이 기반 Packet Framing
- .NET 10 / .NET Standard 2.1 지원
- GitHub Actions로 검증·패키징·NuGet 배포 자동화

## Installation

### .NET — NuGet

GitHub Packages의 `YuJanggi.Protocol` 패키지를 참조합니다. NuGet 소스는 `https://nuget.pkg.github.com/SeokJinYoo98/index.json`이며, 접근 인증은 사용하는 환경에서 설정합니다.

`.nupkg`는 [GitHub Release](https://github.com/SeokJinYoo98/YuJanggi.Protocol/releases)에서도 다운로드할 수 있습니다.

### Unity — UPM

1. [Protocol Release 실행 화면](https://github.com/SeokJinYoo98/YuJanggi.Protocol/actions/workflows/package-release.yml)의 **Artifacts → `yujanggi-protocol-upm`**을 다운로드합니다.
2. 압축을 풀고 Unity Package Manager의 **Install package from tarball**에서 내부 `.tgz`를 선택합니다.
3. Unity 프로젝트에서 **System.Text.Json 8.0.5와 관련 의존성**을 제공합니다. 패키지의 Unity 기준 버전은 6000.0입니다.

UPM은 현재 Registry나 GitHub Release Asset으로 배포하지 않습니다. 생성된 Runtime 소스는 Git에서 제외되므로, 저장소의 `upm/` Git 경로를 직접 설치하는 방식은 사용할 수 없습니다.

## Features

- **Connection**: Engine / Protocol 버전 Handshake 계약
- **Matching**: 매칭 시작·취소, 매치 정보 전달, 포진 제출과 GameReady
- **InGame**: 씬 준비·게임 시작, 이동·게임 종료의 요청·응답·Event

## Architecture

Unity와 Server가 각각 Protocol을 참조합니다. Protocol은 메시지 형식과 직렬화·Framing을 담당하며, 실제 TCP 송수신과 매칭·게임 판단은 소비 프로젝트에 둡니다.

- **메시지 식별**: Request는 ID를 생성하고 Response는 같은 ID를 유지합니다. Event와 단방향 알림에는 RequestId가 없습니다.
- **전송 형식**: UTF-8 JSON 본문 앞에 4바이트 Big-Endian 길이 헤더를 붙입니다. 본문 크기는 1~4,096바이트로 제한합니다.

## CI/CD

- **[CI](.github/workflows/ci.yml)**: `main` / `release/**` 대상 PR에서 Restore → Release Build → Test
- **[Release](.github/workflows/package-release.yml)**: `v*.*.*` Tag Push에서 버전 설정·검증 후 NuGet / UPM Artifact 생성. GitHub Release에는 NuGet만 첨부
- **[CD](.github/workflows/cd.yml)**: 동일 Repository의 성공한 Release Push 실행에서 해당 Run ID의 NuGet Artifact를 받아 GitHub Packages에 Publish

CD는 다시 빌드하지 않으며 `GITHUB_TOKEN`으로 인증합니다. UPM Artifact는 CD에서 처리하지 않습니다.

[workflowConfig.json](workflowConfig.json)에서 프로젝트 경로·SDK·Artifact 이름을 관리합니다. [SetVersion.ps1](scripts/SetVersion.ps1)은 Tag 버전을 `Version.cs`, `.csproj`, `upm/package.json`에 함께 적용합니다. Runner에서 변경한 파일은 저장소나 로컬 폴더에 자동 반영하지 않습니다.

MSTest는 직렬화·Framing 왕복, RequestId와 Payload 보존, 잘못된 패킷 크기 거부를 검증합니다. 서버 정책과 게임 규칙은 테스트 범위에 포함하지 않습니다.

## Project Structure

```text
src/                        # 메시지 계약, 직렬화·Framing
upm/                        # Unity 패키지 설정과 Runtime 구성
scripts/                    # 버전 갱신과 NuGet / UPM 패키징
YuJanggi.Protocol.Tests/     # MSTest 계약 검증
.github/workflows/          # CI / Release / CD
```

로컬 패키징은 `scripts/LocalPackage.bat`에서 Target / Version을 입력해 실행합니다. 결과물은 `artifacts/nuget`과 `artifacts/upm`에 생성합니다.

## Related Projects

- [YuJanggi.Unity](https://github.com/SeokJinYoo98/YuJanggi.Unity) — 요청 전송과 서버 응답·Event를 처리하는 게임 Client
- [YuJanggi.Server](https://github.com/SeokJinYoo98/YuJanggi.Server) — 연결·매칭·게임 유스케이스를 처리하는 Server
- [YuJanggi.Engine](https://github.com/SeokJinYoo98/YuJanggi.Engine) — 장기 규칙과 게임 진행을 담당하는 라이브러리
