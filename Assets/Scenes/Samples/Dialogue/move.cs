using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class move : MonoBehaviour
{
    public float speed = 100f;
    public float JumpHeight;
    public bool InAir = false;

    private Rigidbody2D rb2d;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        InAir = false;
        Debug.Log("InAir false");
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        InAir = true;
        Debug.Log("InAir True");
    }

    void FixedUpdate() {
        Vector2 NoMovement = new Vector2(0f, 0f);

        float moveHorizontal = Input.GetAxis("Horizontal");
        if (moveHorizontal > 0)
        {
            {
                rb2d.velocity = new Vector2(speed, rb2d.velocity.y);

            }
        }
        if (moveHorizontal < 0)
        {
            rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.W) || (Input.GetKeyDown(KeyCode.UpArrow))) {
            if (InAir == false)
            {
                rb2d.AddForce(new Vector2(0, JumpHeight), ForceMode2D.Impulse);
            }
        }
    }

}

