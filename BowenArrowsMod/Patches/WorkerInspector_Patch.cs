using FrooxEngine;
using FrooxEngine.UIX;
using Elements.Core;
using HarmonyLib;

namespace BowenArrowsMod.Patches;

[HarmonyPatch]
public class WorkerInspector_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(WorkerInspector), "BuildInspectorUI")]
    public static void BuildInspectorUI_Postfix(WorkerInspector __instance, Worker worker, UIBuilder ui, Predicate<ISyncMember> memberFilter = null)
    {
        switch (worker)
        {
            case DynamicVariableSpace dynamicVariableSpace:
                // Add custom UI elements for DynamicVariableSpace
                ui.Style.MinHeight = 24;
                ui.NestInto(ui.Empty("DynamicVariableTools"));
                {
                    ui.HorizontalHeader(36, out RectTransform header, out RectTransform content);
                    
                    ui.Style.MinHeight = 24;

                    ui.NestInto(header);
                    ui.Text("Dynamic Variable Space Tools", alignment: Alignment.MiddleCenter);
                    ui.NestOut();

                    ui.NestInto(content);
                    {

                    }
                    ui.NestOut();
                }
                ui.NestOut();
                break;
            case IDynamicVariable dynamicVariable:
                // Add custom UI elements for IDynamicVariable
                ui.Style.MinHeight = 24;
                ui.NestInto(ui.Empty("DynamicVariableTools"));
                {
                    ui.HorizontalHeader(36, out RectTransform header, out RectTransform content);


                    ui.Style.MinHeight = 24;

                    ui.NestInto(header);
                    ui.Text("Dynamic Variable Tools", alignment: Alignment.MiddleCenter);
                    ui.NestOut();

                    ui.NestInto(content);
                    {

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
