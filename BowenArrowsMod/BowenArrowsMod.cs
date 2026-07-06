using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.NET.Common;
using BepInExResoniteShim;

namespace BowenArrowsMod;

[ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
[BepInDependency(BepInExResoniteShim.PluginMetadata.GUID, BepInDependency.DependencyFlags.HardDependency)]
public partial class BowenArrowsMod : BasePlugin
{
    internal static new ManualLogSource Log = null!;

    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> ChangeProtoFluxStringInputs;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {PluginMetadata.GUID} is loaded!");
        HarmonyInstance.PatchAll();
        Enabled = Config.Bind("Toggles", "Enabled", true);
        ChangeProtoFluxStringInputs = Config.Bind("Toggles", "Change ProtoFlux String Inputs", false);
    }
}
