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

현재는 로컬 Git 저장소이며 원격 저장소는 설정되지 않았습니다.
