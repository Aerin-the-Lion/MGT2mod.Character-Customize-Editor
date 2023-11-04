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

            //カメラを無効化
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

        [HarmonyPostfix, HarmonyPatch(typeof(Menu_Start), "OnEnable")]
        public static void OnGameStart(splashScript __instance)
        {
            //開始でカメラを初期化
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
            //キャラクターのオブジェクトを取得
            CharacterSelectionMenu.personalCharacterScript = ___cS_;

            //こうしないとボタンの名前が初期化されて変わる
            CharacterSelectionMenuButtonHandler.selectCharacterEditor_Button.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = "Character Editor";
        }
    }
}
