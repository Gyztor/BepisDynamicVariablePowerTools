using Elements.Core;
using FrooxEngine;
using System.Text;

namespace BowenArrowsMod;

internal sealed class DebugInfoGenerator
{
    internal static void OutputComponentHierarchy(DynamicVariableSpace space, Sync<string> target)
    {
        space.StartTask(async () =>
        {
            await default(ToBackground);

            var hierarchy = new SpaceTree(space);
            var output = hierarchy.Process() ? hierarchy.ToString() : "";

            await default(ToWorld);

            target.Value = output;
        });
    }

    internal static void OutputLinkedVariables(DynamicVariableSpace space, Sync<string> target)
    {
        space.StartTask(async () =>
        {
            await default(ToBackground);

            var names = new StringBuilder($"Variables linked to Namespace [{space.SpaceName}] on {space.Slot.Name}");
            names.AppendLine($"{space.SpaceName}:");

            foreach (var identity in space._dynamicValues.Keys)
            {
                names.AppendLine($"{identity.name} ({identity.type.GetNiceName()})");
            }

            names.Remove(names.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            await default(ToWorld);

            target.Value = names.ToString();
        });
    }
}
