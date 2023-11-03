using HarmonyLib;
using UnityEngine;

namespace CharacterEditor
{
    public class CameraManager: MonoBehaviour
    {

        static public GameObject CharacterEditorCamera;

        public static void InitializeCamera()
        {
            GameObject originalCamera = GameObject.Find("CameraNewGame").gameObject;
            CharacterEditorCamera = GameObject.Instantiate(originalCamera);
            CharacterEditorCamera.name = "CharacterEditorCamera";
            DontDestroyOnLoad(CharacterEditorCamera);
        }

        static public void EnableCamera()
        {
            if (CharacterEditorCamera)
            {
                CharacterEditorCamera.SetActive(true);
            }
        }

        static public void DisableCamera()
        {
            if (CharacterEditorCamera)
            {
                CharacterEditorCamera.SetActive(false);
            }
        }
    }
}
