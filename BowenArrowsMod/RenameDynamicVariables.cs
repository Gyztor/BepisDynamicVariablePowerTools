using BowenArrowsMod.Extensions;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.ProtoFlux;

namespace BowenArrowsMod;

internal sealed class RenameDynamicVariables
{
    private static Type GetDynVarType(IDynamicVariable dynVar)
        => dynVar.GetType().GetGenericArgumentsFromInterface(typeof(IDynamicVariable<>))[0];

    private static bool IsDynVarOfType(IDynamicVariable dynVar, Type innerType)
        => GetDynVarType(dynVar) == innerType;

    internal static void RenameDynamicVariable(IDynamicVariable dynVar, string newName)
    {
        if (!dynVar.TryGetLinkedSpace(out var linkedSpace))
        {
            var nameField = ((Worker)dynVar).TryGetField<string>("VariableName");
            nameField.Value = newName;
            return;
        }

        var dynVarType = GetDynVarType(dynVar);
        var currentFullName = dynVar.VariableName;
        DynamicVariableHelper.ParsePath(currentFullName, out var currentSpaceName, out var currentVariableName);

        Predicate<IDynamicVariable> predicate = linkedSpace.OnlyDirectBinding
            ? (it => it.VariableName == currentFullName && IsDynVarOfType(it, dynVarType))
            : (it => (it.VariableName == currentFullName || it.VariableName == currentVariableName) && IsDynVarOfType(it, dynVarType));

        foreach ( var linkedVar in linkedSpace.GetLinkedVariables(predicate, true))
        {
            var nameField = ((Worker)linkedVar).TryGetField<string>("VariableName") ?? ((Worker)linkedVar).TryGetField<string>("_variableName");

            if (nameField is not null)
            {
                nameField.Value = newName;
                continue;
            }

            if (linkedVar is ProtoFluxEngineProxy { Node.Target: IProtoFluxNode dynVarNode }
             && dynVarNode.TryGetField("VariableName") is SyncRef<IGlobalValueProxy<string>> nameProxyRef
             && nameProxyRef.Target is GlobalValue<string> nameProxy)
            {
                nameProxy.Value.Value = newName;
                continue;
            }

            if (BowenArrowsMod.ChangeProtoFluxStringInputs.Value && currentSpaceName != null)
            {
                linkedSpace.Slot.ForeachComponentInChildren<IInput<string>>(stringInput =>
                {
                    if (stringInput.Value != currentFullName)
                        return;

                    stringInput.Value = newName;
                }, includeLocal: true, cacheItems: true);
            }
        }

    }
}