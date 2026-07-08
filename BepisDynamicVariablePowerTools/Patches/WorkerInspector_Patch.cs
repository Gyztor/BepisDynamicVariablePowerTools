using BepisDynamicVariablePowerTools.Extensions;
using BepisDynamicVariablePowerTools.Helpers;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;

namespace BepisDynamicVariablePowerTools.Patches;

[HarmonyPatch]
public class WorkerInspector_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorkerInspector), "BuildInspectorUI")]
    public static void BuildInspectorUI_Postfix(Worker worker, UIBuilder ui)
    {
        if (!BepisDynamicVariablePowerTools.Mod_Enabled.Value)
            return;

        switch (worker)
        {
            case DynamicVariableSpace space:
                // Add custom UI elements for DynamicVariableSpace
                //ui.Style.MinHeight = 24;
                ui.BuildRenameUI(space.SpaceName, onRename: newName => RenameDirectlyLinkedVariables.RenameSpace(space, newName));
                if (BepisDynamicVariablePowerTools.DebugInfo_Enabled.Value)
                {
                    ui.BuildOutputUI(onOutLinkedVars: target => DebugInfoGenerator.OutputLinkedVariables(space, target),
                        onOutCompHierarchy: target => DebugInfoGenerator.OutputComponentHierarchy(space, target));
                }
                break;
            case IDynamicVariable dynamicVariable:
                // Add custom UI elements for IDynamicVariable
                var nameField = ((Worker)dynamicVariable).TryGetField<string>("VariableName");
                var isLinked = dynamicVariable.IsLinkedToSpace();

                ui.Style.MinHeight = 24;

                if (isLinked)
                {
                    ui.HorizontalLayout(4);
                    ui.Style.MinWidth = 256;
                    dynamicVariable.TryGetLinkedSpace(out var linkedSpace);
                    ui.Button($"DynamicVariableSpace: {linkedSpace.SpaceName}");
                    ui.BuildOpenReference(linkedSpace);
                    ui.NestOut();
                }
                ui.BuildRenameUI(nameField, onRename: newName => RenameDynamicVariables.RenameDynamicVariable(dynamicVariable, newName));
                break;
            default:
                break;
        }
    }
}
