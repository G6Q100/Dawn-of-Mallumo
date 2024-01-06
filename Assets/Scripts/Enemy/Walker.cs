using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField] Transform[] MovePath;
    float speed;
    int moveDir;
    bool MoveLeft;

    Rigidbody2D rb2;

    Animator anim;

    void Start()
    {
        speed = 4;

        anim = GetComponent<Animator>();
        rb2 = GetComponent<Rigidbody2D>();

        MoveLeft = false;
    }

    private void Update()
    {
        MovePattern();
    }

    void FixedUpdate()
    {
        rb2.velocity = new Vector2(moveDir * speed, 0);
    }

    void MovePattern()
    {
        if (MoveLeft)
        {
            moveDir = -1;
            if (transform.position.x <= MovePath[0].position.x)
            {
                MoveLeft = false;
                anim.SetTrigger("ChangeDir");
            }
        }
        else
        {
            moveDir = 1;
            if (transform.position.x >= MovePath[1].position.x)
            {
                MoveLeft = true;
                anim.SetTrigger("ChangeDir");
            }
        }
    }
}
