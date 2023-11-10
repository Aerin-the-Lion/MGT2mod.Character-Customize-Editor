using System;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections;

namespace CharacterEditor
{
    public class CharacterEditorManager : MonoBehaviour
    {
        static public GameObject CharacterEditor;
        static public int HierarchyIndex;
        static public float MaxSkillCap;
        static public int MaxPerksAmount;

        static private sfxScript sfx_;
        static private void FindScripts()
        {
            if (sfx_ == null)
            {
                sfx_ = GameObject.Find("SFX").GetComponent<sfxScript>();
            }
        }
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
            //階層がおかしくなるので、Character EditorをMenu_NewGameCEOの下に置く
            CharacterEditor.transform.SetSiblingIndex(menu_NewGameCEO.transform.GetSiblingIndex());
            CharacterEditor.name = "Character Editor";
            HierarchyIndex = GetHierarchyIndex(CharacterEditor);

            ModifyCharacterEditor_Buttons(CharacterEditor);
        }

        /// <summary>
        /// Characterが使用しているPrehabを取得する関数
        /// ただし、現在使用していません
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
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

        /// <summary>
        /// キャラクターのPrehabをbodyTypeによって取得する関数
        /// </summary>
        /// <param name="CharacterEditor"></param>
        /// <param name="bodyType"></param>
        /// <returns></returns>
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

        static void ImportNewPerksSetting(Menu_NewGameCEO CharacterEditor, characterScript original)
        {
            for (int i = 0; i < original.perks.Length; i++)
            {
                original.perks[i] = CharacterEditor.perks[i];
            }
        }

        /// <summary>
        /// Character Editorのスキル設定を既存のキャラクターに反映する関数
        /// </summary>
        /// <param name="CharacterEditor"></param>
        /// <param name="original"></param>
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

        /// <summary>
        /// Character Editorのモデル設定を既存のキャラクターに反映する関数
        /// </summary>
        /// <param name="CharacterEditor"></param>
        /// <param name="original"></param>
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

        /// <summary>
        /// 既存キャラクターに新規プレハブの諸々を適用する関数
        /// </summary>
        /// <param name="oldMovementScript"></param>
        /// <param name="original"></param>
        /// <param name="newPrehab"></param>
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
            movementScript.charGFX = newPrehab;
            movementScript.charAnimation = movementScript.charGFX.GetComponent<Animator>();
            Traverse.Create(movementScript).Method("InitPathfinding").GetValue();
        }

        /// <summary>
        /// Character EditorをYesボタンで決定した際に、変更内容を既存のキャラクターに反映する関数
        /// </summary>
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
            //新しいPerkの設定を反映
            ImportNewPerksSetting(CharacterEditor, original);
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

        /// <summary>
        /// Character Editorのボタンの処理を変更する関数
        /// Yes, Closeボタンの処理を変更する
        /// </summary>
        /// <param name="CharacterEditor"></param>
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

        static public float GetMaxSkillCap()
        {
            characterScript characterScript = new characterScript();
            Traverse traverse = Traverse.Create(characterScript);
            int i = 0;
            characterScript.beruf = i;
            MaxSkillCap = traverse.Method("GetSkillCap_Skill", new Type[] { typeof(int)}).GetValue<float>(i);
            return MaxSkillCap;
        }

        /// <summary>
        /// カウントを増減するメソッド
        /// </summary>
        /// <param name="isIncrement"></param>
        public static float ChangeCount(bool isIncrement, float skillPoint, float totalSkillPoints)
        {
            float changeValue = Main.SkillPointIncrementValue.Value;

            // 端数を確認して、必要に応じてchangeValueを調整
            if(totalSkillPoints != 0 && totalSkillPoints < changeValue)
            {
                changeValue = totalSkillPoints;
            }

            // カウントを増減
            if (isIncrement)
            {
                skillPoint += changeValue;
            }
            else
            {
                skillPoint -= changeValue;
            }

            return skillPoint;
        }

        /// <summary>
        /// Modによって追加されたPerk処理を行う関数
        /// </summary>
        /// <returns></returns>
        private static int GetMaximumPerksAllowed()
        {
            int maxPerksAmount = 0;

            //個人のPerkを元に、Perkの数を取得するかどうか
            if (Main.IsIndividualPerkCountApplied.Value)
            {
                foreach (bool perk in CharacterSelectionMenu.personalCharacterScript.perks)
                {
                    if (perk)
                    {
                        maxPerksAmount++;
                    }
                }
                if(CharacterSelectionMenu.personalCharacterScript.myID == 1)
                {
                    //プレイヤーキャラCEOの場合は、Perkの数を1つ増やす
                    maxPerksAmount--;
                }
            }
            else
            {
                //設定したPerkの許容値を取得する
                maxPerksAmount = Main.TotalPerksCount.Value;
            }

            if (CharacterSelectionMenu.personalCharacterScript.myID == 1)
            {
                //プレイヤーキャラCEOの場合は、Perkの数を1つ増やす
                maxPerksAmount++;
            }

            Debug.Log("maxPerksAmount: " + maxPerksAmount.ToString());
            return maxPerksAmount;
        }

        private static void ResizeGridLayoutGroupOnPanel(Transform panelTransform)
        {
            float x = 37;
            float y = 37;
            panelTransform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(x, y);
        }

        private static void EnableHiddenPerksButtons(Menu_NewGameCEO __instance)
        {
            Transform PanelPerksButtonsTransform = __instance.gameObject.transform.transform.Find("WindowMain/BGPerks/PanelPerksButtons").transform;

            //PanelPerksButtonsTransformのChildを全てSetActive(true)にする
            foreach (Transform child in PanelPerksButtonsTransform)
            {
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
            }

            ResizeGridLayoutGroupOnPanel(PanelPerksButtonsTransform);
        }


        // ---------------- Initialize ----------------
        public static void Init()
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            InitializeCharacterEditor();
        }


        // ---------------- Harmony Patch ----------------
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_NewGameCEO), "BUTTON_AddStats")]
        public static bool OnPushBUTTON_AddStats_NewGameCEO_ChangeAmount(Menu_NewGameCEO __instance, int i)
        {
            // Config及びCharacter Customization Editorの条件を確認
            if (!Main.CFG_IS_ENABLED.Value) { return true; }
            if (!CharacterSelectionMenu.IsCharacterEditor(__instance.gameObject)) { return true; }


            Traverse traverse = Traverse.Create(__instance);
            var GetSkillCap = traverse.Method("GetSkillCap").GetValue<float>();
            var iAddStats = traverse.Method("iAddStats", new Type[] { typeof(int) }).GetValue<IEnumerator>(i);
            var InitSkills = traverse.Method("InitSkills").GetValue();

            GetMaxSkillCap();
            FindScripts();

            // ------- 以下、元の処理の改変 -------
            sfx_.PlaySound(3, true);
            if (__instance.s_skills <= 0f)
            {
                return false;
            }
            switch (i)
            {
                case 0:
                    if (__instance.s_gamedesign < MaxSkillCap && __instance.beruf == 0)
                    {
                        __instance.s_gamedesign = ChangeCount(true, __instance.s_gamedesign, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_gamedesign += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_gamedesign < GetSkillCap && __instance.beruf != 0)
                    {
                        __instance.s_gamedesign = ChangeCount(true, __instance.s_gamedesign, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_gamedesign += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
                case 1:
                    if (__instance.s_programmieren < MaxSkillCap && __instance.beruf == 1)
                    {
                        __instance.s_programmieren = ChangeCount(true, __instance.s_programmieren, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_programmieren += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_programmieren < GetSkillCap && __instance.beruf != 1)
                    {
                        __instance.s_programmieren = ChangeCount(true, __instance.s_programmieren, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_programmieren += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
                case 2:
                    if (__instance.s_grafik < MaxSkillCap && __instance.beruf == 2)
                    {
                        __instance.s_grafik = ChangeCount(true, __instance.s_grafik, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_grafik += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_grafik < GetSkillCap && __instance.beruf != 2)
                    {
                        __instance.s_grafik = ChangeCount(true, __instance.s_grafik, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_grafik += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
                case 3:
                    if (__instance.s_sound < MaxSkillCap && __instance.beruf == 3)
                    {
                        __instance.s_sound = ChangeCount(true, __instance.s_sound, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_sound += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_sound < GetSkillCap && __instance.beruf != 3)
                    {
                        __instance.s_sound = ChangeCount(true, __instance.s_sound, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_sound += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
                case 4:
                    if (__instance.s_pr < MaxSkillCap && __instance.beruf == 4)
                    {
                        __instance.s_pr = ChangeCount(true, __instance.s_pr, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_pr += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_pr < GetSkillCap && __instance.beruf != 4)
                    {
                        __instance.s_pr = ChangeCount(true, __instance.s_pr, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_pr += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
                case 5:
                    if (__instance.s_gametests < MaxSkillCap && __instance.beruf == 5)
                    {
                        __instance.s_gametests = ChangeCount(true, __instance.s_gametests, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_gametests += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_gametests < GetSkillCap && __instance.beruf != 5)
                    {
                        __instance.s_gametests = ChangeCount(true, __instance.s_gametests, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_gametests += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
                case 6:
                    if (__instance.s_technik < MaxSkillCap && __instance.beruf == 6)
                    {
                        __instance.s_technik = ChangeCount(true, __instance.s_technik, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_technik += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_technik < GetSkillCap && __instance.beruf != 6)
                    {
                        __instance.s_technik = ChangeCount(true, __instance.s_technik, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_technik += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
                case 7:
                    if (__instance.s_forschen < MaxSkillCap && __instance.beruf == 7)
                    {
                        __instance.s_forschen = ChangeCount(true, __instance.s_forschen, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_forschen += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    if (__instance.s_forschen < GetSkillCap && __instance.beruf != 7)
                    {
                        __instance.s_forschen = ChangeCount(true, __instance.s_forschen, __instance.s_skills);
                        __instance.s_skills = ChangeCount(false, __instance.s_skills, __instance.s_skills);
                        //__instance.s_forschen += 5f;
                        //__instance.s_skills -= 5f;
                    }
                    break;
            }
            traverse.Method("InitSkills").GetValue();
            __instance.StartCoroutine(iAddStats);

            return false;
        }

        /// <summary>
        /// CEO作成時のときに、スキルポイント上限を50に設定してあった処理を無効化する
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_NewGameCEO), "Update")]
        public static bool OnUpdateMenu_NewGameCEO_DisableSkillpointCap(Menu_NewGameCEO __instance)
        {
            if (!Main.CFG_IS_ENABLED.Value) { return true; }
            if (!CharacterSelectionMenu.IsCharacterEditor(__instance.gameObject)) { return true; }
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Menu_NewGameCEO), "Init")]
        public static void OnUpdateMenu_NewGameCEO_CalcPerks(Menu_NewGameCEO __instance)
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            if (!CharacterSelectionMenu.IsCharacterEditor(__instance.gameObject)) { return; }

            //Perkの数を確認
            MaxPerksAmount = 0;
            MaxPerksAmount = GetMaximumPerksAllowed();
        }

        /// <summary>
        /// CEO作成時のときに、CEOかどうかの処理をする。
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Menu_NewGameCEO), "Init")]
        public static void OnUpdateMenu_NewGameCEO_NoCeoPerkWhenIsNotCEO(Menu_NewGameCEO __instance)
        {
            if (!Main.CFG_IS_ENABLED.Value) { return; }
            if (!CharacterSelectionMenu.IsCharacterEditor(__instance.gameObject)) { return; }

            if(CharacterSelectionMenu.personalCharacterScript.myID == 1 && !__instance.perks[0])
            {
                __instance.BUTTON_Perk(0);
            }

            //CEO出ない場合は、CEOのPerkを選択できないようにする
            if (CharacterSelectionMenu.personalCharacterScript.myID != 1 && __instance.perks[0])
            {
                __instance.BUTTON_Perk(0);
            }

            //無理やり動かせるようにする
            for (int l = 0; l < __instance.uiObjects[24].transform.childCount; l++)
            {
                if (__instance.uiObjects[24].transform.childCount > l)
                {
                    __instance.uiObjects[24].transform.GetChild(l).GetComponent<Button>().interactable = true;
                }
            }

            EnableHiddenPerksButtons(__instance);
        }
    }
}
