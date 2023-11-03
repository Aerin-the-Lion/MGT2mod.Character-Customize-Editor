using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CharacterEditor
{
    public class IntializeHarmonyPatch: MonoBehaviour
    {
        /// <summary>
        /// セーブデータをロードしたときに呼ばれる初期化関数
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix, HarmonyLib.HarmonyPatch(typeof(savegameScript), "LoadTasks")]
        public static void savegameScript_LoadTasks_Postfix(savegameScript __instance)
        {
            CharacterEditorManager.Init();
            CharacterSelectionMenu.Init();

            if (CameraManager.CharacterEditorCamera == null) { return; }
            if (CameraManager.CharacterEditorCamera)
            {
                CameraManager.CharacterEditorCamera.SetActive(false);
            }
        }

        /// <summary>
        /// New Gameメニューを開いたときに呼ばれる初期化関数
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(Menu_NewGame), "OnEnable")]
        public static void Menu_NewGameCEO_Start_Postfix(Menu_NewGame __instance)
        {
            CharacterEditorManager.Init();
            CharacterSelectionMenu.Init();
        }

        [HarmonyPostfix, HarmonyLib.HarmonyPatch(typeof(Menu_Start), "OnEnable")]
        public static void OnGameStart(splashScript __instance)
        {
            CameraManager.InitializeCamera();
        }

        /// <summary>
        /// 従業員メニューの個人メニューを開いたときに呼ばれる初期化関数
        /// 従業員（キャラクター）のオブジェクトを取得する
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyLib.HarmonyPatch(typeof(Menu_PersonalView), "SetData")]
        public static void Menu_PersonalView_Start_Postfix(Menu_PersonalView __instance, characterScript ___cS_)
        {
            CharacterSelectionMenu.personalCharacterScript = ___cS_;
        }
    }
}
