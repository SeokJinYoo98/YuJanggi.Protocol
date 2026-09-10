# YuJanggi.Protocol

Unity와 서버의 통신 계약을 관리하는 독립 저장소입니다. 기존 YuJanggiCommon 네임스페이스와 DLL 이름을 유지합니다. 서버 저장소에서 분리한 DTO와 길이 헤더 + JSON 직렬화 규약이 포함됩니다. UnityEngine, TCP 연결, 게임 규칙은 포함하지 않습니다.

## 빌드와 배포

.NET 10 SDK가 필요합니다.

```powershell
./Publish-Protocol.ps1 -ServerRoot D:\Git\YuJanggi.Server -UnityRoot D:\Git\YuJanggi.Unity
```

서버와 콘솔 클라이언트는 lib/YuJanggiCommon/YuJanggiCommon.dll(net10.0)을 참조합니다. Unity는 Assets/Plugins/YuJanggiCommon/YuJanggiCommon.dll(netstandard2.1)을 참조합니다. Unity의 기존 JSON 런타임 DLL은 유지하며 새 라이브러리의 참조 어셈블리 버전과 함께 검증해야 합니다.

배포 스크립트는 양쪽 DLL과 protocol-version.json을 함께 갱신합니다. 소스 커밋과 각 DLL SHA-256을 기록하며 미커밋 소스는 배포하지 않습니다. 각 소비 저장소에서 DLL과 버전 기록을 함께 커밋하세요. 프레이밍과 enum 숫자 값 변경은 양쪽 호환성을 함께 검토해야 합니다.

```powershell
dotnet run --project Tests/ProtocolChecks.csproj
```

원격 저장소가 연결되어 있습니다. 아래 CI/CD 설정을 참고하세요.

## 자동 배포

개발과 PR 검증은 dev에서 진행하고 **main push 또는 PR 병합 시 자동 배포**합니다. 수동 태그 push는 배포를 시작하지 않습니다.

버전 기준은 src/YuJanggiCommon/YuJanggiCommon.csproj의 Version (초기 1.0.0)입니다. 새 패키지가 필요하면 dev에서 MAJOR.MINOR.PATCH 버전을 올리고 main에 병합하세요. 이미 완료된 버전은 건너뜁니다.

자동 순서: 버전 확인 → 테스트·패키지 생성 → 소스 태그 v버전 및 draft Release 생성 → NuGet 게시 → UPM 전용 upm/v버전 태그 생성 → 첨부 파일 업로드 → Release 공개.
실패하면 draft 상태로 남으며 원래 커밋의 Actions 실행을 재실행합니다. 미완료 버전을 다른 커밋으로 재사용하면 중단합니다. 기존 버전 태그는 이동하지 않습니다.

GitHub Actions가 허용되어야 하며 GITHUB_TOKEN에 선언한 contents:write와 packages:write를 조직 정책이 허용해야 합니다. NuGet 소비 CI에는 패키지 읽기 권한을 부여하고 개발 PC의 PAT classic(read:packages)은 커밋하지 않습니다.

- NuGet 소스: https://nuget.pkg.github.com/SeokJinYoo98/index.json
- NuGet 패키지: YuJanggi.Protocol
- Unity Git URL: https://github.com/SeokJinYoo98/YuJanggi.Protocol.git#upm/v1.0.0 (실제 게시된 버전으로 변경)
- Release의 .tgz는 Unity Package Manager에서 tarball로 설치할 수 있습니다.

Unity UPM은 기존 JSON 런타임 DLL을 필요로 합니다. 구 YuJanggiCommon.dll과 UPM DLL을 중복 로드하지 마세요. packaging/README.md를 참고하세요.
서버·Unity의 참조 버전과 잠금 파일은 별도 변경으로 함께 검증합니다. 패키지 게시가 운영 서버나 게임 업데이트를 의미하지 않습니다.

로컬 검증: `./scripts/Build-Packages.ps1 -Version 0.0.0`. 같은 출력 폴더가 있으면 깨끗한 체크아웃에서 검증합니다. 로컬 생성 검증은 실제 GitHub 게시나 Unity Player 검증을 대신하지 않습니다.
