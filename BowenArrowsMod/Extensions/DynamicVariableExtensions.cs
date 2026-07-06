using Elements.Core;
using FrooxEngine;
using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace BowenArrowsMod.Extensions;

public static class DynamicVariableExtensions
{
    public static IEnumerable<IDynamicVariable> GetLinkedVariables(this DynamicVariableSpace space,
        Predicate<IDynamicVariable>? filter = null, bool includeLocal = false, bool excludeDisabled = false, Predicate<Slot>? slotFilter = null)
    {
        filter ??= Filter;

        return space.Slot.GetComponentsInChildren<IDynamicVariable>(
            variable => variable.IsLinkedToSpace(space) && filter(variable),
            includeLocal, excludeDisabled, slotFilter!);
    }

    public static bool IsLinkedToSpace(this IDynamicVariable dynamicVariable)
        => dynamicVariable.TryGetLinkedSpace(out _);

    public static bool IsLinkedToSpace(this IDynamicVariable dynamicVariable, DynamicVariableSpace space)
        => dynamicVariable.TryGetLinkedSpace(out var linkedSpace) && linkedSpace == space;

    public static bool TryGetLinkedSpace(this IDynamicVariable dynamicVariable, [NotNullWhen(true)] out DynamicVariableSpace linkedSpace)
    {
        linkedSpace = Traverse.Create(dynamicVariable)
            .Field(nameof(DynamicVariableBase<dummy>.handler))
            .Field(nameof(DynamicVariableHandler<dummy>._currentSpace))
            .GetValue<DynamicVariableSpace>();

        return linkedSpace is not null;
    }

    private static bool Filter(IDynamicVariable variable) => true;
}
