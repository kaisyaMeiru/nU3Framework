---
name: winform-skill
description: DevExpress 컨트롤을 직접 사용하지 않고, Wrapping 된 Custom Control(nU3Control)을 기반으로 WinForms 애플리케이션을 설계/구현합니다.
---

# WinForm Skill

이 스킬은 DevExpress 컨트롤을 직접 사용하지 않고, Wrapping 된 **nU3Control** 기반으로 WinForms 애플리케이션을 제작합니다.

## Policies Enforced
1. **nU3Control Only**: 모든 UI 컨트롤은 nU3Control을 사용합니다. DevExpress Native 컨트롤 사용 금지.
2. **Factory Chain Check**: 필요한 nU3Control이 Factory Chain에 없으면 새로 생성합니다.
3. **Design-Code Separation**: 디자인 코드와 비하인드 코드를 완전히 분리합니다.

## Instructions

1. **Do not use DevExpress Native Controls** — 모든 컨트롤은 nU3Control로 대체해야 합니다.
2. **Check Factory Chain**:
Factory에 없는 컨트롤은 즉시 nU3Control로 Wrapping 생성.
3. **Design Code Rules**:
- 디자인 코드와 비하인드 코드 완전 분리
- 람다식 사용 금지 (디자인 코드 내)
- 이벤트 핸들러는 비하인드 코드에서만 처리
4. **Fallback Method**:
예: 'GridControl' -> 'nU3GridControl'
: 프레임워크 표준 데이터 바인딩 인터페이스(InU3Control) 지원.
예: 'GridView' -> 'nU3GridView'
: 기본 그룹 패널 숨김 및 표준 행 선택 모드 자동 적용.
예: 'GridColumn' -> 'nU3GridColumn'
: 권한 ID(AuthId) 및 다국어 리소스 키(ResourceKey) 설정 기능 지원.

5. **Interpret Results**:
- nU3Control로 완전 변환 → 화면 모듈 완료
- Factory 누락 → 즉시 nU3Control 생성 및 등록

## When to Use
- **nU3 Framework 업무화면 개발 시 항상 사용**
- **일관된 디자인/컨트롤 통일성 확보**

## 모든 해당 인스턴스들을 파악하고
1) DevExpress Native 컨트롤을 사용 중인 주요 화면들을 nU3 표준 컨트롤(nU3Control)로 변환 완료해줘.
2) 아래와 같이 nU3~Control 표준 컨트롤로 변환에서 누락되거나 수정사항이 있나 확인해줘.
3) 혹시 Factory Chain에서 빠져서 DevExpress Native 그대로 사용하고 있는 것이 있나 확인해주고 있는 경우에는 Factory Chain을 구현해주고 nU3 Control로 변환해줘. 


이 스킬은 nU3 Framework의 화면모듈 개발에서 **디자인 통일성**과 **유지보수성**을 보장하기 위해 필수입니다.
