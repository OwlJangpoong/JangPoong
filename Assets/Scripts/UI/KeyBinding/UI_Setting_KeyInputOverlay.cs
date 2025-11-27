using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.KeyBinding
{
    public class UI_Setting_KeyInputOverlay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;

        public string infoMessage = "키를 눌러 새로운 단축키를 설정하세요.";
        public string errorMessage = "잘못된 키입니다. 다시 입력하세요.";
        public string duplicateMessage = "이미 설정된 키입니다. 다른 키를 입력하세요.";

        private void OnEnable()
        {
            ShowMessage(infoMessage);
        }


        public void ShowMessage(string message)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
        }

        public void HideMessage()
        {
            messageText.gameObject.SetActive(false);
        }
    }
}