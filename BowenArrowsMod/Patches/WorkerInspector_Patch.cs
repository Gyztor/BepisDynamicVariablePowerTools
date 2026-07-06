using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;

namespace BowenArrowsMod.Patches;

[HarmonyPatch]
public class WorkerInspector_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorkerInspector), "BuildInspectorUI")]
    public static void BuildInspectorUI_Postfix(Worker worker, UIBuilder ui)
    {
        switch (worker)
        {
            case DynamicVariableSpace space:
                // Add custom UI elements for DynamicVariableSpace
                ui.Style.MinHeight = 24 + 36;
                ui.NestInto(ui.Empty("DynamicVariableTools"));
                {
                    ui.HorizontalHeader(36, out RectTransform header, out RectTransform content);
                    
                    ui.Style.MinHeight = 24;

                    ui.NestInto(header);
                    ui.Text("Dynamic Variable Space Tools", alignment: Alignment.MiddleCenter);
                    ui.NestOut();

                    ui.NestInto(content);
                    {
                        ui.BuildRenameUI(space.SpaceName, onRename: newName => RenameDirectlyLinkedVariables.RenameSpace(space, newName));
                    }
                    ui.NestOut();
                }
                ui.NestOut();
                break;
            case IDynamicVariable dynamicVariable:
                // Add custom UI elements for IDynamicVariable
                var nameField = ((Worker)dynamicVariable).TryGetField<string>("VariableName");

                ui.Style.MinHeight = 24 + 36;
                ui.NestInto(ui.Empty("DynamicVariableTools"));
                {
                    ui.HorizontalHeader(36, out RectTransform header, out RectTransform content);


                    ui.Style.MinHeight = 24;

                    ui.NestInto(header);
                    ui.Text("Dynamic Variable Tools", alignment: Alignment.MiddleCenter);
                    ui.NestOut();

                    ui.NestInto(content);
                    {
                        ui.BuildRenameUI(nameField, onRename: newName => RenameDynamicVariables.RenameDynamicVariable(dynamicVariable, newName));
                    }
                    ui.NestOut();
                }
                ui.NestOut();
                break;
            default:
                break;
        }
    }
}
