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
using System.ComponentModel;
using static UnityEngine.UI.Image;

namespace Character_Customize_Editor
{
    public class CustomEditor: MonoBehaviour
    {
        //static public bool isActivated = false;
        static public GameObject CC_Editor;
        static public int hierarchyIndex;

        static bool IsExistedCCE()
        {
            Transform parentTransform = GameObject.Find("CanvasInGameMenu").transform;
            Transform existingCC_EditorTransform = parentTransform.Find("Character Customize Editor");
            if (existingCC_EditorTransform == null) 
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Character Customize Editorを設置する関数
        /// in English: A function to place Character Customize Editor
        /// </summary>
        static void SetCC_Editor()
        {
            if (IsExistedCCE()) { return; }            // selectCCE_Buttonが既に存在している場合、早期リターン

            Debug.Log("TESTing... CC_Editor Init TESTing...");
            //__instanceをクローン化
            GameObject menu_NewGameCEO = GameObject.Find("CanvasInGameMenu").transform.Find("Menu_NewGame_S2").gameObject;
            CC_Editor = UnityEngine.Object.Instantiate(menu_NewGameCEO);
            CC_Editor.transform.SetParent(menu_NewGameCEO.transform.parent);
            CC_Editor.transform.localPosition = menu_NewGameCEO.transform.localPosition;
            CC_Editor.transform.localScale = menu_NewGameCEO.transform.localScale;
            CC_Editor.name = "Character Customize Editor";
            hierarchyIndex = GetHierarchyIndex(CC_Editor);

            ModifyCCE_Buttons(CC_Editor);
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

        static GameObject GetPrehabMaterialOfCharacter(Menu_NewGameCEO cCE, int bodyType)
        {
            Debug.Log("GetPrehabMaterialOfCharacter()");
            createCharScript cCS_ = Traverse.Create(cCE).Field<createCharScript>("cCS_").Value;

            GameObject prehab = null;
            int index = bodyType;
            if (cCE.male)
            {
                prehab = cCS_.charGfxMales[index] ;
            }
            else
            {
                prehab = cCS_.charGfxFemales[index];
            }
            Debug.Log("prehabMat : " + prehab.name);
            return prehab;
        }

        static void ImportNewModelSetting(Menu_NewGameCEO cCE, characterScript original)
        {
            original.male = cCE.male;
            original.model_body = cCE.body;
            original.model_eyes = cCE.eyes;
            original.model_hair = cCE.hair;
            original.model_beard = cCE.beard;
            original.model_skinColor = cCE.colorSkin;
            original.model_hairColor = cCE.colorHair;
            original.model_beardColor = cCE.colorHair;
            original.model_HoseColor = cCE.colorHose;
            original.model_ShirtColor = cCE.colorShirt;
            original.model_Add1Color = cCE.colorAdd1;
        }
        static void ImportMovementScript(movementScript oldMovementScript, characterScript original, GameObject newPrehab)
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

        static void SetCreatedToOriginalCharacter()
        {
            characterScript original = AddSelectUIonMenu.personalCharacterScript;
            GameObject originalObj = original.gameObject;
            Menu_NewGameCEO cCE = AddSelectUIonMenu.CCE_instance.GetComponent<Menu_NewGameCEO>();
            
            GameObject oldPrehab = originalObj.transform.GetChild(0).gameObject;
            //oldPrehabの位置情報を取得
            Vector3 oldPrehabPosition = oldPrehab.transform.position;
            //oldPrehabの回転情報を取得
            Quaternion oldPrehabRotation = oldPrehab.transform.rotation;
            //oldPrehabのスケール情報を取得
            Vector3 oldPrehabScale = oldPrehab.transform.localScale;
            //oldPrehabのmovementScriptを取得
            movementScript oldMovementScript = oldPrehab.GetComponent<movementScript>();

            //新しいprehabを追加
            int bodyType = cCE.body;
            GameObject prehab = GetPrehabMaterialOfCharacter(cCE, bodyType);
            GameObject newPrehab = UnityEngine.Object.Instantiate<GameObject>(prehab, oldPrehabPosition, oldPrehabRotation, originalObj.transform);

            //新しいキャラクターモデルの設定を反映
            ImportNewModelSetting(cCE, original);
            ImportMovementScript(oldMovementScript, original, newPrehab);
            characterGFXScript gFX = newPrehab.GetComponent<characterGFXScript>();
            gFX.Init(true);

            //既存のprehabクローンを削除
            UnityEngine.Object.Destroy(oldPrehab);
        }

        static void DisableCameraNewGame()
        {
            if (GetCameraNewGame.cameraNewGame)
            {
                GetCameraNewGame.cameraNewGame.SetActive(false);
            }
        }

        static void OnButton_YesClicked()
        {
            Debug.Log(" -- Button_Yes was clicked -- ");
            DisableCameraNewGame();
            SetCreatedToOriginalCharacter();
            GameObject canvasInGameMenu = GameObject.Find("CanvasInGameMenu").gameObject;
            canvasInGameMenu.transform.GetChild(hierarchyIndex).gameObject.SetActive(false);
            DestroyGameObjectIfExists("CHARNEWGAME");
        }

        static void OnButton_CloseClicked()
        {
            Debug.Log(" -- Button_Close was clicked -- ");
            DisableCameraNewGame();
            GameObject canvasInGameMenu = GameObject.Find("CanvasInGameMenu").gameObject;
            canvasInGameMenu.transform.GetChild(hierarchyIndex).gameObject.SetActive(false);
            DestroyGameObjectIfExists("CHARNEWGAME");
            /*
            GameObject charaNewGame;
            try
            {
                charaNewGame = GameObject.Find("CHARNEWGAME").gameObject;
                UnityEngine.Object.Destroy(charaNewGame);
            }catch{}

            try
            {
                UnityEngine.Object.Destroy(AddSelectUIonMenu.clonedCharacterObject);
            }
            catch { }
            */

        }

        static void DestroyGameObjectIfExists(string name)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        static void ModifyCCE_Buttons(GameObject CC_Editor)
        {
            //Button_Yes
            GameObject Button_Yes = CC_Editor.transform.Find("WindowMain/Button_Yes").gameObject;
            Button Button_Yes_ButtonComponent = Button_Yes.GetComponent<Button>();
            //ボタンのコンポーネントのonClickを初期化
            Button_Yes_ButtonComponent.onClick = new Button.ButtonClickedEvent();
            //ボタンのコンポーネントのonClickに処理を追加
            Button_Yes_ButtonComponent.onClick.AddListener(OnButton_YesClicked);

            //Button_Close
            GameObject Button_Close = CC_Editor.transform.Find("WindowMain/Titelleiste/Button_Close").gameObject;
            Button Button_Close_ButtonComponent = Button_Close.GetComponent<Button>();
            //ボタンのコンポーネントのonClickを初期化
            Button_Close_ButtonComponent.onClick = new Button.ButtonClickedEvent();
            //ボタンのコンポーネントのonClickに処理を追加
            Button_Close_ButtonComponent.onClick.AddListener(OnButton_CloseClicked);
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

        static void Init()
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            SetCC_Editor();
        }

        // --------------------- 以下、Harmonyのパッチ関数 ---------------------
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
    }
}
