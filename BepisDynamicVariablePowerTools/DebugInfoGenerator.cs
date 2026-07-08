using Elements.Core;
using FrooxEngine;
using System.Text;
using System.Reflection;

namespace BepisDynamicVariablePowerTools;

internal sealed class DebugInfoGenerator
{
    private static readonly FieldInfo _dynamicValueDict = typeof(DynamicVariableSpace).GetField("_dynamicValues", BindingFlags.Instance | BindingFlags.NonPublic);
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

            var dynamicValueKeys = _dynamicValueDict?.GetValue(space);
            foreach (var identity in (System.Collections.IEnumerable)dynamicValueKeys)
            {
                var key = identity.GetType().GetProperty("Key")!.GetValue(identity)!;

                var keyType = key.GetType();

                var name = (string)keyType.GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(key)!;
                var variableType = (Type)keyType.GetField("type", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(key)!;
                
                names.AppendLine($"{name} ({variableType.GetNiceName()})");
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
