using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [SerializeField] Transform[] MovePath;
    float speed, turnTime, waterSpeed, waterPosY;
    [Range(1, 15)][SerializeField] float sinSpeed;
    [Range(1, 8)][SerializeField] float sinRange;
    int moveDir;
    [SerializeField]bool MoveLeft, camping, inWater;

    string mode;

    Rigidbody2D rb2;

    Animator anim;

    [SerializeField]AttackRange attackRange;

    GameObject player;

    void Start()
    {
        speed = 7f;

        anim = GetComponent<Animator>();
        rb2 = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (camping)
            mode = "Camping";
        else
            mode = "Searching";

        anim.SetBool("InWater", inWater);

        turnTime = 2;
    }

    private void Update()
    {
        MovePattern();

        if (mode == "Attacking" && inWater)
        {
            waterPosY = Mathf.Sin(waterSpeed) * sinRange;
            waterSpeed += Time.deltaTime * sinSpeed;
            if (waterSpeed > 360)
                waterSpeed = 0;
        }
        else
            waterPosY = 0;
    }

    void FixedUpdate()
    {
        if (inWater)
            rb2.velocity = new Vector2(moveDir * speed * 0.75f, waterPosY);
        else
            rb2.velocity = new Vector2(moveDir * speed, 0);
    }

    void MovePattern()
    {
        if (turnTime > 0)
            turnTime -= Time.deltaTime;

        switch (mode)
        {
            case "Camping":
                anim.SetBool("Running", false);
                if (attackRange.triggered)
                {
                    Spotted();
                }
                break;
            case "Searching":
                anim.SetBool("Running", false);
                if (turnTime <= 0)
                {
                    if (MoveLeft)
                    {
                        MoveLeft = false;
                        if (!inWater)
                            transform.localScale = new Vector3(0.3f, 0.3f, 1);
                        else
                            transform.localScale = new Vector3(0.3f, 0.3f, 1);

                    }
                    else
                    {
                        MoveLeft = true;
                        if (!inWater)
                            transform.localScale = new Vector3(-0.3f, 0.3f, 1);
                        else
                            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
                    }
                    turnTime = 2;
                }
                if (attackRange.triggered)
                    Spotted();
                break;
            case "Attacking":
                anim.SetBool("Running", true);
                if (MoveLeft)
                {
                    moveDir = -1;
                    if (transform.position.x <= MovePath[0].position.x)
                    {
                        MoveLeft = false;
                        if (!inWater)
                            transform.localScale = new Vector3(0.3f, 0.3f, 1);
                        else
                            transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    }
                }
                else
                {
                    moveDir = 1;
                    if (transform.position.x >= MovePath[1].position.x)
                    {
                        MoveLeft = true;
                        if (!inWater)
                            transform.localScale = new Vector3(-0.3f, 0.3f, 1);
                        else
                            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
                    }
                }
                break;
        }
    }

    public void Spotted()
    {
        if (mode != "Attacking")
        {
            if (player.transform.position.x > transform.position.x)
            {
                MoveLeft = false;
                if (!inWater)
                    transform.localScale = new Vector3(0.3f, 0.3f, 1);
                else
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
            }
            else
            {
                MoveLeft = true;
                if (!inWater)
                    transform.localScale = new Vector3(-0.3f, 0.3f, 1);
                else
                    transform.localScale = new Vector3(-0.5f, 0.5f, 1);
            }
            mode = "Attacking";
        }
    }
}
