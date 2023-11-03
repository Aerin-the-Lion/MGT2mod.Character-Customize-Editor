using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace CharacterEditor
{
    public class CharacterSelectionMenuButtonHandler: MonoBehaviour
    {

        private static GameObject originalSelectButton;
        public static GameObject selectCharacterEditor_Button;
        /// <summary>
        /// 従業員メニューの個人メニューに、Character Editorを開くボタンを追加する関数
        /// in English: Add a button to open Character Editor to the personal menu of the employee menu
        /// </summary>
        public static void AddCharacterEditorButtonToPersonalMenu()
        {
            if (IsCharacterEditorButtonExists()) { return; }

            selectCharacterEditor_Button = CloneSelectButton();
            CustomizeClonedButton(selectCharacterEditor_Button);
            SetButtonOnClickEvent(selectCharacterEditor_Button);
        }

        private static bool IsCharacterEditorButtonExists()
        {
            Transform menu_Personal_ViewTransform = GameObject.Find("CanvasInGameMenu").transform.Find("Menu_PersonalView/WindowMain");
            Transform existingSelectCharacterEditor_ButtonTransform = menu_Personal_ViewTransform.Find("Button_Select_CharacterEditor");
            return existingSelectCharacterEditor_ButtonTransform != null;
        }

        private static GameObject CloneSelectButton()
        {
            GameObject menu_PersonalView = GameObject.Find("CanvasInGameMenu").transform.Find("Menu_PersonalView").gameObject;
            originalSelectButton = menu_PersonalView.transform.Find("WindowMain/Button_Selektieren").gameObject;
            return UnityEngine.Object.Instantiate(originalSelectButton);
        }

        private static void CustomizeClonedButton(GameObject selectCharacterEditor_Button)
        {
            selectCharacterEditor_Button.name = "Button_Select_CharacterEditor";
            selectCharacterEditor_Button.transform.Find("Text").GetComponent<Text>().text = "Character Editor";
            selectCharacterEditor_Button.transform.SetParent(originalSelectButton.transform.parent);
            selectCharacterEditor_Button.transform.localScale = originalSelectButton.transform.localScale;
            selectCharacterEditor_Button.transform.localPosition = new Vector3(originalSelectButton.transform.localPosition.x * 3.178f, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void OnSelectCharacterEditorButtonClicked()
        {
            CharacterSelectionMenu.DeleteUnimplementedUI();
            CharacterSelectionMenu.SetClonedCharacter();
            CharacterSelectionMenu.InitializeCharacterEditorWithSelectedCharacter();
            CameraManager.EnableCamera();
            GameObject canvasInGameMenu = GameObject.Find("CanvasInGameMenu").gameObject;
            canvasInGameMenu.transform.GetChild(CharacterEditorManager.hierarchyIndex).gameObject.SetActive(true);
            CharacterSelectionMenu.InitializeTitleOfCharacterEditor();  //何故かここに置かないとタイトル変わらない？SetActiveするたびにタイトルが初期化されるみたいです。
        }

        private static void SetButtonOnClickEvent(GameObject selectCharacterEditor_Button)
        {
            Button selectCharacterEditor_ButtonComponent = selectCharacterEditor_Button.GetComponent<Button>();
            selectCharacterEditor_ButtonComponent.onClick = new Button.ButtonClickedEvent();
            selectCharacterEditor_ButtonComponent.onClick.AddListener(OnSelectCharacterEditorButtonClicked);
        }
    }
}
