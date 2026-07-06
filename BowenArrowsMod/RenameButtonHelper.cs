using FrooxEngine;
using FrooxEngine.UIX;

namespace BowenArrowsMod;
internal static class RenameButtonHelper
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
}
