# YuJanggi Protocol V2 — Unity 6

이 디렉터리는 UPM 패키지 템플릿입니다. 사용 전 저장소 루트에서
`./scripts/Prepare-Upm.ps1`을 실행하여 Runtime/Generated를 생성해야 합니다.
원본은 `src/`에만 있으며 생성된 C# 파일은 직접 수정하지 않습니다.

## 설치

1. Unity 6 프로젝트의 API Compatibility Level을 .NET Standard 2.1로 설정합니다.
2. 프로젝트에서 System.Text.Json **8.0.5**와 호환되는 전이 의존성을 제공합니다.
   NuGet을 Unity로 가져오는 도구 또는 검증된 managed plugin 배포를 사용해야 합니다.
   UPM은 .csproj의 PackageReference를 읽거나 NuGet 복원을 수행하지 않습니다.
3. 저장소에서 생성 스크립트를 실행한 뒤 Package Manager의
   **Install package from disk**에서 이 폴더의 `package.json`을 선택합니다.
4. 사용자 asmdef에서는 `YuJanggi.Protocol`를 참조합니다.
   JSON DLL은 Plugin Inspector에서 Auto Reference가 활성화되어 있어야 합니다.

현재 netstandard2.1 복원 그래프에는 System.Text.Encodings.Web 8.0.0,
Microsoft.Bcl.AsyncInterfaces 8.0.0, System.Runtime.CompilerServices.Unsafe 6.0.0이
추가됩니다. 가져오기 도구의 타깃 선택에 따라 추가 의존성이 생길 수 있으므로
선택한 타깃의 전체 의존성을 해결해야 합니다. Unity가 이미 제공하는 시스템 DLL을
중복 설치하지 말고, 기존 프로젝트의 JSON DLL과 버전을 통일해야 합니다.

## 배포 및 제한

- `Runtime/Generated`는 Git에서 제외됩니다. 이 저장소의 `?path=/upm` Git URL을
  직접 설치하면 소스가 없으므로 사용할 수 없습니다.
- 로컬 설치는 원본 변경 후 생성 스크립트를 다시 실행합니다. 배포 시에도 스크립트를
  실행하고 생성된 소스와 .meta를 포함한 **upm 폴더 전체**를 배포해야 합니다.
- 외부 JSON 의존성은 의도적으로 포함하지 않습니다. 완전한 자동 설치가 필요하면
  검증된 UPM JSON 래퍼 의존성을 선택하거나, 라이선스와 중복 DLL을 검토하여
  JSON DLL과 전이 의존성을 배포물에 포함하는 후속 작업이 필요합니다.
- Newtonsoft.Json 등으로의 교체는 public Payload의 JsonElement 타입과 직렬화 동작을
  바꾸므로 이 작업에서는 수행하지 않습니다.
- Unity Editor/Mono 및 IL2CPP 실행은 아직 검증하지 않았습니다. System.Text.Json의
  reflection 기반 직렬화와 임의의 generic payload는 IL2CPP/AOT 및 stripping 환경에서
  실제 사용 DTO에 대한 보존 설정과 왕복 테스트가 필요합니다.
- 동일 Unity 프로젝트에 기존 프로토콜 DLL과 이 소스 패키지를 함께 넣지 않습니다.

참고: [Unity C# 9 및 IsExternalInit](https://docs.unity3d.com/6000.0/Documentation/Manual/csharp-compiler.html),
[System.Text.Json 8.0.5 의존성](https://www.nuget.org/packages/System.Text.Json/8.0.5).
