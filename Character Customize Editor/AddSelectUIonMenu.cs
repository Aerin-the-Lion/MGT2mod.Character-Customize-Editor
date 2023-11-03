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
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.Events;

namespace Character_Customize_Editor
{
    public class AddSelectUIonMenu
    {

        static public characterScript personalCharacterScript;
        static public GameObject clonedCharacterObject;

        public static void SetCloneCharacter()
        {
            clonedCharacterObject = UnityEngine.Object.Instantiate(personalCharacterScript.gameObject);
            //クローン元と同じ位置情報を共有する
            clonedCharacterObject.transform.position = personalCharacterScript.gameObject.transform.position;
            clonedCharacterObject.transform.rotation = personalCharacterScript.gameObject.transform.rotation;
            clonedCharacterObject.transform.localScale = personalCharacterScript.gameObject.transform.localScale;
            clonedCharacterObject.name = "CHARNEWGAME";
        }

        /// <summary>
        /// 従業員メニュー -> 個人メニュー -> Character Customize Editorを開くボタンを押したときに呼ばれる関数
        /// 個人のcharacterScriptをCharacter Customize Editorに渡す
        /// </summary>
        static void SetChoseCharacterScript()
        {
            Debug.Log(" -- SetCustomEditor() Entered -- ");
            Menu_NewGameCEO mainMenuScript = CustomEditor.CC_Editor.GetComponent<Menu_NewGameCEO>();
            Traverse.Create(mainMenuScript).Field("character").SetValue(null);
            Traverse.Create(mainMenuScript).Field("character").SetValue(clonedCharacterObject);

            //初期化 .textを空にしないとInitしないので……
            mainMenuScript.uiObjects[12].GetComponent<InputField>().text = "";
            mainMenuScript.Init();
        }

        static void EnableCameraNewGame()
        {
            if (GetCameraNewGame.cameraNewGame)
            {
                GetCameraNewGame.cameraNewGame.SetActive(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnSelectCCEButtonClicked()
        {
            Debug.Log(" -- selectCCE_Button was clicked -- ");
            SetCloneCharacter();
            SetChoseCharacterScript();
            EnableCameraNewGame();
            GameObject canvasInGameMenu = GameObject.Find("CanvasInGameMenu").gameObject;
            canvasInGameMenu.transform.GetChild(CustomEditor.hierarchyIndex).gameObject.SetActive(true);
        }

        /// <summary>
        /// 従業員メニューの個人メニューに、Character Customize Editorを開くボタンを追加する関数
        /// in English: Add a button to open Character Customize Editor to the personal menu of the employee menu
        /// </summary>
        static void SetSelectCCE_Button()
        {
            // 既に存在するselectCCE_Buttonを探す
            Transform parentTransform = GameObject.Find("CanvasInGameMenu").transform;
            Transform menu_Personal_ViewTransform = parentTransform.Find("Menu_PersonalView/WindowMain");
            Transform existingSelectCCE_ButtonTransform = menu_Personal_ViewTransform.Find("Button_Select_CCE");
            if (existingSelectCCE_ButtonTransform != null) { return; }            // selectCCE_Buttonが既に存在している場合、早期リターン

            Debug.Log(" TESTing... Menu_PersonalView_Add Custom Button Init TESTing...");
            //__instanceをクローン化
            GameObject menu_PersonalView = GameObject.Find("CanvasInGameMenu").transform.Find("Menu_PersonalView").gameObject;
            GameObject select_Button = menu_PersonalView.transform.Find("WindowMain").transform.Find("Button_Selektieren").gameObject;
            GameObject selectCCE_Button = UnityEngine.Object.Instantiate(select_Button);

            //クローン化したものの名前を変更
            selectCCE_Button.name = "Button_Select_CCE";
            //クローン化したものの親を変更
            selectCCE_Button.transform.SetParent(select_Button.transform.parent);
            //クローン化したものの位置を変更z
            selectCCE_Button.transform.localScale = select_Button.transform.localScale;
            selectCCE_Button.transform.localPosition = new Vector3(select_Button.transform.localPosition.x * 3.178f, 0, 0);

            //Buttonをクリックしたときの処理を追加
            //ボタンのコンポーネントを取得
            Button selectCCE_ButtonComponent = selectCCE_Button.GetComponent<Button>();
            //ボタンのコンポーネントのonClickを初期化
            selectCCE_ButtonComponent.onClick = new Button.ButtonClickedEvent();
            //ボタンのコンポーネントのonClickに処理を追加
            AddSelectUIonMenu addSelectUIonMenu = new AddSelectUIonMenu();
            selectCCE_ButtonComponent.onClick.AddListener(addSelectUIonMenu.OnSelectCCEButtonClicked);
        }

        static void Init()
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            SetSelectCCE_Button();
        }

        static bool IsNewGameCharacter(GameObject character) => character.name == "CHARNEWGAME";
        static bool IsCCE(GameObject me) => me.name == "Character Customize Editor";

        public static Menu_NewGameCEO CCE_instance;
        public static GameObject createdCharacter;

        // --------------------- 以下、Harmonyのパッチ関数 ---------------------

        
        /// <summary>
        /// 2023.10.29 なんか知らんけどバグってNullReferenceExceptionが出るので、対処する
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="___character"></param>
        /// <returns></returns>
        [HarmonyPrefix, HarmonyPatch(typeof(Menu_NewGameCEO), "LoadData")]
        public static bool CCE_LoadExistingCharacter(Menu_NewGameCEO __instance, GameObject ___character)
        {
            if (IsCCE(__instance.gameObject) == false) { return true; }

            GameObject chara = clonedCharacterObject;
            ___character = chara;

            characterScript charaScript = chara.GetComponent<characterScript>();

            
            __instance.uiObjects[12].GetComponent<InputField>().text = charaScript.myName;
            __instance.beruf = charaScript.beruf;
            __instance.s_skills = charaScript.GetBestSkillValue();
            __instance.s_gamedesign = charaScript.s_gamedesign;
            __instance.s_programmieren = charaScript.s_programmieren;
            __instance.s_grafik = charaScript.s_grafik;
            __instance.s_sound = charaScript.s_sound;
            __instance.s_pr = charaScript.s_pr;
            __instance.s_gametests = charaScript.s_gametests;
            __instance.s_technik = charaScript.s_technik;
            __instance.s_forschen = charaScript.s_forschen;
            
            
            //Perk処理
            for (int i = 0; i < __instance.perks.Length; i++)
            {
                if (charaScript.perks[i])
                {
                    __instance.perks[i] = true;
                }
                else
                {
                    __instance.perks[i] = false;
                }
            }

            if (charaScript.male)
            {
                __instance.male = true;
            }
            else
            {
                __instance.male = false;
            }

            __instance.body = charaScript.model_body;
            __instance.hair = charaScript.model_hair;
            __instance.eyes = charaScript.model_eyes;
            __instance.beard = charaScript.model_beard;
            __instance.colorSkin = charaScript.model_skinColor;
            __instance.colorHair = charaScript.model_hairColor;
            __instance.colorShirt = charaScript.model_ShirtColor;
            __instance.colorHose = charaScript.model_HoseColor;
            __instance.colorAdd1 = charaScript.model_Add1Color;


            CCE_instance = __instance;
            createdCharacter = ___character;
            Debug.Log("CHECKING... Menu_NewGameCEO.LoadData END");
            return false;
        }

        /// <summary>
        /// セーブデータをロードしたときに呼ばれる初期化関数
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix, HarmonyPatch(typeof(savegameScript), "LoadTasks")]
        public static void savegameScript_LoadTasks_Postfix(savegameScript __instance)
        {
            Init();
        }

        /// <summary>
        /// New Gameメニューを開いたときに呼ばれる初期化関数
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Menu_NewGame), "OnEnable")]
        public static void Menu_NewGameCEO_Start_Postfix(Menu_NewGame __instance)
        {
            Init();
        }

        /// <summary>
        /// 従業員メニューの個人メニューを開いたときに呼ばれる初期化関数
        /// 従業員（キャラクター）のオブジェクトを取得する
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Menu_PersonalView), "SetData")]
        public static void Menu_PersonalView_Start_Postfix(Menu_PersonalView __instance, characterScript ___cS_)
        {
            personalCharacterScript = ___cS_;
            //personalCharacterScript = Traverse.Create(__instance).Field("cS_").GetValue<characterScript>();
        }
    }
}
