using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Suikoden_Fix.Patches;
using HarmonyLib;
using System;

namespace Suikoden_Fix;

[BepInPlugin("s2_character_mod", "S2 Character Mod", "1.0.0")]
public partial class Plugin : BasePlugin
{
    public new static ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;

        Log.LogInfo("Loading...");
        LoadConfig();
        ApplyPatches();
    }

    public void LoadConfig()
    {
        CharacterMod.ForceRecreate = Config.Bind("1. General", "Force Recreate Character JSON", false, "If true, will export and overwrite the character json to default values.").Value;
        CharacterMod.MaxAll = Config.Bind("1. General", "Max All Stats", false, "If true, all entries in character json file will get max stat growth, max rune affinities, and all rune slots unlocked").Value;
        Config.GetSetting<bool>("1. General", "Force Recreate Character JSON").Value = false;
        Config.Save();
    }

    private void ApplyPatches()
    {
        ApplyPatch(typeof(CharacterMod));

        Log.LogInfo("Patches applied!");
    }

    private void ApplyPatch(Type type)
    {
        Log.LogInfo($"Patching {type.Name}...");

        Harmony.CreateAndPatchAll(type);
    }
}