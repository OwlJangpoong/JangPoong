using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public abstract class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer _interactSprite;

    [SerializeField]private Transform _playerTransform;

    private const float INTERACTIVE_DISTANCE = 4F;
    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame && isWithInInteractDistance())
        {
            //interact
            Interact();
        }

        if (_interactSprite.gameObject.activeSelf && !isWithInInteractDistance())
        {
            //turn off the sprite
            _interactSprite.gameObject.SetActive(false);
        }

        else if (!_interactSprite.gameObject.activeSelf && isWithInInteractDistance())
        {
            //turn on the sprite
            _interactSprite.gameObject.SetActive(true);
        }
    }

    public abstract void Interact();

    private bool isWithInInteractDistance()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) < INTERACTIVE_DISTANCE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
