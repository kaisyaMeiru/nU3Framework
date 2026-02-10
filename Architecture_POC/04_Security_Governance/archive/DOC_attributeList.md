# Framework Attribute List

이 문서는 프레임워크 소스 코드 분석을 통해 식별된 메타데이터(Attribute) 목록입니다.
주로 윈도우 폼(WinForms) 컨트롤의 디자인 타임 지원(Toolbox, Property Grid)을 위해 사용되었으며, 용도에 따라 **Toolbox/Design-Time**과 **Runtime/General**로 분류하였습니다.

---

## 1. Toolbox & Design-Time Attributes (디자인 타임 속성)

Visual Studio의 Form Designer, Toolbox, Property Grid에서 컨트롤이 어떻게 보이고 동작하는지를 제어하는 속성들입니다. 

| Attribute | 설명 및 사용 패턴 | 주요 예시 파일 |
| :--- | :--- | :--- |
| **`[ToolboxItem]`** | 도구 상자(Toolbox)에 컨트롤 표시 여부를 결정합니다.<br>- `true`: 도구 상자에 표시 (예: `XGrid`, `XButton`)<br>- `false`: 도구 상자에서 숨김 (내부용 컨트롤 또는 추상 클래스) | `XGrid.cs`, `XScreen.cs`, `BackContainer.cs` |
| **`[ToolboxBitmap]`** | 도구 상자에 표시될 아이콘(비트맵)을 지정합니다.<br>- 는 `Images` 폴더 내의 리소스를 연결하거나, 표준 컨트롤(`Panel`, `Label` 등)의 아이콘을 차용하여 사용합니다. | `XGrid.cs`, `XButton.cs`, `XPatientBox.cs` |
| **`[Designer]`** | 컨트롤의 디자인 타임 동작을 정의하는 별도 Designer 클래스를 연결합니다.<br>- 예: `XGrid`는 `XGridDesigner`를 통해 드래그 앤 드롭 컬럼 편집 등의 고급 기능을 제공합니다. | `XGrid.cs` (`XGridDesigner`), `XTaskBar.cs` |
| **`[Editor]`** | 속성 창(Property Grid)에서 특정 속성을 편집할 때 사용할 전용 UI 에디터를 지정합니다.<br>- 예: `XGrid`의 컬럼 편집기, 색상 선택기 등 팝업 형태의 에디터를 연결합니다. | `XGrid.cs` (`XGridCellEditor`), `FindColumnInfos.cs` |
| **`[Category]`** | 속성 창에서 속성을 그룹핑할 카테고리 이름을 지정합니다.<br>- 는 `"추가속성"`, `"출력정보"`, `"XGrid"`, `"Appearance"` 등의 커스텀 카테고리를 정의하여 속성을 체계적으로 관리합니다. | `XEditGrid.cs`, `XButtonList.cs`, `XColor.cs` |
| **`[Description]`** | 속성 창 하단에 표시되는 도움말(설명) 텍스트를 지정합니다.<br>- 한글로 상세한 설명을 제공하여 개발 가이드를 대신하고 있습니다. | `XScreen.cs` ("화면이 활성화될때 발생합니다."), `XGrid.cs` |
| **`[Browsable]`** | 속성 창에 특정 속성을 표시할지 여부를 제어합니다.<br>- `false`: 내부 로직용 속성이나 일반 개발자가 건드리면 안 되는 속성을 숨길 때 사용됩니다. | `XGrid.cs`, `XScreen.cs` |
| **`[DefaultValue]`** | 속성의 기본값을 지정합니다.<br>- 폼 디자이너가 `InitializeComponent`에 코드를 생성할지 결정하는 기준이 됩니다. (기본값과 같으면 코드 생성 생략) | `XColor.cs`, `XButtonList.cs` |
| **`[DesignerSerializationVisibility]`** | 속성 값이 코드(Resx 또는 CS)로 저장(직렬화)되는 방식을 제어합니다.<br>- `Content`: 하위 속성까지 저장 (예: 컬렉션)<br>- `Hidden`: 저장하지 않음 (런타임 전용 속성) | `XGrid.cs`, `XScreen.cs` |
| **`[TypeConverter]`** | 특정 타입의 객체를 문자열 등으로 변환하여 속성 창에 표시하거나 설정을 돕습니다.<br>- 예: `XColor`를 속성 창에서 쉽게 선택하도록 변환. | `XColor.cs`, `XCalendarMonth.cs` |

---

## 2. Runtime & General Attributes (런타임 및 일반 속성)

프로그램 실행 중(Runtime)의 동작이나 컴파일러, 통신 등과 관련된 속성들입니다.

| Attribute | 설명 및  사용 패턴 | 주요 예시 파일 |
| :--- | :--- | :--- |
| **`[OneWay]`** | (System.Runtime.Remoting.Messaging) 리턴 값이 없는 비동기 메서드 호출(Fire-and-forget)을 의미합니다.<br>- `XSerialWorker`에서 시리얼 통신이나 백그라운드 작업 시 사용된 것으로 추정됩니다. | `XSerialWorker.cs` |
| **`[Obsolete]`** | 더 이상 사용되지 않는 클래스나 멤버임을 표시합니다.<br>-  내부적으로 기능이 변경되었거나 지원 중단된 속성에 `"This property is not supported"` 메시지와 함께 적용되었습니다. | `XCalendar.cs` |
| **`[Serializable]`** | 클래스가 직렬화 가능함을 나타냅니다. (데이터 전송/저장 용)<br>- 주로 컬렉션 아이템 클래스(`FindColumnInfo`) 등에 적용되어 있습니다. | `FindColumnInfos.cs` |

---

## 3. 분석 요약 (Analysis Summary)

 프레임워크는 **WinForms의 `System.ComponentModel`** 속성을 매우 정교하게 사용하여 개발 생산성을 높이는 데 주력했습니다.

1.  **커스텀 디자이너 연결 (`Designer`, `Editor`)**: 단순 속성 설정이 아닌, `XGrid`와 같은 복잡한 컨트롤을 디자인 타임에 시각적으로 편집할 수 있도록 전용 에디터(`UITypeEditor`)를 다수 구현했습니다.
2.  **한글화된 메타데이터 (`Category`, `Description`)**: 속성 창의 카테고리와 설명을 한글화하여, 별도 매뉴얼 없이도 프레임워크 기능을 쉽게 파악할 수 있도록 했습니다.
3.  **코드 생성 최적화 (`DefaultValue`, `DesignerSerializationVisibility`)**: 불필요한 초기화 코드가 `InitializeComponent`에 쌓이는 것을 방지하여 폼 로딩 성능을 관리했습니다.

**nU 프레임워크로 마이그레이션 시 고려사항:**
*   `XGrid` 등의 복잡한 속성 설정 UI(`Editor` Attribute로 연결된 폼들)가 nU(DevExpress 등) 환경에서도 동일하게 필요한지, 혹은 DevExpress의 기본 Designer로 대체 가능한지 검토가 필요합니다.
*   기존에 `[Browsable(false)]`로 숨겨진 속성들이 비즈니스 로직에서 사용되고 있는지 확인해야 합니다.
