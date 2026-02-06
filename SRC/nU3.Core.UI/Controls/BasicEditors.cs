using System;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace nU3.Core.UI.Controls
{
    #region 1. TextEdit

    [UserRepositoryItem("RegisternU3TextEdit")]
    public class nU3RepositoryItemTextEdit : RepositoryItemTextEdit
    {
        static nU3RepositoryItemTextEdit() { RegisternU3TextEdit(); }
        public const string CustomEditName = "nU3TextEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3TextEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3TextEdit), typeof(nU3RepositoryItemTextEdit),
                typeof(TextEditViewInfo), new TextEditPainter(), true));
        }

        //[Category("nU3 Framework")]
        //public bool IsRequired { get; set; } = false;
    }

    [ToolboxItem(true)]
    public class nU3TextEdit : TextEdit, InU3Control
    {
        static nU3TextEdit() { nU3RepositoryItemTextEdit.RegisternU3TextEdit(); }
        public nU3TextEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemTextEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;

        // Designer property used in generated code
        public bool IsRequired { get; set; }
    }

    #endregion

    #region 2. ButtonEdit

    [UserRepositoryItem("RegisternU3ButtonEdit")]
    public class nU3RepositoryItemButtonEdit : RepositoryItemButtonEdit
    {
        static nU3RepositoryItemButtonEdit() { RegisternU3ButtonEdit(); }
        public const string CustomEditName = "nU3ButtonEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ButtonEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3ButtonEdit), typeof(nU3RepositoryItemButtonEdit),
                typeof(ButtonEditViewInfo), new ButtonEditPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3ButtonEdit : ButtonEdit, InU3Control
    {
        static nU3ButtonEdit() { nU3RepositoryItemButtonEdit.RegisternU3ButtonEdit(); }
        public nU3ButtonEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemButtonEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 3. CheckEdit

    [UserRepositoryItem("RegisternU3CheckEdit")]
    public class nU3RepositoryItemCheckEdit : RepositoryItemCheckEdit
    {
        static nU3RepositoryItemCheckEdit() { RegisternU3CheckEdit(); }
        public const string CustomEditName = "nU3CheckEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3CheckEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3CheckEdit), typeof(nU3RepositoryItemCheckEdit),
                typeof(CheckEditViewInfo), new CheckEditPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3CheckEdit : CheckEdit, InU3Control
    {
        static nU3CheckEdit() { nU3RepositoryItemCheckEdit.RegisternU3CheckEdit(); }
        public nU3CheckEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemCheckEdit.CustomEditName;

        public object? GetValue() => this.Checked;
        public void SetValue(object? value)
        {
            if (value is bool b) this.Checked = b;
            else if (value != null) this.Checked = Convert.ToBoolean(value);
            else this.Checked = false;
        }
        public void Clear() => this.Checked = false;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 4. MemoEdit

    [UserRepositoryItem("RegisternU3MemoEdit")]
    public class nU3RepositoryItemMemoEdit : RepositoryItemMemoEdit
    {
        static nU3RepositoryItemMemoEdit() { RegisternU3MemoEdit(); }
        public const string CustomEditName = "nU3MemoEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3MemoEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3MemoEdit), typeof(nU3RepositoryItemMemoEdit),
                typeof(MemoEditViewInfo), new MemoEditPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3MemoEdit : MemoEdit, InU3Control
    {
        static nU3MemoEdit() { nU3RepositoryItemMemoEdit.RegisternU3MemoEdit(); }
        public nU3MemoEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemMemoEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;

        // Designer property
        public bool IsRequired { get; set; }
    }

    #endregion

    #region 5. ComboBoxEdit

    [UserRepositoryItem("RegisternU3ComboBoxEdit")]
    public class nU3RepositoryItemComboBoxEdit : RepositoryItemComboBox
    {
        static nU3RepositoryItemComboBoxEdit() { RegisternU3ComboBoxEdit(); }
        public const string CustomEditName = "nU3ComboBoxEdit";
        public override string EditorTypeName => CustomEditName;

    public static void RegisternU3ComboBoxEdit()
    {
        EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
            CustomEditName, typeof(nU3ComboBoxEdit), typeof(nU3RepositoryItemComboBoxEdit),
            typeof(ComboBoxViewInfo), new ButtonEditPainter(), true));
    }
    }

    [ToolboxItem(true)]
    public class nU3ComboBoxEdit : ComboBoxEdit, InU3Control
    {
        static nU3ComboBoxEdit() { nU3RepositoryItemComboBoxEdit.RegisternU3ComboBoxEdit(); }
        public nU3ComboBoxEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemComboBoxEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;

        // Designer property
        public bool IsRequired { get; set; }
    }

    #endregion

    #region 6. LookUpEdit

    [UserRepositoryItem("RegisternU3LookUpEdit")]
    public class nU3RepositoryItemLookUpEdit : RepositoryItemLookUpEdit
    {
        static nU3RepositoryItemLookUpEdit() { RegisternU3LookUpEdit(); }
        public const string CustomEditName = "nU3LookUpEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3LookUpEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3LookUpEdit), typeof(nU3RepositoryItemLookUpEdit),
                typeof(LookUpEditViewInfo), new LookUpEditPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3LookUpEdit : LookUpEdit, InU3Control
    {
        static nU3LookUpEdit() { nU3RepositoryItemLookUpEdit.RegisternU3LookUpEdit(); }
        public nU3LookUpEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemLookUpEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;

        // Designer property
        public bool IsRequired { get; set; }
    }

    #endregion

    #region 7. DateEdit

    [UserRepositoryItem("RegisternU3DateEdit")]
    public class nU3RepositoryItemDateEdit : RepositoryItemDateEdit
    {
        static nU3RepositoryItemDateEdit() { RegisternU3DateEdit(); }
        public const string CustomEditName = "nU3DateEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3DateEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3DateEdit), typeof(nU3RepositoryItemDateEdit),
                typeof(DateEditViewInfo), new ButtonEditPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3DateEdit : DateEdit, InU3Control
    {
        static nU3DateEdit() { nU3RepositoryItemDateEdit.RegisternU3DateEdit(); }
        public nU3DateEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemDateEdit.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 8. SpinEdit

    [UserRepositoryItem("RegisternU3SpinEdit")]
    public class nU3RepositoryItemSpinEdit : RepositoryItemSpinEdit
    {
        static nU3RepositoryItemSpinEdit() { RegisternU3SpinEdit(); }
        public const string CustomEditName = "nU3SpinEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3SpinEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3SpinEdit), typeof(nU3RepositoryItemSpinEdit),
                typeof(BaseSpinEditViewInfo), new ButtonEditPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3SpinEdit : SpinEdit, InU3Control
    {
        static nU3SpinEdit() { nU3RepositoryItemSpinEdit.RegisternU3SpinEdit(); }
        public nU3SpinEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemSpinEdit.CustomEditName;

        public object? GetValue() => this.Value;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.Value = 0;
        public string GetControlId() => this.Name;

        // Designer property
        public bool IsRequired { get; set; }
    }

    #endregion

    #region 9. RadioGroup

    [UserRepositoryItem("RegisternU3RadioGroup")]
    public class nU3RepositoryItemRadioGroup : RepositoryItemRadioGroup
    {
        static nU3RepositoryItemRadioGroup() { RegisternU3RadioGroup(); }
        public const string CustomEditName = "nU3RadioGroup";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3RadioGroup()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3RadioGroup), typeof(nU3RepositoryItemRadioGroup),
                typeof(RadioGroupViewInfo), new RadioGroupPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3RadioGroup : RadioGroup, InU3Control
    {
        static nU3RadioGroup() { nU3RepositoryItemRadioGroup.RegisternU3RadioGroup(); }
        public nU3RadioGroup() : base() { }
        public override string EditorTypeName => nU3RepositoryItemRadioGroup.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 10. ToggleSwitch

    [UserRepositoryItem("RegisternU3ToggleSwitch")]
    public class nU3RepositoryItemToggleSwitch : RepositoryItemToggleSwitch
    {
        static nU3RepositoryItemToggleSwitch() { RegisternU3ToggleSwitch(); }
        public const string CustomEditName = "nU3ToggleSwitch";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ToggleSwitch()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3ToggleSwitch), typeof(nU3RepositoryItemToggleSwitch),
                typeof(ToggleSwitchViewInfo), new ToggleSwitchPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3ToggleSwitch : ToggleSwitch, InU3Control
    {
        static nU3ToggleSwitch() { nU3RepositoryItemToggleSwitch.RegisternU3ToggleSwitch(); }
        public nU3ToggleSwitch() : base() { }
        public override string EditorTypeName => nU3RepositoryItemToggleSwitch.CustomEditName;

        public object? GetValue() => this.IsOn;
        public void SetValue(object? value)
        {
            if (value is bool b) this.IsOn = b;
            else if (value != null) this.IsOn = Convert.ToBoolean(value);
            else this.IsOn = false;
        }
        public void Clear() => this.IsOn = false;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 11. ImageComboBoxEdit

    [UserRepositoryItem("RegisternU3ImageComboBox")]
    public class nU3RepositoryItemImageComboBox : RepositoryItemImageComboBox
    {
        static nU3RepositoryItemImageComboBox() { RegisternU3ImageComboBox(); }
        public const string CustomEditName = "nU3ImageComboBox";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ImageComboBox()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3ImageComboBoxEdit), typeof(nU3RepositoryItemImageComboBox),
                typeof(ImageComboBoxEditViewInfo), new ButtonEditPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3ImageComboBoxEdit : ImageComboBoxEdit, InU3Control
    {
        static nU3ImageComboBoxEdit() { nU3RepositoryItemImageComboBox.RegisternU3ImageComboBox(); }
        public nU3ImageComboBoxEdit() : base() { }
        public override string EditorTypeName => nU3RepositoryItemImageComboBox.CustomEditName;

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public new void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 12. ProgressBarControl

    [UserRepositoryItem("RegisternU3ProgressBar")]
    public class nU3RepositoryItemProgressBar : RepositoryItemProgressBar
    {
        static nU3RepositoryItemProgressBar() { RegisternU3ProgressBar(); }
        public const string CustomEditName = "nU3ProgressBar";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ProgressBar()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3ProgressBarControl), typeof(nU3RepositoryItemProgressBar),
                typeof(ProgressBarViewInfo), new ProgressBarPainter(), true));
        }
    }

    [ToolboxItem(true)]
    public class nU3ProgressBarControl : ProgressBarControl, InU3Control
    {
        static nU3ProgressBarControl() { nU3RepositoryItemProgressBar.RegisternU3ProgressBar(); }
        public nU3ProgressBarControl() : base() { }
        public override string EditorTypeName => nU3RepositoryItemProgressBar.CustomEditName;

        public object? GetValue() => this.Position;
        public void SetValue(object? value) => this.Position = Convert.ToInt32(value);
        public new void Clear() => this.Position = 0;
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 13. SimpleButton

    [ToolboxItem(true)]
    public class nU3SimpleButton : SimpleButton, InU3Control
    {
        public nU3SimpleButton() : base() { }

        [Category("nU3 Framework")]
        [Description("버튼 권한 제어를 위한 고유 ID")]
        public string AuthId { get; set; } = string.Empty;

        public object? GetValue() => this.Text;
        public void SetValue(object? value) => this.Text = value?.ToString();
        public void Clear() { }
        public string GetControlId() => this.Name;
    }

    #endregion

    #region 14. LabelControl

    [ToolboxItem(true)]
    public class nU3LabelControl : LabelControl, InU3Control
    {
        public nU3LabelControl() : base() { }

        //[Category("nU3 Framework")]
        //[Description("필수 입력 표시 여부")]
        //public bool IsRequiredMarker { get; set; } = false;

        public object? GetValue() => this.Text;
        public void SetValue(object? value) => this.Text = value?.ToString();
        public void Clear() => this.Text = string.Empty;
        public string GetControlId() => this.Name;

        // Designer property
        public bool IsRequiredMarker { get; set; }
    }

    #endregion
}