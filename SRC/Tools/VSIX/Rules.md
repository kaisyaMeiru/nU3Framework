요구사항 기준으로 “개발자가 신규 모듈(프로그램) 프로젝트를 만들 때 매번 반복하는 작업”을 자동화하는 nU3 Framework 전용 프로젝트 템플릿 생성기를 만들려는 거고, 프로젝트 생성 규칙은 다음으로 정리됩니다.
1) 사용자 입력(생성 시 받는 값)
•	시스템: {System} (예: BAS)
•	서브시스템: {SubSystem} (예: ZZ)
•	모듈 네임스페이스(모듈명): {ModuleNamespace} (예: zipcode)
•	프로그램명(표시명): {ProgramName} (예: 우편번호검색)
•	프로그램ID: {ProgramId} (예: ZIPCODE_SEARCH)
•	이 값은 “문자열 그대로”를 클래스/파일명으로도 사용(단, C# 식별자 규칙은 최소 검증 필요)
2) 생성되는 폴더 규칙(솔루션/SRC 루트 기준)
사용자가 “루트 폴더”를 선택하면 아래 경로를 생성:
Modules\{System}\{SubSystem}\nU3.Modules.{System}.{SubSystem}.{ModuleNamespace}\
예) Modules\BAS\ZZ\nU3.Modules.BAS.ZZ.zipcode\
3) 프로젝트명 / DLL명(AssemblyName) 규칙
•	프로젝트명(= csproj 파일명, AssemblyName):
•	nU3.Modules.{System}.{SubSystem}.{ModuleNamespace}
•	최종 DLL명:
•	nU3.Modules.{System}.{SubSystem}.{ModuleNamespace}.dll
즉 csproj 파일명도 보통:
•	nU3.Modules.{System}.{SubSystem}.{ModuleNamespace}.csproj
4) 기본 네임스페이스 규칙
•	메인 컨트롤 네임스페이스:
•	nU3.Modules.{System}.{SubSystem}.{ModuleNamespace}
•	BizLogic 네임스페이스:
•	nU3.Modules.{System}.{SubSystem}.{ModuleNamespace}.Logic
5) “메인 컨트롤” / “BizLogic” 파일 및 클래스 생성 규칙
•	메인 컨트롤 파일:
•	{ProgramId}.cs
•	메인 컨트롤 클래스명:
•	{ProgramId}
•	BizLogic 파일:
•	Logic\{ProgramId}BizLogic.cs
•	BizLogic 클래스명:
•	{ProgramId}BizLogic
6) nU3ProgramInfoAttribute 적용 규칙
메인 컨트롤 클래스에 아래 형태로 부여:
[nU3ProgramInfo(typeof({ProgramId}), "{ProgramName}", "{ProgramId}", "CHILD")]
※ nU3ProgramInfoAttribute는 declaringType.Namespace/AssemblyName로 System/SubSystem/DllName 등을 파싱하므로, 위 네임스페이스/프로젝트명/DLL명이 규칙대로 생성되는 게 핵심입니다.
---
결론: 개발자에게 제공하고 싶은 “애드온”의 역할
•	입력값 5개만 받으면
•	위 규칙에 맞는 폴더/프로젝트/기본 코드(컨트롤+BizLogic+csproj)를 자동 생성
•	그리고 필요하면 솔루션에 프로젝트를 자동 추가까지(선택)
원하면, 이걸 Visual Studio 확장으로 만들 때 접근은 보통 2가지로 정리됩니다:
1.	진짜 “프로젝트 템플릿(New Project)” 방식: 새 프로젝트 화면에서 템플릿 선택 → Wizard로 입력 받음
2.	VS 메뉴/커맨드 방식: “nU3 모듈 생성” 메뉴 클릭 → 입력받고 파일 생성 + 솔루션에 자동 Add
지금까지 만든 코드는 2)에 가까운 “파일 생성기(Wizard)” 방식입니다.



서명된 키 필요
c:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools>sn.exe -k C:\Project2_OPERATION\05.Framework\nU3.Framework\SRc\tools\VSIX\nU3.ZipModuleCreator.Vsix\Properties\nU3.snk