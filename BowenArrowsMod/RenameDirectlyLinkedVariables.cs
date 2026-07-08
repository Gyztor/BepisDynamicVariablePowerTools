using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;

namespace BepisDynamicVariablePowerTools;

internal sealed class RenameDirectlyLinkedVariables
{
    internal static void RenameSpace(DynamicVariableSpace space, string newName)
    {
        newName = DynamicVariableHelper.ProcessName(newName);
        var currentName = space.SpaceName.Value;

        var prefixName = $"{currentName}/";

        space.Slot.ForeachComponentInChildren<IDynamicVariable>(dynVar =>
        {
            DynamicVariableHelper.ParsePath(dynVar.VariableName, out var spaceName, out var variableName);

            if (spaceName == null || Traverse.Create(dynVar).Field("handler").Field("_currentSpace").GetValue() != space)
                return;

            var nameField = ((Worker)dynVar).TryGetField<string>("VariableName") ?? ((Worker)dynVar).TryGetField<string>("_variableName");

            if (nameField is not null && nameField.Value.StartsWith(prefixName))
            {
                nameField.Value = $"{newName}/{variableName}";
                return;
            }

            if (dynVar is ProtoFluxEngineProxy { Node.Target: IProtoFluxNode dynVarNode }
             && dynVarNode.TryGetField("VariableName") is SyncRef<IGlobalValueProxy<string>> nameProxyRef
             && nameProxyRef.Target is GlobalValue<string> nameProxy
             && nameProxy.Value.Value.StartsWith(prefixName))
            {
                nameProxy.Value.Value = $"{newName}/{variableName}";
                return;
            }
        }, includeLocal: true, cacheItems: true);

        if (BepisDynamicVariablePowerTools.ChangeProtoFluxStringInputs.Value)
        {
            space.Slot.ForeachComponentInChildren<IInput<string>>(stringInput =>
            {
                DynamicVariableHelper.ParsePath(stringInput.Value, out var spaceName, out var variableName);
                if (spaceName == null || spaceName != currentName)
                    return;

                stringInput.Value = stringInput.Value.Replace(spaceName, newName);
            }, includeLocal: true, cacheItems: true);
        }

        space.SpaceName.Value = newName;
    }
}
