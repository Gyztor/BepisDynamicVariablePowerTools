using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.NET.Common;
using BepInExResoniteShim;

namespace BowenArrowsMod;

[ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
[BepInDependency(BepInExResoniteShim.PluginMetadata.GUID, BepInDependency.DependencyFlags.HardDependency)]
public class BowenArrowsMod : BasePlugin
{
    internal static new ManualLogSource Log = null!;

    internal static ConfigEntry<bool> Mod_Enabled;
    internal static ConfigEntry<bool> ChangeProtoFluxStringInputs;
    internal static ConfigEntry<bool> DebugInfo_Enabled;
    internal static ConfigEntry<bool> DebugInfo_LinkedVars;
    internal static ConfigEntry<bool> DebugInfo_CompHierarchy;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {PluginMetadata.GUID} is loaded!");
        HarmonyInstance.PatchAll();
        Mod_Enabled = Config.Bind("General", "Enabled", true);
        ChangeProtoFluxStringInputs = Config.Bind("General", "Change ProtoFlux String Inputs on Rename", false);
        DebugInfo_Enabled = Config.Bind("Debug Info", "Enabled", true);
        DebugInfo_LinkedVars = Config.Bind("Debug Info", "Output Linked Variables", true);
        DebugInfo_CompHierarchy = Config.Bind("Debug Info", "Output Component Hierarchy", true);
    }
}
