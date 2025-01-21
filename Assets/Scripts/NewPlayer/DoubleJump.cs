using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 600f, speed = 5f;
    float moveX;
    Rigidbody2D rb;

    bool doubleJumpState = false;
    public bool isGround = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (rb.velocity.y == 0)
            isGround = true;
        else
            isGround = false;

        if (isGround)
            doubleJumpState = true;

        if (isGround && Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
            JumpAddForce();
        else if (doubleJumpState && Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
        {
            JumpAddForce();
            doubleJumpState = false;
        }

        moveX = Input.GetAxis("Horizontal") * speed;
        rb.velocity = new Vector2(moveX, rb.velocity.y);
    }

    void JumpAddForce()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce);
    }

}
