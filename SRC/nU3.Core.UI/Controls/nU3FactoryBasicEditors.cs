using System;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace nU3.Core.UI.Controls
{
    // =================================================================================================
    // 1. nU3CheckEdit (CheckEdit Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3CheckEdit")]
    public class nU3RepositoryItemCheckEdit : RepositoryItemCheckEdit
    {
        static nU3RepositoryItemCheckEdit() { RegisternU3CheckEdit(); }
        public const string CustomEditName = "nU3CheckEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3CheckEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3CheckEditExtended), typeof(nU3RepositoryItemCheckEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo), new DevExpress.XtraEditors.Drawing.CheckEditPainter(), true));
        }
    }

    public class nU3CheckEditExtended : CheckEdit, InU3Control
    {
        static nU3CheckEditExtended() { nU3RepositoryItemCheckEdit.RegisternU3CheckEdit(); }
        public override string EditorTypeName => nU3RepositoryItemCheckEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemCheckEdit();

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


    // =================================================================================================
    // 2. nU3MemoEdit (MemoEdit Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3MemoEdit")]
    public class nU3RepositoryItemMemoEdit : RepositoryItemMemoEdit
    {
        static nU3RepositoryItemMemoEdit() { RegisternU3MemoEdit(); }
        public const string CustomEditName = "nU3MemoEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3MemoEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3MemoEditExtended), typeof(nU3RepositoryItemMemoEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.MemoEditViewInfo), new DevExpress.XtraEditors.Drawing.MemoEditPainter(), true));
        }
    }

    public class nU3MemoEditExtended : MemoEdit, InU3Control
    {
        static nU3MemoEditExtended() { nU3RepositoryItemMemoEdit.RegisternU3MemoEdit(); }
        public override string EditorTypeName => nU3RepositoryItemMemoEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemMemoEdit();

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }


    // =================================================================================================
    // 3. nU3ComboBoxEdit (ComboBoxEdit Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3ComboBoxEdit")]
    public class nU3RepositoryItemComboBoxEdit : RepositoryItemComboBox
    {
        static nU3RepositoryItemComboBoxEdit() { RegisternU3ComboBoxEdit(); }
        public const string CustomEditName = "nU3ComboBoxEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ComboBoxEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3ComboBoxEditExtended), typeof(nU3RepositoryItemComboBoxEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true));
        }
    }

    public class nU3ComboBoxEditExtended : ComboBoxEdit, InU3Control
    {
        static nU3ComboBoxEditExtended() { nU3RepositoryItemComboBoxEdit.RegisternU3ComboBoxEdit(); }
        public override string EditorTypeName => nU3RepositoryItemComboBoxEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemComboBoxEdit();

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }


    // =================================================================================================
    // 4. nU3SpinEdit (SpinEdit Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3SpinEdit")]
    public class nU3RepositoryItemSpinEdit : RepositoryItemSpinEdit
    {
        static nU3RepositoryItemSpinEdit() { RegisternU3SpinEdit(); }
        public const string CustomEditName = "nU3SpinEdit";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3SpinEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3SpinEditExtended), typeof(nU3RepositoryItemSpinEdit),
                typeof(DevExpress.XtraEditors.ViewInfo.SpinEditViewInfo), new DevExpress.XtraEditors.Drawing.SpinEditPainter(), true));
        }
    }

    public class nU3SpinEditExtended : SpinEdit, InU3Control
    {
        static nU3SpinEditExtended() { nU3RepositoryItemSpinEdit.RegisternU3SpinEdit(); }
        public override string EditorTypeName => nU3RepositoryItemSpinEdit.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemSpinEdit();

        public object? GetValue() => this.Value;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.Value = 0;
        public string GetControlId() => this.Name;
    }


    // =================================================================================================
    // 5. nU3RadioGroup (RadioGroup Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3RadioGroup")]
    public class nU3RepositoryItemRadioGroup : RepositoryItemRadioGroup
    {
        static nU3RepositoryItemRadioGroup() { RegisternU3RadioGroup(); }
        public const string CustomEditName = "nU3RadioGroup";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3RadioGroup()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3RadioGroupExtended), typeof(nU3RepositoryItemRadioGroup),
                typeof(DevExpress.XtraEditors.ViewInfo.RadioGroupViewInfo), new DevExpress.XtraEditors.Drawing.RadioGroupPainter(), true));
        }
    }

    public class nU3RadioGroupExtended : RadioGroup, InU3Control
    {
        static nU3RadioGroupExtended() { nU3RepositoryItemRadioGroup.RegisternU3RadioGroup(); }
        public override string EditorTypeName => nU3RepositoryItemRadioGroup.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemRadioGroup();

        public object? GetValue() => this.EditValue;
        public void SetValue(object? value) => this.EditValue = value;
        public void Clear() => this.EditValue = null;
        public string GetControlId() => this.Name;
    }


    // =================================================================================================
    // 6. nU3ToggleSwitch (ToggleSwitch Chain)
    // =================================================================================================

    [UserRepositoryItem("RegisternU3ToggleSwitch")]
    public class nU3RepositoryItemToggleSwitch : RepositoryItemToggleSwitch
    {
        static nU3RepositoryItemToggleSwitch() { RegisternU3ToggleSwitch(); }
        public const string CustomEditName = "nU3ToggleSwitch";
        public override string EditorTypeName => CustomEditName;

        public static void RegisternU3ToggleSwitch()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(
                CustomEditName, typeof(nU3ToggleSwitchExtended), typeof(nU3RepositoryItemToggleSwitch),
                typeof(DevExpress.XtraEditors.ViewInfo.ToggleSwitchViewInfo), new DevExpress.XtraEditors.Drawing.ToggleSwitchPainter(), true));
        }
    }

    public class nU3ToggleSwitchExtended : ToggleSwitch, InU3Control
    {
        static nU3ToggleSwitchExtended() { nU3RepositoryItemToggleSwitch.RegisternU3ToggleSwitch(); }
        public override string EditorTypeName => nU3RepositoryItemToggleSwitch.CustomEditName;

        protected override RepositoryItem CreateRepositoryItem() => new nU3RepositoryItemToggleSwitch();

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
}
