---
name: comment-skill
description: 모든 코드 주석, 문서, 메시지를 한글로 작성하고 UTF-8 인코딩으로 저장하여 깨짐을 방지합니다.
---

# Comment Skill

코드, 문서 작성 시 모든 주석과 메시지를 **한글**로 작성하고 **UTF-8** 인코딩으로 저장합니다.

## Policies Enforced
1. **Korean Only**: 모든 주석, 문서, 메시지는 한글로 작성합니다.
2. **UTF-8 Encoding**: 파일은 반드시 UTF-8로 저장합니다.
3. **No English Comments**: 영문 주석 사용 금지 (한글 대체 필수).

## Instructions

1. **Do not write English comments** — 모든 주석은 한글로 작성합니다.
2. **Save as UTF-8**:
   ```csharp
   // Visual Studio: File → Advanced Save Options → UTF-8 with BOM
   // VS Code: 파일 하단 상태바 → UTF-8 클릭 → UTF-8 선택

3. **Comment Standards**:
// 한 줄 주석: 기능 설명
/* 블록 주석: 
 * 복잡한 로직 설명
 * 변경 이력 기록
 */

4. **Required Comment Locations**:

코드 완성 후 리뷰 전: 전체 기능 주석 작성

문서 작성 시: 섹션별 한글 설명 추가

복잡한 로직: 상세 동작 설명 필수

5. **File Check**:
UTF-8 확인: 파일 열었을 때 한글 깨짐 없음
한글 주석 100%: 영문 주석 0개

## When to Use
코드 완성 → 리뷰 전 (주석 필수)
문서 작성 (한글 설명 필수)
팀 공유 파일 (UTF-8 저장 필수)

## Example
csharp
// ✅ 올바른 예시 (UTF-8)
public void ProcessPatientData()
{
    // 환자 데이터 검증: 필수 항목 확인 후 처리
    if (!ValidatePatientInfo())
    {
        /* 오류 처리:
         * 1. 로그 기록
         * 2. 사용자 알림
         * 3. 기본값 설정
         */
        LogError("환자정보 불완전");
        return;
    }
}

이 스킬은 팀 협업에서 주석 가독성과 호환성을 보장하기 위해 필수입니다.