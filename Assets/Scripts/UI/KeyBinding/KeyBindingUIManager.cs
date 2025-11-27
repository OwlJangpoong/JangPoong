using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyBindingUIManager : MonoBehaviour
{
    public TMP_InputField jumpKeyInputField;
    public TMP_InputField slideKeyInputField;
    public TMP_InputField runKeyInputField;
    public TMP_InputField leftKeyInputField;
    public TMP_InputField rightKeyInputField;

    // private KeyBindingManager keyBindingManager;

    // ??? ?????? ???
    private string nextSceneName = "1-1 tutorial";

    public void OnButtonClick()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // private void Start()
    // {
        // keyBindingManager = KeyBindingManager.Instance;

        // �ʱⰪ ����
        // jumpKeyInputField.text = Managers.KeyBind.jumpKeyCode.ToString();
        // slideKeyInputField.text = Managers.KeyBind.slideKeyCode.ToString();
        // runKeyInputField.text = Managers.KeyBind.runKeyCode.ToString();
        // leftKeyInputField.text = Managers.KeyBind.leftKeyCode.ToString();
        // rightKeyInputField.text = Managers.KeyBind.rightKeyCode.ToString();

        // jumpKeyInputField.onEndEdit.AddListener(SetJumpKey);
        // slideKeyInputField.onEndEdit.AddListener(SetSlideKey);
        // //runKeyInputField.onEndEdit.AddListener(SetRunKey);
        // leftKeyInputField.onEndEdit.AddListener(SetLeftKey);
        // rightKeyInputField.onEndEdit.AddListener(SetRightKey);
    // }
    //
    // private KeyCode GetKeyCodeFromName(string keyName)
    // {
    //     KeyCode keyCode;
    //     string keyNameUpper = keyName.ToUpper();
    //     if (System.Enum.TryParse(keyNameUpper, out keyCode) && System.Enum.IsDefined(typeof(KeyCode), keyCode))
    //     {
    //         return keyCode;
    //     }
    //     Debug.LogWarning($"Invalid key name: {keyName}. Defaulting to KeyCode.None.");
    //     return KeyCode.None;
    // }
    //
    //
    // private void SetJumpKey(string key)
    // {
    //     KeyCode newKey = GetKeyCodeFromName(key);
    //     if (newKey != KeyCode.None)
    //     {
    //         Managers.KeyBind.jumpKeyCode = newKey;
    //         Managers.KeyBind.SaveKeyBinding();
    //     }
    //     else
    //     {
    //         jumpKeyInputField.text = Managers.KeyBind.jumpKeyCode.ToString();
    //     }
    // }
//
//     private void SetSlideKey(string key)
//     {
//         KeyCode newKey = GetKeyCodeFromName(key);
//         if (newKey != KeyCode.None)
//         {
//             Managers.KeyBind.slideKeyCode = newKey;
//             Managers.KeyBind.SaveKeyBinding();
//         }
//         else
//         {
//             slideKeyInputField.text = Managers.KeyBind.slideKeyCode.ToString();
//         }
//     }
//
//     /*    private void SetRunKey(string key)
//         {
//             KeyCode newKey = GetKeyCodeFromName(key);
//             if (newKey != KeyCode.None)
//             {
//                 Managers.KeyBind.runKeyCode = newKey;
//                 Managers.KeyBind.SaveKeyBindings();
//             }
//             else
//             {
//                 runKeyInputField.text = Managers.KeyBind.runKeyCode.ToString();
//             }
//         }*/
//
//     private void SetLeftKey(string key)
//     {
//         KeyCode newKey = GetKeyCodeFromName(key);
//         if (newKey != KeyCode.None)
//         {
//             Managers.KeyBind.leftKeyCode = newKey;
//             Managers.KeyBind.SaveKeyBinding();
//         }
//         else
//         {
//             leftKeyInputField.text = Managers.KeyBind.leftKeyCode.ToString();
//         }
//     }
//
//     private void SetRightKey(string key)
//     {
//         KeyCode newKey = GetKeyCodeFromName(key);
//         if (newKey != KeyCode.None)
//         {
//             Managers.KeyBind.rightKeyCode = newKey;
//             Managers.KeyBind.SaveKeyBinding();
//         }
//         else
//         {
//             rightKeyInputField.text = Managers.KeyBind.rightKeyCode.ToString();
//         }
//     }
}
