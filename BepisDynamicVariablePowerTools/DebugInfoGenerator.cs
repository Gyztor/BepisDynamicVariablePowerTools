using Elements.Core;
using FrooxEngine;
using System.Text;

namespace BepisDynamicVariablePowerTools;

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
            if (!BepisDynamicVariablePowerTools.DebugInfo_OutputInPopoutUI.Value)
                return;

            Slot newText = space.LocalUserSpace.AddSlot("DynVarSpaceInfo");
            newText.PositionInFrontOfUser(faceDirection: float3.Backward, distance: 1f, preserveUp: true);
            UniversalImporter.SpawnText(newText, $"Hierarchy of linked dynamic variable components of Namespace {space.SpaceName} on {space.Slot.Name}", output, allowRTF: true);
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

            if (!BepisDynamicVariablePowerTools.DebugInfo_OutputInPopoutUI.Value)
                return;

            Slot newText = space.LocalUserSpace.AddSlot("DynVarSpaceInfo");
            newText.PositionInFrontOfUser(faceDirection: float3.Backward, distance: 1f, preserveUp: true);
            UniversalImporter.SpawnText(newText, $"Variables linked to Namespace [{space.SpaceName}] on {space.Slot.Name}", names.ToString(), allowRTF: true);
        });
    }
}
