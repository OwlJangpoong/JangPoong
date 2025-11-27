using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button_KeyReset : MonoBehaviour
{
    private Button keyRestBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        keyRestBtn = GetComponent<Button>();
        keyRestBtn.onClick.RemoveAllListeners();
        keyRestBtn.onClick.AddListener(ResetKeyBinding);
        
    }

    private void ResetKeyBinding()
    {
        Managers.KeyBind.ResetKeyBinding();
    }

    
}
