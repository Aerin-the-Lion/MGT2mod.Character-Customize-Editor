using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;

namespace CharacterEditor
{
    public class CharacterSelectionMenu
    {

        static public characterScript personalCharacterScript;
        static public GameObject clonedCharacterObject;

        public static void Init()
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            CharacterSelectionMenuButtonHandler.AddCharacterEditorButtonToPersonalMenu();
        }

        public static void SetClonedCharacter()
        {
            clonedCharacterObject = UnityEngine.Object.Instantiate(personalCharacterScript.gameObject);
            //クローン元と同じ位置情報を共有する
            clonedCharacterObject.transform.position = personalCharacterScript.gameObject.transform.position;
            clonedCharacterObject.transform.rotation = personalCharacterScript.gameObject.transform.rotation;
            clonedCharacterObject.transform.localScale = personalCharacterScript.gameObject.transform.localScale;
            clonedCharacterObject.name = "CHARNEWGAME";
        }

        /// <summary>
        /// 従業員メニュー -> 個人メニュー -> Character Editorを開くボタンを押したときに呼ばれる関数
        /// 個人のcharacterScriptをCharacter Editorに渡す
        /// </summary>
        public static void InitializeCharacterEditorWithSelectedCharacter()
        {
            Menu_NewGameCEO mainMenuScript = CharacterEditorManager.CharacterEditor.GetComponent<Menu_NewGameCEO>();
            Traverse.Create(mainMenuScript).Field("character").SetValue(null);
            Traverse.Create(mainMenuScript).Field("character").SetValue(clonedCharacterObject);

            //EditorのUIを初期化
            mainMenuScript.uiObjects[12].GetComponent<InputField>().text = "";  //.textを空にしないとInitしないので、あえて空にする
            mainMenuScript.Init();
        }

        public static void InitializeTitleOfCharacterEditor()
        {
            string title = "Character Editor : Customize your character";
            CharacterEditorManager.CharacterEditor.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = title;
        }
        /// <summary>
        /// まぁいつかは実装するけど……あったらあったで面倒なので、今は削除しておく
        /// </summary>
        public static void DeleteUnimplementedUI()
        {
            //CharacterEditorManager.CharacterEditor.transform.GetChild(0).Find("BGStats").gameObject.SetActive(false);
            CharacterEditorManager.CharacterEditor.transform.GetChild(0).Find("BGPerks").gameObject.SetActive(false);
        }

        static bool IsNewGameCharacter(GameObject character) => character.name == "CHARNEWGAME";
        static bool IsCharacterEditor(GameObject me) => me.name == "Character Editor";

        public static Menu_NewGameCEO CharacterEditorInstance;
        public static GameObject createdCharacter;

        /// <summary>
        /// 既存のキャラクターデータをロードして、Character Editorに適用します。
        /// </summary>
        /// <param name="__instance">Menu_NewGameCEOのインスタンス(Character Editor)</param>
        /// <param name="___character">ロードするキャラクターオブジェクト</param>
        public static void ImportExistingCharacter(Menu_NewGameCEO CharacterEditor, GameObject ___character)
        {
            GameObject character = clonedCharacterObject;
            ___character = character;

            characterScript characterScript = character.GetComponent<characterScript>();

            // キャラクターの属性をCharacter Editorに適用
            CharacterEditor.uiObjects[12].GetComponent<InputField>().text = characterScript.myName;
            CharacterEditor.beruf = characterScript.beruf;                      //職業
            CharacterEditor.s_gamedesign = characterScript.s_gamedesign;        //ゲームデザイン, Game Design
            CharacterEditor.s_programmieren = characterScript.s_programmieren;  //プログラミング, Programming
            CharacterEditor.s_grafik = characterScript.s_grafik;                //グラフィック, Graphics
            CharacterEditor.s_sound = characterScript.s_sound;                  //音楽, Music & Sound
            CharacterEditor.s_pr = characterScript.s_pr;                        //Promotion, Marketing & Support
            CharacterEditor.s_gametests = characterScript.s_gametests;          //ゲームテスト, Game Tests
            CharacterEditor.s_technik = characterScript.s_technik;              //ハードウェア, Hardware
            CharacterEditor.s_forschen = characterScript.s_forschen;            //研究, Research

            CharacterEditor.s_skills = 0;   //残りのスキルポイント


            //Perk処理
            for (int i = 0; i < CharacterEditor.perks.Length; i++)
            {
                if (characterScript.perks[i])
                {
                    CharacterEditor.perks[i] = true;
                }
                else
                {
                    CharacterEditor.perks[i] = false;
                }
            }

            if (characterScript.male)
            {
                CharacterEditor.male = true;
            }
            else
            {
                CharacterEditor.male = false;
            }

            CharacterEditor.body = characterScript.model_body;
            CharacterEditor.hair = characterScript.model_hair;
            CharacterEditor.eyes = characterScript.model_eyes;
            CharacterEditor.beard = characterScript.model_beard;
            CharacterEditor.colorSkin = characterScript.model_skinColor;
            CharacterEditor.colorHair = characterScript.model_hairColor;
            CharacterEditor.colorShirt = characterScript.model_ShirtColor;
            CharacterEditor.colorHose = characterScript.model_HoseColor;
            CharacterEditor.colorAdd1 = characterScript.model_Add1Color;


            CharacterEditorInstance = CharacterEditor;
            createdCharacter = ___character;
        }

            // --------------------- 以下、Harmonyのパッチ関数 ---------------------


            /// <summary>
            /// 既存のキャラクターデータをロードして、Character Editorに適用します。
            /// </summary>
            /// <param name="__instance">Menu_NewGameCEOのインスタンス(Character Editor)</param>
            /// <param name="___character">ロードするキャラクターオブジェクト</param>
            /// <returns>通常の処理(CEOの作成の場合は、trueを返し、CharacterEditorの場合は、falseを返す</returns>
            [HarmonyPrefix, HarmonyLib.HarmonyPatch(typeof(Menu_NewGameCEO), "LoadData")]
        public static bool OnLoadDataMenu_NewGameCEO(Menu_NewGameCEO __instance, GameObject ___character)
        {
            // Character Customization Editorの条件を確認
            if (!IsCharacterEditor(__instance.gameObject)) { return true; }

            // キャラクターの属性をCharacter Editorに適用
            ImportExistingCharacter(__instance, ___character);
            return false;
        }
    }
}
