using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;

namespace CharacterEditor
{
    public class CharacterEditorManager : MonoBehaviour
    {
        //static public bool isActivated = false;
        static public GameObject CharacterEditor;
        static public int hierarchyIndex;
        static bool IsExistedCharacterEditor()
        {
            Transform parentTransform = GameObject.Find("CanvasInGameMenu").transform;
            Transform existingCharacterEditorTransform = parentTransform.Find("Character Editor");
            if (existingCharacterEditorTransform == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Character Editorを設置する関数
        /// in English: A function to place Character Editor
        /// </summary>
        static void InitializeCharacterEditor()
        {
            if (IsExistedCharacterEditor()) { return; }            // selectCharacterEditor_Buttonが既に存在している場合、早期リターン

            //__instanceをクローン化
            GameObject menu_NewGameCEO = GameObject.Find("CanvasInGameMenu").transform.Find("Menu_NewGame_S2").gameObject;
            CharacterEditor = UnityEngine.Object.Instantiate(menu_NewGameCEO);
            CharacterEditor.transform.SetParent(menu_NewGameCEO.transform.parent);
            CharacterEditor.transform.localPosition = menu_NewGameCEO.transform.localPosition;
            CharacterEditor.transform.localScale = menu_NewGameCEO.transform.localScale;
            CharacterEditor.name = "Character Editor";
            hierarchyIndex = GetHierarchyIndex(CharacterEditor);

            ModifyCharacterEditor_Buttons(CharacterEditor);
        }

        //Characterが使用しているPrehabを取得する関数
        //例えば、original.transform.GetChild(0)が、characterMale19_MAT(Clone)がある場合は、9を返すswitch文, min = 0, max = 9
        //original.maleがfalseの場合で、characterFemale08_MAT(Clone)がある場合は、7を返すswitch文, min = 0, max = 7
        static int FindPrehabIndexOfCharacter(GameObject original)
        {
            int result = -1; // 何も見つからない場合は-1を返す
            if (original == null) return result;

            characterScript characterScript = original.GetComponent<characterScript>();
            if (characterScript == null) return result;

            GameObject targetChild = original.transform.GetChild(0).gameObject;
            if (targetChild == null) return result;

            string name = targetChild.name;
            if (name == null) return result;

            if (characterScript.male)
            {
                // 男性キャラクターの場合
                for (int i = 10; i <= 99; i++)
                {
                    if (name.Contains("characterMale" + i.ToString("D2")))
                    {
                        result = i - 10;
                        break;
                    }
                }
            }
            else
            {
                // 女性キャラクターの場合
                for (int i = 0; i <= 99; i++)
                {
                    if (name.Contains("characterFemale" + i.ToString("D2")))
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
        }

        static GameObject GetPrehabMaterialOfCharacter(Menu_NewGameCEO CharacterEditor, int bodyType)
        {
            createCharScript cCS_ = Traverse.Create(CharacterEditor).Field<createCharScript>("cCS_").Value;

            GameObject prehab = null;
            int index = bodyType;
            if (CharacterEditor.male)
            {
                prehab = cCS_.charGfxMales[index];
            }
            else
            {
                prehab = cCS_.charGfxFemales[index];
            }
            return prehab;
        }

        static void ImportNewSkillSetting(Menu_NewGameCEO CharacterEditor, characterScript original)
        {
            original.beruf = CharacterEditor.beruf;                     //職業
            original.s_gamedesign = CharacterEditor.s_gamedesign;       //ゲームデザイン, Game Design
            original.s_programmieren = CharacterEditor.s_programmieren; //プログラミング, Programming
            original.s_grafik = CharacterEditor.s_grafik;               //グラフィック, Graphics
            original.s_sound = CharacterEditor.s_sound;                 //音楽, Music & Sound
            original.s_pr = CharacterEditor.s_pr;                       //Promotion, Marketing & Support
            original.s_gametests = CharacterEditor.s_gametests;         //ゲームテスト, Game Tests
            original.s_technik = CharacterEditor.s_technik;             //ハードウェア, Hardware
            original.s_forschen = CharacterEditor.s_forschen;           //研究, Research
        }

        static void ImportNewModelSetting(Menu_NewGameCEO CharacterEditor, characterScript original)
        {
            original.myName = CharacterEditor.uiObjects[12].GetComponent<InputField>().text;
            original.male = CharacterEditor.male;                       //性別, Gender
            original.model_body = CharacterEditor.body;                 //体型, Body
            original.model_eyes = CharacterEditor.eyes;                 //目, Eyes
            original.model_hair = CharacterEditor.hair;                 //髪型, Hair
            original.model_beard = CharacterEditor.beard;               //髭, Beard
            original.model_skinColor = CharacterEditor.colorSkin;       //肌色, Skin Color
            original.model_hairColor = CharacterEditor.colorHair;       //髪色, Hair Color
            original.model_beardColor = CharacterEditor.colorHair;      //髭色, Beard Color, ただし髭の色は髪の色と同じ
            original.model_HoseColor = CharacterEditor.colorHose;       //ズボンの色, Hose Color
            original.model_ShirtColor = CharacterEditor.colorShirt;     //シャツの色, Shirt Color
            original.model_Add1Color = CharacterEditor.colorAdd1;       //アクセサリーの色, Accessory Color
        }
        public static void ImportMovementScript(movementScript oldMovementScript, characterScript original, GameObject newPrehab)
        {
            movementScript movementScript = original.gameObject.GetComponent<movementScript>();
            GameObject main_ = GameObject.FindWithTag("Main");
            movementScript.main_ = main_;
            movementScript.mS_ = main_.GetComponent<mainScript>();
            movementScript.cS_ = original;
            movementScript.sfx_ = GameObject.Find("SFX").GetComponent<sfxScript>();
            movementScript.clipS_ = main_.GetComponent<clipScript>();
            movementScript.mapS_ = main_.GetComponent<mapScript>();
            //Traverse.Create(movementScript).Method("Init").GetValue();

            movementScript.charGFX = newPrehab;
            movementScript.charAnimation = movementScript.charGFX.GetComponent<Animator>();
            Traverse.Create(movementScript).Method("InitPathfinding").GetValue();
        }

        public static void SetCreatedToOriginalCharacter()
        {
            characterScript original = CharacterSelectionMenu.personalCharacterScript;
            GameObject originalObj = original.gameObject;
            Menu_NewGameCEO CharacterEditor = CharacterSelectionMenu.CharacterEditorInstance.GetComponent<Menu_NewGameCEO>();

            //既存のprehabの設定などを取得
            GameObject oldPrehab = originalObj.transform.GetChild(0).gameObject;
            Vector3 oldPrehabPosition = oldPrehab.transform.position;
            Quaternion oldPrehabRotation = oldPrehab.transform.rotation;
            Vector3 oldPrehabScale = oldPrehab.transform.localScale;
            movementScript oldMovementScript = oldPrehab.GetComponent<movementScript>();

            //新しいprehabを追加
            int bodyType = CharacterEditor.body;
            GameObject prehab = GetPrehabMaterialOfCharacter(CharacterEditor, bodyType);
            GameObject newPrehab = Instantiate<GameObject>(prehab, oldPrehabPosition, oldPrehabRotation, originalObj.transform);

            //新しいキャラクターモデルの設定を反映
            ImportNewModelSetting(CharacterEditor, original);
            //新しいスキルの設定を反映
            ImportNewSkillSetting(CharacterEditor, original);
            //movementScriptを初期化
            ImportMovementScript(oldMovementScript, original, newPrehab);
            characterGFXScript gFX = newPrehab.GetComponent<characterGFXScript>();
            gFX.Init(true);

            //既存のprehabクローンを削除
            UnityEngine.Object.Destroy(oldPrehab);
        }

        public static void DestroyGameObjectIfExists(string name)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        static void ModifyCharacterEditor_Buttons(GameObject CharacterEditor)
        {
            //Button_Yes
            GameObject Button_Yes = CharacterEditor.transform.Find("WindowMain/Button_Yes").gameObject;
            Button Button_Yes_ButtonComponent = Button_Yes.GetComponent<Button>();
            //ボタンのコンポーネントのonClickを初期化
            Button_Yes_ButtonComponent.onClick = new Button.ButtonClickedEvent();
            //ボタンのコンポーネントのonClickに処理を追加
            Button_Yes_ButtonComponent.onClick.AddListener(CharacterEditorButtonHandler.OnButton_YesClicked);

            //Button_Close
            GameObject Button_Close = CharacterEditor.transform.Find("WindowMain/Titelleiste/Button_Close").gameObject;
            Button Button_Close_ButtonComponent = Button_Close.GetComponent<Button>();
            //ボタンのコンポーネントのonClickを初期化
            Button_Close_ButtonComponent.onClick = new Button.ButtonClickedEvent();
            //ボタンのコンポーネントのonClickに処理を追加
            Button_Close_ButtonComponent.onClick.AddListener(CharacterEditorButtonHandler.OnButton_CloseClicked);
        }

        /// <summary>
        /// ヒエラルキーの何番目かを取得する関数（Unity標準では無かったため）
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        static int GetHierarchyIndex(GameObject gameObject)
        {
            Transform parentTransform = gameObject.transform.parent;
            if (parentTransform != null)
            {
                for (int i = 0; i < parentTransform.childCount; i++)
                {
                    if (parentTransform.GetChild(i) == gameObject.transform)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static void Init()
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            InitializeCharacterEditor();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_NewGameCEO), "Update")]
        public static bool OnUpdateMenu_NewGameCEO_DisableSkillpointCap(Menu_NewGameCEO __instance)
        {
            if (!Main.CFG_IS_ENABLED.Value) { return true; }
            if (__instance.name != "Character Editor") { return true; }
            return false;
        }
    }
}
