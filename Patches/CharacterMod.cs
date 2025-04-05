extern alias GSD1;
extern alias GSD2;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using HarmonyLib;
using static GSD1::Saru_h;

namespace Suikoden_Fix.Patches;

public class CharacterMod
{
    public static bool ForceRecreate { get; set; }
    public static bool MaxAll { get; set; }

    [HarmonyPatch(typeof(GSD2.CHARA_DATA), nameof(GSD2.CHARA_DATA.UnityEngine_ISerializationCallbackReceiver_OnAfterDeserialize))]
    [HarmonyPostfix]
    private static void AfterDeserialize(GSD2.CHARA_DATA __instance)
    {
        if (!Character.ConfigExists() || ForceRecreate || MaxAll)
        {
            var characterData = new List<Character>();
            var charNames = Character.PlayerOffsetsString.Split(Environment.NewLine).ToList();
            for (int i = 0; i < __instance.c_kotei_dat.Count; i++)
            {
                GSD2.C_KOTEI_DAT kotei = __instance.c_kotei_dat[i];
                if (kotei.okori_panic == 0) // seems to be null characters
                {
                    continue;
                }
                if (charNames.Count <= i) // Theres like 3 characters that are unaccounted for in name list, dunno who they belong to
                {
                    continue;
                }
                string name = charNames[i].Split(" - ")[0];
                characterData.Add(new Character(name, i, kotei, MaxAll));
            }
            Character.Save(characterData);
        }
        {
            var characterData = Character.Load().ToDictionary(x => x.Name);
            var charNames = Character.PlayerOffsetsString.Split(Environment.NewLine).ToList();
            for (int i = 0; i < __instance.c_kotei_dat.Count; i++)
            {
                GSD2.C_KOTEI_DAT kotei = __instance.c_kotei_dat[i];
                if (kotei.okori_panic == 0) // seems to be null characters
                {
                    continue;
                }
                if (charNames.Count <= i) // Theres like 3 characters that are unaccounted for in name list, dunno who they belong to
                {
                    continue;
                }
                string name = charNames[i].Split(" - ")[0];
                if (!characterData.ContainsKey(name))
                {
                    Plugin.Log.LogError($"Can't find character named {name}");
                    continue;
                }
                var loadedCharacter = characterData[name];
                loadedCharacter.WriteToKotei(kotei);
            }
        }
    }
}

public class Character
{
    public string Name { get; set; }

    [JsonIgnore]
    public int Index { get; set; }

    public byte Str { get; set; }
    public byte Mag { get; set; }
    public byte Prot { get; set; }
    public byte Mdf { get; set; }
    public byte Tech { get; set; }
    public byte Spd { get; set; }
    public byte Luck { get; set; }
    public byte Hp { get; set; }

    public byte FireAff { get; set; }
    public byte WaterAff { get; set; }
    public byte WindAff { get; set; }
    public byte EarthAff { get; set; }
    public byte LightningAff { get; set; }
    public byte ResurrectionAff { get; set; }
    public byte DarkAff { get; set; }
    public byte BrightAff { get; set; }

    public byte HeadLev { get; set; }
    public byte RHLev { get; set; }
    public byte LHLev { get; set; }

    //mon_kosu_type == rune level unlocks
    //mon_zoku_aisyo == Rune Affinity
    //seicyo_type == growthrate
    public Character()
    { }

    public Character(string name, int index, GSD2.C_KOTEI_DAT kotei, bool maxStats)
    {
        Name = name;
        Index = index;
        Str = high(kotei.seicyo_type[0]);
        Mag = low(kotei.seicyo_type[0]);
        Prot = high(kotei.seicyo_type[1]);
        Mdf = low(kotei.seicyo_type[1]);
        Tech = high(kotei.seicyo_type[2]);
        Spd = low(kotei.seicyo_type[2]);
        Luck = high(kotei.seicyo_type[3]);
        Hp = low(kotei.seicyo_type[3]);

        FireAff = high(kotei.mon_zoku_aisyo[0]);
        WaterAff = low(kotei.mon_zoku_aisyo[0]);
        WindAff = high(kotei.mon_zoku_aisyo[1]);
        EarthAff = low(kotei.mon_zoku_aisyo[1]);
        LightningAff = high(kotei.mon_zoku_aisyo[2]);
        ResurrectionAff = low(kotei.mon_zoku_aisyo[2]);
        DarkAff = high(kotei.mon_zoku_aisyo[3]);
        BrightAff = low(kotei.mon_zoku_aisyo[3]);

        HeadLev = kotei.mon_kosu_type[0];
        RHLev = kotei.mon_kosu_type[1];
        LHLev = kotei.mon_kosu_type[2];
        if (maxStats)
        {
            Str = 8;
            Mag = 8;
            Prot = 8;
            Mdf = 8;
            Tech = 8;
            Spd = 8;
            Luck = 8;
            Hp = 8;

            FireAff = 1;
            WaterAff = 1;
            WindAff = 1;
            EarthAff = 1;
            LightningAff = 1;
            ResurrectionAff = 1;
            DarkAff = 1;
            BrightAff = 1;

            HeadLev = 1;
            RHLev = 1;
            LHLev = 1;
        }
    }

    public static void Save(List<Character> characters)
    {
        string folder = "./bepinex/config/s2_character_mod/";
        string file = $"characters.json";
        var json = System.Text.Json.JsonSerializer.Serialize(characters, new JsonSerializerOptions() { WriteIndented = true });
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        File.WriteAllText($"{folder}{file}", json);
    }

    public static List<Character> Load()
    {
        string folder = "./bepinex/config/s2_character_mod/";
        string file = $"characters.json";
        var filetext = File.ReadAllText($"{folder}{file}");
        var characters = JsonSerializer.Deserialize<List<Character>>(filetext);
        return characters;
    }

    public static bool ConfigExists()
    {
        string folder = "./bepinex/config/s2_character_mod/";
        string file = $"characters.json";
        return File.Exists($"{folder}{file}");
    }

    public void WriteToKotei(GSD2.C_KOTEI_DAT kotei)
    {
        kotei.mon_kosu_type[0] = HeadLev;
        kotei.mon_kosu_type[1] = RHLev;
        kotei.mon_kosu_type[2] = LHLev;

        kotei.mon_zoku_aisyo[0] = setByte(FireAff, WaterAff);
        kotei.mon_zoku_aisyo[1] = setByte(WindAff, EarthAff);
        kotei.mon_zoku_aisyo[2] = setByte(LightningAff, ResurrectionAff);
        kotei.mon_zoku_aisyo[3] = setByte(DarkAff, BrightAff);

        kotei.seicyo_type[0] = setByte(Str, Mag);
        kotei.seicyo_type[1] = setByte(Prot, Mdf);
        kotei.seicyo_type[2] = setByte(Tech, Spd);
        kotei.seicyo_type[3] = setByte(Luck, Hp);
    }

    private byte low(byte val)
    { return (byte)(val & 0xf); }

    private byte high(byte val)
    { return (byte)((val >> 4) & 0xf); }

    private byte setByte(byte high, byte low)
    { return (byte)((high << 4) | (low & 0x0F)); }

    /// <summary>
    /// pulled from psx s2 editor tool
    /// </summary>
    public const string PlayerOffsetsString = @"
Hero - 111E1E7A
Flik - 111E1E8C
Viktor - 111E1E9E
Viki - 111E1EB0
Sheena - 111E1EC2
Clive - 111E1ED4
Hix - 111E1EE6
Tengaar - 111E1EF8
Futch - 111E1F0A
Humphrey - 111E1F1C
Georg - 111E1F2E
Valeria - 111E1F40
Pesmerga - 111E1F52
Lorelai - 111E1F64
Shin - 111E1F76
Rikimaru - 111E1F88
Tomo - 111E1F9A
Nanami - 111E1FAC
Eilie - 111E1FBE
Rina - 111E1FD0
Bolgan - 111E1FE2
Tuta - 111E1FF4
Hanna - 111E2006
Millie - 111E2018
Karen - 111E202A
Shiro - 111E203C
Zamza - 111E204E
Gengen - 111E2060
Gabocha - 111E2072
Kinnison - 111E2084
Shilo - 111E2096
Miklotov - 111E20A8
Camus - 111E20BA
Hauser - 111E20CC
Freed Y - 111E20DE
Kahn - 111E20F0
Amada - 111E2102
Tai Ho - 111E2114
Anita - 111E2126
Bob - 111E2138
Meg - 111E214A
Gadget - 111E215C
Ayda - 111E216E
Killey - 111E2180
Sierra - 111E2192
Oulan - 111E21A4
Genshu - 111E21B6
Mukumuku - 111E21C8
Abizboah - 111E21DA
Feather - 111E21EC
Badeaux - 111E21FE
Tsai - 111E2210
Luc - 111E2222
Chaco - 111E2234
Nina - 111E2246
Sid - 111E2258
Yoshino - 111E226A
Gijimu - 111E227C
Koyu - 111E228E
Lo Wen - 111E22A0
Mazus - 111E22B2
Sasuke - 111E22C4
Mondo - 111E22D6
Vincent - 111E22E8
Simone - 111E22FA
Hai Yo - 111E230C
Stallion - 111E231E
Wakaba - 111E2330
L.C. Chan - 111E2342
Gantetsu - 111E2354
Hoi - 111E2366
Sigfried - 111E2378
Kasumi - 111E238A
Rulodia - 111E239C
Makumaku - 111E23AE
Mikumiku - 111E23C0
Mekumeku - 111E23D2
Mokumoku - 111E23E4
Chuchara - 111E23F6
Jowy 2 - 111E2408
";
}