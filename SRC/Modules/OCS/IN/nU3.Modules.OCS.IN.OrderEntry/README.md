# nU3 OCS IN OrderEntry 모듈

## 개요

이 모듈은 기존 GMIS OCSINPOrderMain 프로젝트를 nU3 Framework로 마이그레이션한 OCS 입원 처방 모듈입니다.

## 주요 기능

1. **환자정보 조회**: 입원 환자의 기본 정보 조회
2. **처방코드 관리**: 약제, 주사, 검사 등 다양한 처방 코드 관리
3. **진단코드 관리**: 주/부 진단코드 등록 및 관리
4. **문제리스트 관리**: 간호문제 등 문제리스트 관리
5. **기타처방**: 검사, 방사선, 수술, 간호, 물리치료, 식이처방 등 기타 처방 관리
6. **전달메모**: 환자 전달 메모 작성 및 관리

## 프로젝트 구조

```
SRC/Modules/OCS/IN/nU3.Modules.OCS.IN.OrderEntry/
├── OCSINPOrderMainControl.cs               # 메인 컨트롤
├── OCSINPOrderMainControl.Designer.cs       # 메인 컨트롤 디자이너
├── nU3.Modules.OCS.IN.OrderEntry.csproj     # 프로젝트 파일
└── Controls/                                # 컴포넌트 컨트롤
    ├── PatientInfoControl.cs               # 환자정보 컨트롤
    ├── PatientInfoControl.Designer.cs       # 환자정보 컨트롤 디자이너
    ├── PatientListControl.cs               # 환자리스트 컨트롤
    ├── PatientListControl.Designer.cs       # 환자리스트 컨트롤 디자이너
    ├── OrderCodeControl.cs                 # 처방코드 컨트롤
    ├── OrderCodeControl.Designer.cs         # 처방코드 컨트롤 디자이너
    ├── DiagCodeControl.cs                  # 진단코드 컨트롤
    ├── DiagCodeControl.Designer.cs          # 진단코드 컨트롤 디자이너
    ├── ProblemListControl.cs               # 문제리스트 컨트롤
    ├── ProblemListControl.Designer.cs       # 문제리스트 컨트롤 디자이너
    ├── OtherOrderControl.cs                # 기타처방 컨트롤
    ├── OtherOrderControl.Designer.cs        # 기타처방 컨트롤 디자이너
    ├── SendMemoControl.cs                  # 전달메모 컨트롤
    ├── SendMemoControl.Designer.cs          # 전달메모 컨트롤 디자이너
    └── OtherTabControl.cs                  # 기타탭 컨트롤
        OtherTabControl.Designer.cs          # 기타탭 컨트롤 디자이너
└── Properties/
    └── Resources.Designer.cs              # 리소스 파일
```

## 마이그레이션 규칙

### 1. nU3 Framework 규칙 준수
- `BaseWorkControl` 상속
- nU3 프로그램 정보 특성 적용
- 이벤트 기반 아키텍처 사용
- Dependency Injection 지원

### 2. 컴포넌트화
- 재사용 가능한 컨트롤로 분리
- 각 컨트롤은 독립적인 기능 단위로 구현
- 디자이너 코드와 비즈니스 로직 분리

### 3. 화면 구성
- 기존 화면과 최대한 유사하게 구성
- DevExpress 컨트롤 사용 (nU3 컨트롤로 변환 가능)
- 윈폼 디자이너에서 깨지지 않도록 구현

### 4. 데이터 처리
- 데모 데이터 사용 (실제 DB 연동 없음)
- 비동기 처리 지원
- 트랜잭션 처리 구조 유지

## 사용 방법

1. **환자 선택**: 좌측 환자리스트에서 환자를 선택
2. **처방일자 설정**: 상단의 처방일자를 선택
3. **처방타입 선택**: 처방타입 콤보박스에서 처방 유형 선택
4. **진단코드 등록**: 진단코드 영역에서 진단 정보 입력
5. **문제리스트 등록**: 문제리스트 영역에서 간호문제 등록
6. **처방코드 등록**: 처방코드 영역에서 처방 내용 입력
7. **기타처방 입력**: 우측 기타처방 탭에서 추가 처방 입력
8. **전달메모 작성**: 하단 전달메모 영역에 메모 작성
9. **처방 저장/보류**: 하단 버튼에서 처방완료 또는 처방보류 선택

## 빌드 방법

```bash
# 전체 솔루션 빌드
dotnet build nU3.Framework.sln --configuration Release

# 특정 프로젝트 빌드
dotnet build SRC/Modules/OCS/IN/nU3.Modules.OCS.IN.OrderEntry/nU3.Modules.OCS.IN.OrderEntry.csproj --configuration Release
```

## 향후 개선 사항

1. **실제 DB 연동**: 데모 데이터 대신 실제 DB 연동
2. **nU3 컨트롤로 전환**: DevExpress 컨트롤을 nU3 래핑 컨트롤로 변경
3. **검증 로직 강화**: 입력값 검증 및 비즈니스 규칙 적용
4. **성능 최적화**: 대량 데이터 처리 시 성능 개선
5. **사용자 경험 개선**: UI/UX 개선 및 사용성 증대

## 참고 사항

- 이 모듈은 POC(Proof of Concept) 데모용으로 개발되었습니다.
- 실제 운영 환경에서 사용하기 위해서는 추가적인 테스트와 검증이 필요합니다.
- nU3 Framework의 다른 모듈과의 통합을 고려하여 개발되었습니다.