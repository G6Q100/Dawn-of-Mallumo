using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SmallRockWorm : ThrowPos
{
    float modeCD;
    string mode = "Wondering";
    [SerializeField] GameObject rock;
    [SerializeField] AttackRange[] attackRange;

    Animator anim;
    GameObject player;

    int rand;
    float speed = 2;
    Rigidbody2D rb2;

    [SerializeField] Transform[] MovePath;
    [SerializeField]bool MoveLeft;
    bool walking, inRange;
    int moveDir;

    int temp;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb2 = GetComponent<Rigidbody2D>();
        walking = true;
    }

    private void Update()
    {
        AttackPattern();
    }

    void FixedUpdate()
    {
        if (mode != "Wondering")
        {
            rb2.velocity = Vector2.zero;
            return;
        }

        rb2.velocity = new Vector2(moveDir * speed, 0);
    }

    void FacingPlayer()
    {
        if (player.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-0.4f, 0.4f, 1);
        else
            transform.localScale = new Vector3(0.4f, 0.4f, 1);
    }

    void AttackPattern()
    {
        if (modeCD > -1)
        {
            modeCD -= Time.deltaTime;
        }

        switch (mode)
        {
            case "Attacking":
                FacingPlayer();
                anim.SetTrigger("RockAttack");
                modeCD = 2.5f;
                mode = "Waiting";
                break;
            case "Waiting":
                FacingPlayer();
                if (modeCD > 0)
                    break;
                inRange = false;
                foreach (AttackRange range in attackRange)
                {
                    if (range.triggered)
                    {
                        inRange = true;
                        break;
                    }
                }
                if (inRange)
                    mode = "Attacking";
                else
                    mode = "Wondering";
                break;
            case "Wondering":
                inRange = false;
                foreach (AttackRange range in attackRange)
                {
                    if (range.triggered)
                    {
                        inRange = true;
                        break;
                    }
                }
                if (inRange)
                {
                    walking = false;
                    anim.SetBool("Wondering", false);
                    mode = "Attacking";
                    break;
                }
                if (modeCD > 0)
                {
                    walking = false;
                    anim.SetBool("Wondering", false);
                }
                else
                    walking = true;

                if (!walking)
                    break;
                anim.SetBool("Wondering", true);
                if (MoveLeft)
                {
                    moveDir = -1;
                    transform.localScale = new Vector3(0.4f, 0.4f, 1);
                    if (transform.position.x <= MovePath[0].position.x)
                    {
                        MoveLeft = false;
                        walking = false;
                        moveDir = 0;
                        modeCD = 1.5f;
                    }
                }
                else
                {
                    moveDir = 1;
                    transform.localScale = new Vector3(-0.4f, 0.4f, 1);
                    if (transform.position.x >= MovePath[1].position.x)
                    {
                        MoveLeft = true;
                        walking = false;
                        moveDir = 0;
                        modeCD = 1.5f;
                    }
                }
                break;
        }
    }

    public void RockAttack()
    {
        for (int i = 1; i <= attackRange.Length; i++)
        {
            if (!attackRange[attackRange.Length - i].triggered)
                continue;
            if (temp == 0)
                temp = attackRange.Length - i;
                break;
        }


        if (temp == 0)
            rand = 1;
        else
            rand = temp;

        if (player.transform.position.x > transform.position.x)
        {
            RockAttack oRock = Instantiate(rock, transform.position + Vector3.left * -3f, Quaternion.identity).GetComponent<RockAttack>();
            oRock.right = true;
            oRock.rb2d.gravityScale = stats[rand].gravity;
            oRock.rb2d.velocity = new Vector2(oRock.rb2d.velocity.x, stats[rand].Yvelocity);
            oRock.speed = stats[rand].speed;

        }
        else
        {
            RockAttack oRock = Instantiate(rock, transform.position + Vector3.left * 3f, Quaternion.identity).GetComponent<RockAttack>();
            oRock.right = false;
            oRock.rb2d.gravityScale = stats[rand].gravity;
            oRock.rb2d.velocity = new Vector2(oRock.rb2d.velocity.x, stats[rand].Yvelocity);
            oRock.speed = stats[rand].speed;
        }
        stats[rand].used = true;

        rand = Random.Range(0, stats.Length);
        while (stats[rand].used)
        {
            if (rand == 0)
            {
                rand = 3;
            }
            else
            {
                rand--;
            }
        }

        if (player.transform.position.x > transform.position.x)
        {
            RockAttack oRock = Instantiate(rock, transform.position + Vector3.left * -3f, Quaternion.identity).GetComponent<RockAttack>();
            oRock.right = true;
            oRock.rb2d.gravityScale = stats[rand].gravity;
            oRock.rb2d.velocity = new Vector2(oRock.rb2d.velocity.x, stats[rand].Yvelocity);
            oRock.speed = stats[rand].speed;

        }
        else
        {
            RockAttack oRock = Instantiate(rock, transform.position + Vector3.left * 3f, Quaternion.identity).GetComponent<RockAttack>();
            oRock.right = false;
            oRock.rb2d.gravityScale = stats[rand].gravity;
            oRock.rb2d.velocity = new Vector2(oRock.rb2d.velocity.x, stats[rand].Yvelocity);
            oRock.speed = stats[rand].speed;
        }

        foreach (ThrowStats stat in stats)
            stat.used = false;
    }
    public void Spotted()
    {
        if (mode != "Attacking" && mode != "Waiting")
        {
            walking = false;
            anim.SetBool("Wondering", false);
            mode = "Attacking";
        }
    }
}
