using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticTest : MonoBehaviour
{
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent <Button> ();
        button.onClick.AddListener(TestStatistic);
    }

    void TestStatistic()
    {
        Managers.Data.SetSlotNum(1);
        
        Managers.Scene.LoadScene("Statistic_Temp");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
