using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CharacterEditor
{
    public class CharacterEditorButtonHandler : MonoBehaviour
    {

        public static void OnButton_YesClicked()
        {
            CommonButtonActions();
            CharacterEditorManager.SetCreatedToOriginalCharacter();
        }

        public static void OnButton_CloseClicked()
        {
            CommonButtonActions();
        }
        static void CommonButtonActions()
        {
            CameraManager.DisableCamera();
            GameObject canvasInGameMenu = GameObject.Find("CanvasInGameMenu").gameObject;
            canvasInGameMenu.transform.GetChild(CharacterEditorManager.hierarchyIndex).gameObject.SetActive(false);
            CharacterEditorManager.DestroyGameObjectIfExists("CHARNEWGAME");
        }
    }
}
