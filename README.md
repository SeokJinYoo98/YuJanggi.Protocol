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

## CI/CD

GitHub 원격: https://github.com/SeokJinYoo98/YuJanggi.Protocol

- PR 및 main/master push: 계약 검사와 두 대상 프레임워크 NuGet·UPM 생성 검증.
- `v1.0.0` 형식의 태그 push: 검사 후 GitHub Packages에 NuGet 게시, 같은 저장소에 `upm/v1.0.0` 패키지 전용 태그 생성, GitHub Release에 .nupkg/.tgz 첨부.
- 정식 3자리 버전만 허용합니다. `v1.1`과 사전 릴리스 태그는 거부합니다.
- 배포 결과는 서버/Unity의 사용 버전을 자동으로 바꾸지 않습니다.

### GitHub 설정과 첫 실행

1. 이 변경을 GitHub 기본 브랜치에 커밋·push하고 Actions를 허용합니다.
2. CI가 통과한 커밋에 새 태그를 붙입니다.
3. `git tag -a v1.0.0 -m "Protocol 1.0.0"`, `git push origin v1.0.0`를 실행합니다.
4. Actions, Packages, Releases와 upm 태그를 확인합니다.

워크플로의 GITHUB_TOKEN에 packages:write, contents:write를 선언했습니다. 조직 정책이나 태그 규칙이 쓰기를 차단하면 해당 정책 설정이 필요합니다. 별도의 게시 PAT는 필요하지 않습니다.
소비 저장소의 CI에는 패키지 Actions 읽기 접근 권한을 부여합니다. 로컬 NuGet 인증에는 read:packages 권한의 PAT classic을 사용하고 비밀값은 커밋하지 않습니다.

### 서버와 Unity 설치

NuGet 소스: `https://nuget.pkg.github.com/SeokJinYoo98/index.json`
패키지 ID: `YuJanggi.Protocol` (어셈블리는 기존 YuJanggiCommon).
서버의 직접 DLL 참조를 `<PackageReference Include="YuJanggi.Protocol" Version="[1.0.0]" />`로 교체합니다.

Unity Git URL:
`https://github.com/SeokJinYoo98/YuJanggi.Protocol.git#upm/v1.0.0`

UPM은 netstandard2.1 DLL만 포함합니다. 기존 JSON 런타임 플러그인은 유지해야 하며, 구 YuJanggiCommon DLL과 meta는 중복되지 않도록 제거합니다. 자세한 의존성 조건은 packaging/README.md를 참고하세요. Core와 소비 프로젝트 참조는 이 변경에서 수정하지 않습니다.

### 로컬 확인

```powershell
./scripts/Build-Packages.ps1 -Version 0.0.0
```

계약 검사 후 artifacts/0.0.0 아래 NuGet과 UPM, tarball을 생성하고 두 프레임워크 DLL의 NuGet 포함 여부를 검사합니다. 같은 출력 폴더가 있으면 덮어쓰지 않고 중단하므로 깨끗한 체크아웃이나 다른 시험 버전을 사용하세요.

### 재실행 및 버전 복구

태그는 이동하지 않습니다. NuGet 중복 버전은 건너뛰고, 기존 UPM 태그는 원본 소스 커밋이 일치할 때만 재사용합니다. Release 첨부 파일은 동일 소스 태그의 재실행 시 갱신됩니다. 실패한 배포는 같은 소스로 재실행하며 코드 수정은 새 버전으로 배포합니다. 소비 측 문제는 이전에 검증된 버전과 잠금 파일로 되돌립니다.

이 설정 자체의 로컬 검증과 실제 GitHub 게시 성공은 별개입니다. 최초 원격 실행에서 패키지 권한과 Unity Player 동작을 확인해야 합니다.
