using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    [SerializeField] Transform[] MovePath;
    float speed;
    int moveDir;
    bool MoveLeft, triggered, jumped;

    [SerializeField] AttackRange attackRange;

    Rigidbody2D rb2;

    void Start()
    {
        speed = 4.5f;

        rb2 = GetComponent<Rigidbody2D>();

        MoveLeft = false;
    }

    private void Update()
    {
        if (!attackRange.triggered && !triggered)
        {
            return;
        }

        if (!triggered)
        {
            GetComponent<HP>().enabled = true;
            rb2.velocity = Vector2.zero;
            triggered = true;
        }

        if (transform.position.y > -21.3f && !jumped)
        {
            rb2.gravityScale += Time.deltaTime * 10;
        }
        else if (transform.position.y > -21.3f)
        {
            rb2.gravityScale =  1f;
            MovePattern();
        }
        else
        {
            jumped = true;
            rb2.gravityScale = 0;
            rb2.velocity = new Vector2(rb2.velocity.x, 5f);
            MovePattern();
        }
    }

    void FixedUpdate()
    {
        rb2.velocity = new Vector2(moveDir * speed, rb2.velocity.y);
    }

    void MovePattern()
    {
        if (MoveLeft)
        {
            moveDir = -1;
            if (transform.position.x <= MovePath[0].position.x)
            {
                MoveLeft = false;
            }
        }
        else
        {
            moveDir = 1;
            if (transform.position.x >= MovePath[1].position.x)
            {
                MoveLeft = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        if(MoveLeft)
            MoveLeft = false;
        else
            MoveLeft = true;
    }
}
