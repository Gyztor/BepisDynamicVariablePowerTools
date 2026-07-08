using BepisDynamicVariablePowerTools.Extensions;
using FrooxEngine;
using FrooxEngine.UIX;
using System.Reflection;

namespace BepisDynamicVariablePowerTools.Helpers;

internal static class BAUIHelper
{
    private static readonly FieldInfo _refTarget = typeof(RefEditor).GetField("_targetRef", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo _openInspectorButton = typeof(RefEditor).GetMethod("OpenInspectorButon", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo _openWorkerInspectorButton = typeof(RefEditor).GetMethod("OpenWorkerInspectorButon", BindingFlags.Instance | BindingFlags.NonPublic);

    internal static void BuildRenameUI(this UIBuilder builder, IField<string> nameField, Action<string> onRename)
    {
        var layout = builder.HorizontalLayout(4).Slot.DestroyWhenLocalUserLeaves();
        var style = builder.Style;

        style.FlexibleWidth = 1;
        var newNameField = builder.TextField(nameField.Value, parseRTF: false);

        void ChangedListener(object _) => newNameField.Text.Content.Value = nameField.Value;
        nameField.Changed += ChangedListener;
        layout.Destroyed += _ => nameField.Changed -= ChangedListener;

        style.FlexibleWidth = -1;
        style.MinWidth = 256;
        var btn = builder.Button("Rename");
        btn.LocalPressed += (button, data) =>
        {
            onRename(newNameField.Text.Content.Value);
        };
        builder.NestOut();
    }

    internal static void BuildOutputUI(this UIBuilder builder, Action<Sync<string>> onOutLinkedVars, Action<Sync<string>> onOutCompHierarchy)
    {
        var outputfield = builder.Current.AttachComponent<ValueField<string>>();

        if (BepisDynamicVariablePowerTools.DebugInfo_LinkedVars.Value)
        {
            var linkedVariableButton = builder.Button("Output Variable Definitions");
            linkedVariableButton.LocalPressed += (button, data) =>
            {
                onOutLinkedVars(outputfield.Value);
            };
        }
        if (BepisDynamicVariablePowerTools.DebugInfo_CompHierarchy.Value)
        {
            var componentHierarchyButton = builder.Button("Output Component Hierarchy");
            componentHierarchyButton.LocalPressed += (button, data) =>
            {
                onOutCompHierarchy(outputfield.Value);
            };
        }
        if (!BepisDynamicVariablePowerTools.DebugInfo_OutputInPopoutUI.Value)
        {
            SyncMemberEditorBuilder.Build(outputfield.Value, "Output", outputfield.GetSyncMemberFieldInfo("Value"), builder);
        }
    }

    internal static ReferenceField<TReference> BuildOpenReference<TReference>(this UIBuilder ui, TReference reference)
            where TReference : class, IWorldElement
    {
        if (reference is null)
            throw new ArgumentNullException(nameof(reference));

        ui.PushStyle();
        ui.Style.FlexibleWidth = -1;
        ui.Style.MinWidth = 40;

        var button = ui.Button("⤴");

        var backingField = button.Slot.AttachComponent<ReferenceField<TReference>>();
        backingField.Reference.Target = reference;

        var refEditor = button.Slot.AttachComponent<RefEditor>();
        var targetValue = (RelayRef<ISyncRef>)_refTarget?.GetValue(refEditor);
        targetValue.Target = backingField.Reference;

        var InspectorAction = (ButtonEventHandler)Delegate.CreateDelegate(typeof(Action<IButton, ButtonEventData>), refEditor, _openInspectorButton);
        var WorkerInspectorAction = (ButtonEventHandler)Delegate.CreateDelegate(typeof(Action<IButton, ButtonEventData>), refEditor, _openWorkerInspectorButton);

        button.Pressed.Target = InspectorAction;
        ui.Button("↑").Pressed.Target = WorkerInspectorAction;

        ui.PopStyle();

        return backingField;
    }
}
