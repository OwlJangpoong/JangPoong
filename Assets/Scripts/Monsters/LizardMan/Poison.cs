using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    private void Start()
    {
        //Destroy(gameObject, 1.5f); // 발사 후 1.5초 뒤 자동 파괴
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
