using BowenArrowsMod.Extensions;
using FrooxEngine;
using FrooxEngine.UIX;

namespace BowenArrowsMod.Helpers;

internal static class BAUIHelper
{
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

        if (BowenArrowsMod.DebugInfo_LinkedVars.Value)
        {
            var linkedVariableButton = builder.Button("Output Variable Definitions");
            linkedVariableButton.LocalPressed += (button, data) =>
            {
                onOutLinkedVars(outputfield.Value);
            };
        }
        if (BowenArrowsMod.DebugInfo_CompHierarchy.Value)
        {
            var componentHierarchyButton = builder.Button("Output Component Hierarchy");
            componentHierarchyButton.LocalPressed += (button, data) =>
            {
                onOutCompHierarchy(outputfield.Value);
            };
        }

        SyncMemberEditorBuilder.Build(outputfield.Value, "Output", outputfield.GetSyncMemberFieldInfo("Value"), builder);
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
        refEditor._targetRef.Target = backingField.Reference;

        button.Pressed.Target = refEditor.OpenInspectorButton;
        ui.Button("↑").Pressed.Target = refEditor.OpenWorkerInspectorButton;

        ui.PopStyle();

        return backingField;
    }
}
