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
        public const string PluginVersion = "1.0.1.0";

        public static ConfigEntry<bool> CFG_IS_ENABLED { get; private set; }
        public void LoadConfig()
        {
            string textIsEnable = "0. MOD Settings";

            CFG_IS_ENABLED = Config.Bind<bool>(textIsEnable, "Activate the MOD", true, "If you need to enable the mod, toggle it to 'Enabled'");

            Config.SettingChanged += delegate (object sender, SettingChangedEventArgs args) { };
        }

        void Awake()
        {
            LoadConfig();
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            Harmony.CreateAndPatchAll(typeof(CharacterEditorManager));
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