using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Suikoden_Fix.Patches;
using HarmonyLib;
using System;

namespace Suikoden_Fix;

[BepInPlugin("s2_character_mod", "S2 Character Mod", "1.1.0")]
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
        CharacterMod.MaxGrowthRates = Config.Bind("1. General", "Max Stat Growth Rates", false, "If true, all entries in character json file will get max stat growth").Value;
        CharacterMod.MaxRuneAffinities = Config.Bind("1. General", "Max Rune Affinities", false, "If true, all entries in character json file will get max rune affinities").Value;
        CharacterMod.MaxRuneLevels = Config.Bind("1. General", "Max Rune Levels", false, "If true, all entries in character json file will get all rune slots unlocked").Value;
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