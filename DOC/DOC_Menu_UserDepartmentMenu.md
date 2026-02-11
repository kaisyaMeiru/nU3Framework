# 사용자별/부서별 메뉴 관리 시스템 구현 가이드

## 개요

nU3.Framework의 메뉴 시스템을 단일 글로벌 메뉴에서 **사용자별/부서별 커스터마이징 가능한 계층적 메뉴 시스템**으로 개선한 구현 가이드입니다.

## 시스템 아키텍처

### 메뉴 로딩 우선순위 (3-Tier Fallback)

```
1순위: SYS_USER_MENU    (사용자+부서별 커스텀 메뉴)
   ↓ 없으면
2순위: SYS_DEPT_MENU    (부서별 템플릿 메뉴)
   ↓ 없으면
3순위: SYS_MENU         (시스템 글로벌 템플릿)
```

### 데이터베이스 스키마

#### 1. SYS_DEPT (부서 마스터)
```sql
CREATE TABLE SYS_DEPT (
    DEPT_CODE VARCHAR(10) PRIMARY KEY,
    DEPT_NAME VARCHAR(100) NOT NULL,
    DEPT_NAME_ENG VARCHAR(100),
    DESCRIPTION TEXT,
    DISPLAY_ORDER INTEGER DEFAULT 0,
    PARENT_DEPT VARCHAR(10),
    IS_ACTIVE VARCHAR(1) DEFAULT 'Y',
    CREATED_DATE TEXT DEFAULT (datetime('now')),
    MODIFIED_DATE TEXT
);
```

**데이터 출처**: `nU3.Core.Enums.Department` (18개 의료 부서)

#### 2. SYS_USER_DEPT (사용자-부서 매핑)
```sql
CREATE TABLE SYS_USER_DEPT (
    USER_ID VARCHAR(50) NOT NULL,
    DEPT_CODE VARCHAR(10) NOT NULL,
    IS_PRIMARY VARCHAR(1) DEFAULT 'N',
    PRIMARY KEY (USER_ID, DEPT_CODE),
    FOREIGN KEY (USER_ID) REFERENCES SYS_USER(USER_ID),
    FOREIGN KEY (DEPT_CODE) REFERENCES SYS_DEPT(DEPT_CODE)
);
```

**관계**: N:M (한 사용자는 여러 부서에 속할 수 있음)

#### 3. SYS_DEPT_MENU (부서별 메뉴 템플릿)
```sql
CREATE TABLE SYS_DEPT_MENU (
    MENU_ID VARCHAR(50) NOT NULL,
    DEPT_CODE VARCHAR(10) NOT NULL,
    PARENT_ID VARCHAR(50),
    MENU_NAME VARCHAR(100) NOT NULL,
    PROG_ID VARCHAR(50),
    SORT_ORD INTEGER DEFAULT 0,
    AUTH_LEVEL INTEGER DEFAULT 1,
    PRIMARY KEY (MENU_ID, DEPT_CODE)
);
```

#### 4. SYS_USER_MENU (사용자별 커스텀 메뉴)
```sql
CREATE TABLE SYS_USER_MENU (
    MENU_ID VARCHAR(50) NOT NULL,
    USER_ID VARCHAR(50) NOT NULL,
    DEPT_CODE VARCHAR(10) NOT NULL,
    PARENT_ID VARCHAR(50),
    MENU_NAME VARCHAR(100) NOT NULL,
    PROG_ID VARCHAR(50),
    SORT_ORD INTEGER DEFAULT 0,
    AUTH_LEVEL INTEGER DEFAULT 1,
    PRIMARY KEY (MENU_ID, USER_ID, DEPT_CODE)
);
```

## 구현 세부사항

### 1. Repository Layer

#### IDepartmentRepository
- `GetAllDepartments()`: 모든 활성 부서 조회
- `GetDepartmentByCode(deptCode)`: 단일 부서 조회
- `GetDepartmentsByUserIdAsync(userId)`: 사용자가 접근 가능한 부서 목록

#### IMenuRepository (신규 메서드)
- `GetMenusByDeptCode(deptCode)`: 부서 템플릿 메뉴
- `GetMenusByUserAndDept(userId, deptCode)`: 사용자 커스텀 메뉴
- `DeleteMenusByDeptCode(deptCode)`: 부서 메뉴 삭제
- `DeleteMenusByUserAndDept(userId, deptCode)`: 사용자 메뉴 삭제
- `AddMenuForDept(deptCode, menu)`: 부서 메뉴 추가
- `AddMenuForUser(userId, deptCode, menu)`: 사용자 메뉴 추가

#### IUserRepository (신규 메서드)
- `GetAllUsersAsync()`: 전체 사용자 목록 (UI용)

### 2. UI 구조

#### MenuTreeManagementControl (4-Section Layout)

```
┌─────────────────────────────────────────┐
│  [사용자 목록]  │  [부서 목록]  │  [메뉴 트리]  │  [프로그램 목록]  │
│                │                │                │                    │
│  □ [user01]    │  □ 내과(1)     │  ├─ 내 즐겨찾기│  ■ [EMR_001]     │
│    홍길동      │  □ 외과(2)     │  │  ├─ 빠른 외래│    외래환자관리   │
│                │                │  │  └─ 빠른 처방│  ■ [EMR_002]     │
│  □ user02      │                │  └─ 내과 환자관리│    입원환자관리   │
│    김철수      │                │     └─ 외래 환자 │                    │
└─────────────────────────────────────────┘
```

**동작 흐름**:
1. 사용자 선택 → `GetDepartmentsByUserIdAsync()` 호출 → 부서 목록 표시
2. 부서 선택 → 3-tier fallback으로 메뉴 로드
3. 메뉴 편집 → "저장" 버튼 클릭 → `SYS_USER_MENU`에 저장

### 3. 메뉴 로딩 로직

```csharp
private void LoadMenuTree(string userId, string deptCode)
{
    List<MenuDto> allMenus;

    // 1순위: 사용자+부서 커스텀 메뉴
    var userMenus = _menuRepo.GetMenusByUserAndDept(userId, deptCode);
    if (userMenus.Count > 0)
    {
        allMenus = userMenus;
    }
    else
    {
        // 2순위: 부서 템플릿 메뉴
        var deptMenus = _menuRepo.GetMenusByDeptCode(deptCode);
        if (deptMenus.Count > 0)
        {
            allMenus = deptMenus;
        }
        else
        {
            // 3순위: 글로벌 템플릿
            allMenus = _menuRepo.GetAllMenus();
        }
    }

    AddTreeNodes(allMenus, null, tvMenu.Nodes);
    tvMenu.ExpandAll();
}
```

### 4. 메뉴 저장 로직

```csharp
private void SaveMenu()
{
    if (string.IsNullOrEmpty(_currentUserId) || string.IsNullOrEmpty(_currentDeptCode))
    {
        XtraMessageBox.Show("사용자와 부서를 먼저 선택해주세요.");
        return;
    }

    // 기존 사용자 커스텀 메뉴 삭제
    _menuRepo.DeleteMenusByUserAndDept(_currentUserId, _currentDeptCode);
    
    // 새 메뉴 저장
    SaveNodesRecursive(tvMenu.Nodes, null, _currentUserId, _currentDeptCode);
}
```

## 테스트 시나리오

### 시나리오 1: 커스텀 메뉴가 있는 사용자
- **사용자**: user01 (홍길동)
- **부서**: 내과(1)
- **예상 결과**: `SYS_USER_MENU`에서 user01+내과(1) 커스텀 메뉴 로드
- **확인**: "내 즐겨찾기" 폴더가 최상위에 표시됨

### 시나리오 2: 부서 템플릿만 있는 경우
- **사용자**: user01 (홍길동)
- **부서**: 외과(2)
- **예상 결과**: `SYS_DEPT_MENU`에서 외과(2) 템플릿 로드
- **확인**: "외과 수술관리" 폴더가 표시됨

### 시나리오 3: 템플릿이 없는 부서
- **사용자**: user03 (이영희)
- **부서**: 산부인과(4)
- **예상 결과**: `SYS_MENU`에서 글로벌 템플릿 로드
- **확인**: 모든 부서 공통 메뉴가 표시됨

### 시나리오 4: 여러 부서에 속한 사용자
- **사용자**: user03 (이영희)
- **예상 부서 목록**: 내과(1), 산부인과(4), 정형외과(5)
- **확인**: 부서 선택 시 각 부서별로 다른 메뉴 표시

## 데이터베이스 마이그레이션

### 자동 초기화 (완전 자동화)

`SqliteSchemaService.cs`에서 서버 시작 시 자동으로:

1. **테이블 생성**: `SYS_DEPT`, `SYS_USER_DEPT`, `SYS_DEPT_MENU`, `SYS_USER_MENU` + 인덱스
2. **부서 데이터 자동 삽입**: `Department` enum에서 18개 부서 sync
3. **테스트 데이터 자동 삽입** (DEBUG 모드):
   - 3명의 테스트 사용자 (user01, user02, user03)
   - 9개의 사용자-부서 매핑
   - 13개의 부서 메뉴 템플릿 (내과, 외과, 소아청소년과)
   - 5개의 사용자 커스텀 메뉴 (user01+내과)

**작동 방식**:
- `#if DEBUG` 조건부 컴파일로 테스트 데이터 자동 생성
- 기존 테스트 사용자가 있으면 스킵 (중복 방지)
- Release 빌드에서는 테스트 데이터 생성 안 됨

**데이터 초기화**:
```bash
# DB 파일 삭제하면 서버 재시작 시 자동 재생성
rm Server_Database/data.db
# 서버 시작 → 테이블 + 부서 + 테스트 데이터 자동 생성
```

## DI 등록

### nU3.Tools.Deployer/Program.cs
```csharp
services.AddScoped<IDepartmentRepository, SQLiteDepartmentRepository>();
services.AddScoped<IUserRepository, SQLiteUserRepository>();
services.AddScoped<IMenuRepository, SQLiteMenuRepository>();
```

## 주의사항

1. **데이터 무결성**: 사용자 삭제 시 `SYS_USER_MENU`, `SYS_USER_DEPT` 연쇄 삭제 필요
2. **부서 코드 타입**: VARCHAR(10)로 저장 (정수값 "1", "2", ...)
3. **메뉴 ID 충돌**: 각 테이블(`SYS_MENU`, `SYS_DEPT_MENU`, `SYS_USER_MENU`)에서 MENU_ID는 독립적
4. **성능**: 사용자가 많은 경우 `SYS_USER_MENU` 인덱스 확인 필요

## 향후 개선사항

1. **메뉴 복사 기능**: 부서 템플릿 → 사용자 커스텀으로 복사
2. **메뉴 공유**: 사용자 간 메뉴 설정 공유
3. **역할 기반 메뉴**: ROLE_CODE 기반 메뉴 필터링
4. **메뉴 변경 이력**: 감사(Audit) 테이블 추가
5. **UI 개선**: Drag&Drop으로 메뉴 순서 변경

## 파일 목록

### 신규 생성 파일
- `SRC/nU3.Models/DepartmentDto.cs`
- `SRC/nU3.Core/Repositories/IDepartmentRepository.cs`
- `SRC/nU3.Data/Repositories/SQLiteDepartmentRepository.cs`
- `DOC/DOC_Menu_UserDepartmentMenu.md` (본 문서)

### 수정된 파일
- `SRC/nU3.Core/Repositories/IMenuRepository.cs` (6개 메서드 추가)
- `SRC/nU3.Data/Repositories/SQLiteMenuRepository.cs` (6개 메서드 구현)
- `SRC/nU3.Core/Repositories/IUserRepository.cs` (GetAllUsersAsync 추가)
- `SRC/nU3.Data/Repositories/SQLiteUserRepository.cs` (GetAllUsersAsync 구현)
- `SRC/Servers/nU3.Server.Connectivity/Services/SqliteSchemaService.cs` (스키마 + 테스트 데이터 자동 생성)
- `SRC/nU3.Tools.Deployer/Views/MenuTreeManagementControl.Designer.cs` (4-Section UI)
- `SRC/nU3.Tools.Deployer/Views/MenuTreeManagementControl.cs` (비즈니스 로직)
- `SRC/nU3.Tools.Deployer/Program.cs` (DI 등록)

---

**작성일**: 2026-02-11  
**작성자**: Sisyphus (AI Agent)  
**버전**: 1.0
