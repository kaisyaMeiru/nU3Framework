---
name: winform-skill
description: DevExpress 컨트롤을 직접 사용하지 않고, Wrapping 된 Custom Control을 기반으로 WinForms 애플리케이션을 설계/구현
---

# winform-skill

DevExpress 컨트롤을 직접 사용하지 않고, Wrapping 된 nU3Control 기반으로 WinForms 애플리케이션을 제작.
만약 빠진 Wrapping 컨트롤이 있는 경우, nU3Control 생성해서 제작. Factory Chain에서 빠질 수 있으니 점검.

## When to use this skill

- nU3 Framework에 화면모듈(업무화면)을 개발할 때는 항상 이 스킬을 사용.
- 일관된 디자인 및 컨트롤을 사용해서 통일성 갖춘 디자인 작성.


## How to use it

- 모든 디자인코드는 비하인드코드와 분리하고, 람다식을 넣지 않도록 한다.
- 디자인 코드 생성 후 DevExpress Native 컨트롤이 있나 확인한다.
- 일반적인 사용법은 DevExpress와 동일하므로 만약, nU3Control 사용법을 모르는 경우, DevExpress로 작성 후 nU3 접두어를 붙여서 해결하는 방법을 해본다.