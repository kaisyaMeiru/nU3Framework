# POC 보고서 마스터 문서 (POC Reports Master Document)

**버전:** 1.0 (통합본)
**날짜:** 2026-02-10
**컨텍스트:** nU3.Framework (POC 상태, 검증, 로드맵)

---

## 1. POC 중간 보고서

### 1.1 개요
nU3.Framework는 대형 병원을 위한 차세대 EMR 플랫폼으로, 분리된 개발 환경과 24/7 운영을 위해 설계되었습니다.

### 1.2 핵심 아키텍처
*   **구조:** 5계층 아키텍처 (Bootstrapper -> Core -> Shell -> Modules -> Data).
*   **기술:** .NET 8, WinForms (DevExpress), SQLite (로컬), Oracle (서버).
*   **배포:** 핫 디플로이를 위한 섀도우 카피 전략.
*   **통신:** 느슨한 결합을 위한 이벤트 애그리게이터.

### 1.3 구현된 핵심 기능 (Phase 1)
*   ✅ **플러그인 아키텍처**: `AssemblyLoadContext` 기반 동적 로딩.
*   ✅ **메타데이터 발견**: `[nU3ProgramInfo]` 속성 스캔.
*   ✅ **기본 UI**: 라이프사이클 관리가 포함된 `BaseWorkControl`.
*   ✅ **연결성**: DB/File/Log용 HTTP 클라이언트.
*   ✅ **로컬 DB**: 설정/메타데이터용 SQLite 리포지토리.

### 1.4 대기 중 / 진행 중 (Phase 2+)
*   🚧 **보안**: JWT/RBAC 통합.
*   🚧 **서버**: ASP.NET Core API (Host).
*   ⏳ **외부**: HL7/FHIR, PACS, LIS 통합.

---

## 2. 검증 보고서: 팩토리 메서드 체인

### 2.1 범위
DevExpress 패턴에 대한 `nU3.Core.UI` 컨트롤 래퍼 검증.

### 2.2 결과
*   **점수:** 99/100 (우수).
*   **일관성:** 모든 컨트롤이 `InU3Control` 구현.
*   **패턴:** GridView/Columns에 대한 올바른 팩토리 메서드 사용.
*   **디자이너 지원:** `ViewInfo`/`Painter`와 함께 `EditorClassInfo`가 올바르게 등록됨.

---

## 3. 로드맵 (8개월)

### Phase 1: 기반 (완료됨)
*   코어 프레임워크, 쉘, 부트스트래퍼, 디플로이어.

### Phase 2: 보안 및 데이터 (현재)
*   JWT 인증, RBAC, Unit of Work, 마이그레이션 시스템.

### Phase 3: 의료 표준 (3-4개월 차)
*   HL7, FHIR, DICOM 통합.

### Phase 4: 통합 (5-6개월 차)
*   LIS, PACS, 보험 (EDI).

### Phase 5: 운영 (7-8개월 차)
*   Docker/K8s, CI/CD, 모니터링 (APM/ELK).

---

## 4. 결론
POC는 핵심 아키텍처 결정(플러그인, 섀도우 카피, 이벤트 버스)을 성공적으로 검증했습니다. 프레임워크는 Phase 2 개발을 위한 안정적인 상태이며, 보안 및 서버 측 로직에 집중하고 있습니다.