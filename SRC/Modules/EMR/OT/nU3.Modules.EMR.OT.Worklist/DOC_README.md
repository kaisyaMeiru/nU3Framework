# nU3.Modules.EMR.OT.Worklist - 외래수술 진료 워크리스트 모듈

## 개요

외래수술 진료 워크리스트 모듈은 환자 수술 진료 일정을 관리하고 추적하는 모듈입니다.

## 프로젝트 구조

```
nU3.Modules.EMR.OT.Worklist/
├── nU3.Modules.EMR.OT.Worklist.csproj
├── WorklistControl.cs           # 진료 일정 컨트롤
├── README.md                    # 이 파일
└── DOC_README.md
```

## 주요 기능

- **수술 진료 일정 관리**: 환자의 수술 예약 및 실제 진행 상황 추적
- **진료 상태 모니터링**: 대기, 수술 중, 수술 완료 등 상태별 관리
- **진료 기록 자동화**: 진료 완료 시 자동으로 상태 업데이트

## 사용 패턴

이 모듈은 nU3 프레임워크의 기본 패턴을 따릅니다:
- **BaseWorkControl 상속**: 모든 화면 컨트롤은 BaseWorkControl을 상속받습니다
- **EventBus 통신**: 모듈 간 이벤트 기반 통신 사용
- **MVVM 패턴**: 모듈 간 통신에는 EventBus를 사용

## 빌드 및 실행

```bash
# 프로젝트 빌드
dotnet build SRC/Modules/EMR/OT/nU3.Modules.EMR.OT.Worklist/nU3.Modules.EMR.OT.Worklist.csproj

# 전체 솔루션 빌드
dotnet build nU3.Framework.sln
```

## 참고 자료

- Framework 개요: DOC/DOC_개요.md
- 모듈 통신 패턴: DOC/DOC_고급_UI_및_connections.md
- 배포 도구: DOC/DOC_배포_도구_구현_및_종단간_검증.md

---

**작성일:** 2026-02-05
**버전:** 1.0
