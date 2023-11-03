using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;


// ILコードのIndexで処理箇所の指定しているので、別のMod、もしくはメインゲーム内でなにかしらの変更があった場合、
// 動作しなくなるおそれがあります。
namespace Character_Customize_Editor
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGuid = "me.Aerin_the_Lion.Mad_Games_Tycoon_2.plugins.Character_Customize_Editor";
        public const string PluginName = "Character Customize Editor";
        public const string PluginVersion = "0.0.0.1";

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
            Harmony.CreateAndPatchAll(typeof(CustomEditor));
            Harmony.CreateAndPatchAll(typeof(AddSelectUIonMenu));
            Harmony.CreateAndPatchAll(typeof(GetCameraNewGame));

        }

        //void Update()
        //{
        //UpdateCount++;
        //Debug.Log("Update Count : " + UpdateCount);
        //}
    }


}