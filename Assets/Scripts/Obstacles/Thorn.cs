using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorn : MonoBehaviour
{
    [SerializeField] private float damage = 0.5f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatsController>().OnAttacked(damage);
        }
    }
}
