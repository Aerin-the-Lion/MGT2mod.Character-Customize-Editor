using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;


namespace CharacterEditor
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGuid = "me.Aerin_the_Lion.Mad_Games_Tycoon_2.plugins.CharacterEditor";
        public const string PluginName = "Character Editor";
        public const string PluginVersion = "1.0.2.0";

        public static ConfigEntry<bool> CFG_IS_ENABLED { get; private set; }
        public static ConfigEntry<float> SkillPointDecreaseMultiplierOnChange { get; private set; }
        public static ConfigEntry<float> SkillPointIncrementValue { get; private set; }
        public static ConfigEntry<bool> IsIndividualPerkCountApplied { get; private set; }
        public static ConfigEntry<int> TotalPerksCount { get; private set; }

        public void LoadConfig()
        {
            string textIsEnable = "0. MOD Settings";
            string textCharacteEditorSkillPointsSetting = "1. Character Editor : Skill Points";
            string textCharacterEditorPerksSetting = "2. Character Editor : Perks";

            CFG_IS_ENABLED = Config.Bind<bool>(textIsEnable, "Activate the MOD", true, "If you need to enable the mod, toggle it to 'Enabled'");
            SkillPointDecreaseMultiplierOnChange = Config.Bind<float>(
                section: textCharacteEditorSkillPointsSetting,
                key: "Skill Point Decrease Multiplier On Change",
                defaultValue: 0.9f,
                new ConfigDescription("When changing skill points or the profession, it decreases the total skill point amount. This is used for adjusting difficulty. The default multiplier is '0.9', but if you don't want to change the total value, please input '1.0'.", new AcceptableValueRange<float>(0.2f, 1.0f))  // ここで範囲を指定
            );

            SkillPointIncrementValue = Config.Bind<float>(textCharacteEditorSkillPointsSetting, "Skill Point Increment Value", 5.0f, "This sets the value to increase or decrease the skill point. The default is '5.0'.");
            IsIndividualPerkCountApplied = Config.Bind<bool>(textCharacterEditorPerksSetting, "Is Individual Perk Count Applied", true, "If you need to apply the perk count limit to each character, toggle it to 'Enabled'.");
            TotalPerksCount = Config.Bind<int>(textCharacterEditorPerksSetting, "Total Perks Count", 4, "This sets the total allowable value for perks. The default is '4'.");
            

            Config.SettingChanged += delegate (object sender, SettingChangedEventArgs args) { };
        }

        void Awake()
        {
            LoadConfig();

            if (!Main.CFG_IS_ENABLED.Value) { return; }
            Harmony.CreateAndPatchAll(typeof(CharacterEditorManager));
            Harmony.CreateAndPatchAll(typeof(CharacterEditorButtonHandler));
            Harmony.CreateAndPatchAll(typeof(CharacterSelectionMenu));
            Harmony.CreateAndPatchAll(typeof(CharacterSelectionMenuButtonHandler));
            Harmony.CreateAndPatchAll(typeof(CameraManager));
            Harmony.CreateAndPatchAll(typeof(IntializeHarmonyPatch));

        }

        //void Update()
        //{
        //UpdateCount++;
        //Debug.Log("Update Count : " + UpdateCount);
        //}
    }


}