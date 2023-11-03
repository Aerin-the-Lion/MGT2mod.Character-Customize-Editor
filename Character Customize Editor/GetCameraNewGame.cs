using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Character_Customize_Editor
{
    public class GetCameraNewGame: MonoBehaviour
    {

        static public GameObject cameraNewGame;
        // --------------------- 以下、Harmonyのパッチ関数 ---------------------

        public static void Init()
        {
            GameObject original = GameObject.Find("CameraNewGame").gameObject;
            //クローン化
            cameraNewGame = GameObject.Instantiate(original);
            cameraNewGame.name = "Character_Customize_Editor_Camera";
            DontDestroyOnLoad(cameraNewGame);
        }
        /// <summary>
        /// セーブデータをロードしたときに呼ばれる初期化関数
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix, HarmonyPatch(typeof(Menu_Start), "OnEnable")]
        public static void savegameScript_LoadTasks_Postfix(splashScript __instance)
        {
            Init();
        }

        /// <summary>
        /// セーブデータをロードしたときに呼ばれる初期化関数
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix, HarmonyPatch(typeof(savegameScript), "LoadTasks")]
        public static void savegameScript_LoadTasks_Postfix(savegameScript __instance)
        {
            if(cameraNewGame == null) { return; }
            if (cameraNewGame)
            {
                cameraNewGame.SetActive(false);
            }
        }
    }
}
