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
예: `GridControl` → `nU3GridControl`
5. **Interpret Results**:
- nU3Control로 완전 변환 → 화면 모듈 완료
- Factory 누락 → 즉시 nU3Control 생성 및 등록
## When to Use
- **nU3 Framework 업무화면 개발 시 항상 사용**
- **일관된 디자인/컨트롤 통일성 확보**

이 스킬은 nU3 Framework의 화면모듈 개발에서 **디자인 통일성**과 **유지보수성**을 보장하기 위해 필수입니다.
