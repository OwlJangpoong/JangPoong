using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForInvisible : MonoBehaviour
{
    private Collider2D _collider2D;

    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.layer = (int)Define.Layer.PlayerDamaged;
        other.GetComponent<PlayerStatsController>().IsInvisible = true;
    }

    
}
