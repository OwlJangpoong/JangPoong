using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison_Area : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerStatsController>();
            if (player != null)
            {
                player.ApplyPoison(1.5f, 0.5f, 3f);
            }
        }
    }
}