using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;

namespace CharacterEditor
{
    public class CharacterEditorButtonHandler : MonoBehaviour
    {
        private static GameObject main_;
        private static GameObject cE_;
        private static Menu_NewGameCEO cEScript_;
        private static sfxScript sfx_;
        private static textScript tS_;
        private static GUI_Main guiMain_;

        private static void FindScripts()
        {
            if (!main_)
            {
                main_ = GameObject.Find("Main");
            }
            if (cE_ == null)
            {
                cE_ = CharacterEditorManager.CharacterEditor;
            }
            if(cEScript_ == null)
            {
                cEScript_ = cE_.GetComponent<Menu_NewGameCEO>();
            }
            if(sfx_ == null)
            {
                sfx_ = GameObject.Find("SFX").GetComponent<sfxScript>();
            }
            if(tS_ == null)
            {
                tS_ = main_.GetComponent<textScript>();
            }
            if(guiMain_ == null)
            {
                guiMain_ = GameObject.Find("CanvasInGameMenu").GetComponent<GUI_Main>();
            }

        }

        private static int ReviewCharacterSetupIntegrity()
        {
            //もし、名前が入力されていなかったら、エラーを表示して、処理を終了する
            if (cEScript_.uiObjects[12].GetComponent<InputField>().text.Length <= 0)
            {
                guiMain_.MessageBox(tS_.GetText(824), false);
                return 1;
            }
            //もし、スキルポイントが残っていたら、エラーを表示して、処理を終了する
            if (cEScript_.s_skills > 0f)
            {
                guiMain_.MessageBox(tS_.GetText(831), false);
                return 1;
            }

            //職業の変更を確認し、変更済みであった場合は、エラーを表示して、処理を終了する
            float value = CharacterSelectionMenu.TotalRemainingSkillPoints;
            int value2 = cEScript_.beruf;
            int value3 = CharacterSelectionMenu.personalCharacterScript.beruf;
            if (value != 0 && cEScript_.s_skills == value && value2 != value3)
            {
                guiMain_.MessageBox("You're change proffession so you must set new skill points!", false);
                return 1;
            }

            //もし、スキルポイントを変更し、スキルポイントがマイナスだったら、エラーを表示して、処理を終了する
            if (cEScript_.s_skills < 0f && cEScript_.s_skills != CharacterSelectionMenu.TotalRemainingSkillPoints)
            {
                guiMain_.MessageBox("The remaining skill points are negative!", false);
                return 1;
            }

            //もし、Perkの数が、IsIndividualPerkCountAppliedで設定した値よりも少なかったら、エラーを表示して、処理を終了する
            if(Main.IsIndividualPerkCountApplied.Value && cEScript_.perks.Count(x => x) < CharacterSelectionMenu.personalCharacterScript.perks.Count(x => x))
            {
                guiMain_.MessageBox("The number of perks is less than the limit!", false);
                return 1;
            }

            //もし、CEOでないキャラクターがCEOのPerkを選択していたら、エラーを表示して、処理を終了する
            if (cEScript_.perks[0] && CharacterSelectionMenu.personalCharacterScript.myID != 1)
            {
                guiMain_.MessageBox("Only CEO can select CEO's perk!", false);
                cEScript_.BUTTON_Perk(0);
                return 1;
            }

            //もし、CEOがCEOのPerkを選択していなかったら、エラーを表示して、処理を終了する
            if (!cEScript_.perks[0] && CharacterSelectionMenu.personalCharacterScript.myID == 1)
            {
                guiMain_.MessageBox("CEO must select CEO's perk!", false);
                //cEScript_.BUTTON_Perk(0);
                return 1;
            }

            int perksLeft = CharacterSelectionMenu.personalCharacterScript.perks.Count(x => x) - cEScript_.perks.Count(x => x);
            //もし、Perks leftがマイナスだった場合、エラーを表示して、処理を終了する
            if (Main.IsIndividualPerkCountApplied.Value && perksLeft < 0)
            {
                guiMain_.MessageBox("The perks left are negative!", false);
                return 1;
            }

            int perksLeft2 = Main.TotalPerksCount.Value - cEScript_.perks.Count(x => x);
            if(CharacterSelectionMenu.personalCharacterScript.myID == 1)
            {
                perksLeft2 = perksLeft2 + 1;
            }
            //もし、Main.IsIndividualPerkCountApplied.Valueがfalseで、perksLeftがマイナスだったら、エラーを表示して、処理を終了する
            if (!Main.IsIndividualPerkCountApplied.Value && perksLeft2 < 0)
            {
                guiMain_.MessageBox("The perks left are negative!", false);
                return 1;
            }
            return 0;
        }

        public static void OnButton_YesClicked()
        {
            FindScripts();
            sfx_.PlaySound(3, true);
            int result = ReviewCharacterSetupIntegrity();
            if (result == 1) { return; }

            //本来は、Perkの数もチェックする必要があるが、Perkの数はn個までなので、チェックしない

            CommonButtonActions();
            CharacterEditorManager.SetCreatedToOriginalCharacter();
        }

        public static void OnButton_CloseClicked()
        {
            FindScripts();
            sfx_.PlaySound(3, true);
            CommonButtonActions();
        }
        static void CommonButtonActions()
        {
            CameraManager.DisableCamera();
            GameObject canvasInGameMenu = GameObject.Find("CanvasInGameMenu").gameObject;
            canvasInGameMenu.transform.GetChild(CharacterEditorManager.HierarchyIndex).gameObject.SetActive(false);
            CharacterEditorManager.DestroyGameObjectIfExists("CHARNEWGAME");
        }

        [HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(Menu_NewGameCEO), "BUTTON_Perk")]
        public static bool OnButton_PerkClicked(Menu_NewGameCEO __instance, int i, sfxScript ___sfx_, GUI_Main ___guiMain_, textScript ___tS_)
        {
            // Character Customization Editorの条件を確認
            if (!CharacterSelectionMenu.IsCharacterEditor(__instance.gameObject)) { return true; }

            Traverse traverse = Traverse.Create(__instance);

            ___sfx_.PlaySound(3, true);
            __instance.perks[i] = !__instance.perks[i];

            int num = 0;
            for (int j = 0; j < __instance.perks.Length; j++)
            {
                if (__instance.perks[j])
                {
                    if (__instance.uiObjects[24].transform.childCount > j)
                    {
                        __instance.uiObjects[24].transform.GetChild(j).GetComponent<Image>().color = ___guiMain_.colors[0];
                        num++;
                    }
                }
                else if (__instance.uiObjects[24].transform.childCount > j)
                {
                    __instance.uiObjects[24].transform.GetChild(j).GetComponent<Image>().color = Color.white;
                }
            }
            string text = ___tS_.GetText(1682);
            text = text.Replace("<NUM>", (CharacterEditorManager.MaxPerksAmount - num).ToString());
            __instance.uiObjects[25].GetComponent<Text>().text = text;
            if (num >= CharacterEditorManager.MaxPerksAmount)
            {
                for (int k = 0; k < __instance.perks.Length; k++)
                {
                    if (__instance.uiObjects[24].transform.childCount > k && !__instance.perks[k])
                    {
                        __instance.uiObjects[24].transform.GetChild(k).GetComponent<Button>().interactable = false;
                    }
                }
            }
            else
            {
                for (int l = 0; l < __instance.uiObjects[24].transform.childCount; l++)
                {
                    if (__instance.uiObjects[24].transform.childCount > l)
                    {
                        __instance.uiObjects[24].transform.GetChild(l).GetComponent<Button>().interactable = true;
                    }
                }
            }
            __instance.DROPDOWN_Beruf();
            traverse.Method("InitSkills").GetValue();

            return false;
        }
    }
}
