using System;
using System.Data;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    float speed;

    [HideInInspector] public float moveDir = 0;

    int jumpTime;
    float jumpDelay, jumpCD;
    bool onAir;

    public float lostControl, iframe;
    bool isfloating;

    Rigidbody2D rb2;
    public bool moveable;

    Animator anim;
    bool attacking, startDashing, dashEnd;
    float dashCD, dashing, attackCD;
    int dashType;

    Vector3[] attackRot;
    [SerializeField] GameObject fireBall;
    [SerializeField] Transform[] attackPos;

    HP hp;
    float scale;

    bool inWater;

    void Start()
    {
        moveable = true;
        speed = 5;
        attackRot = new Vector3[2];

        rb2 = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        hp = GetComponent<HP>();
        scale = Mathf.Abs(transform.localScale.x);
        lostControl = 2;
    }

    private void Update()
    {
        if (iframe > 0)
        {
            iframe -= Time.deltaTime;
        }
        if (lostControl > 0)
        {
            moveable = false;
            anim.SetBool("Jump", false);
            anim.SetBool("Walk", false);
            lostControl -= Time.deltaTime;
        }
        else
            moveable = true;
        if (!moveable)
            return;
        CheckGround();


        Movement();
        Jump();
        Attack();
        if (dashing <= 0)
        {
            GetComponent<TrailRenderer>().enabled = false;
            WalkAnim();
        }
        else
            GetComponent<TrailRenderer>().enabled = true;

        if (GameManager.instance.boss1Down)
            Dash();
    }

    void FixedUpdate()
    {
        rb2.velocity = new Vector2(rb2.velocity.x, Mathf.Clamp(rb2.velocity.y, -8, float.MaxValue));

        if (lostControl > 0)
        {
            lostControl -= Time.deltaTime;
            return;
        }
        if (attackCD > 0)
            attackCD -= Time.deltaTime;
        else if(attacking)
        {
            attacking = false;
        }

        if (dashing > 0)
        {
            switch (dashType)
            {
                case 1:
                    rb2.velocity = Vector2.right * speed * 2.5f;
                    break;
                case 2:
                    rb2.velocity = Vector2.left * speed * 2.5f;
                    break;
                case 3:
                    rb2.velocity = Vector2.up * speed * 2.5f;
                    break;
                case 4:
                    rb2.velocity = Vector2.down * speed * 2.5f;
                    break;
            }
            dashing -= Time.deltaTime;
            dashEnd = true;
            return;
        }

        rb2.velocity = new Vector2(moveDir * speed, rb2.velocity.y);
    }

    void WalkAnim()
    {
        if (moveDir < 0)
        {
            transform.localScale = new Vector3(-scale, scale, scale);
        }
        else if (moveDir != 0)
        {
            transform.localScale = Vector3.one * scale;
        }

        if (Mathf.Approximately(moveDir, 0))
        {
            anim.SetBool("Walk", false);
        }
        else
        {
            anim.SetBool("Walk", true);
        }
    }

    public void Movement()
    {
        moveDir = Input.GetAxis("Horizontal");
    }

    void CheckGround()
    {
        if (jumpDelay >= 0.2f)
        {
            jumpTime = 0;
        }

        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position + Vector3.down * (transform.localScale.y - 0.24f),
            transform.position + Vector3.down * transform.localScale.y);

        if (jumpDelay < 0.2f)
            jumpDelay += Time.deltaTime;

        onAir = true;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform == null)
            {
                continue;
            }
            if (hit.transform.tag != "Ground" && hit.transform.tag != "DropDown")
            {
                continue;
            }
            onAir = false;
        }

        if (jumpCD > 0)
        {
            jumpCD -= Time.deltaTime;
            onAir = true;
        }

        if (!isfloating && onAir && !inWater)
        {
            anim.SetTrigger("StartJump");
            anim.SetBool("Jump", true);
            isfloating = true;
        }

        if (inWater)
            onAir = false;

        if (onAir)
            return;

        if(jumpTime != 1)
            GameManager.instance.onGroundSound.Play();

        anim.SetBool("Jump", false);
        isfloating = false;

        jumpTime = 1;

        jumpDelay = 0;
        onAir = false;
    }

    public void Jump()
    {
        if (jumpTime > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.jumpSound.Play();
            jumpTime--;
            jumpDelay = 0.3f;
            jumpCD = 0.2f;
            anim.SetTrigger("StartJump");
            anim.SetBool("Jump", true);
            isfloating = true;
            rb2.velocity = new Vector2(rb2.velocity.x, 8f);
        }
    }

    void Dash()
    {
        if (dashEnd)
        {
            dashEnd = false;
            startDashing = false;
            rb2.velocity = new Vector2(rb2.velocity.x, 1);
            anim.SetTrigger("EndDash");
        }

        if (dashCD > 0)
        {
            dashCD -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.K) && !startDashing && dashCD <= 0)
        {
            dashCD = 1;
            startDashing = true;

            if (Input.GetKey(KeyCode.W))
            {
                anim.SetTrigger("UpDash");
                return;
            }
            if (Input.GetKey(KeyCode.S))
            {
                anim.SetTrigger("DownDash");
                return;
            }
            anim.SetTrigger("Dash");
        }
    }

    public void DashEvent(int mode)
    {
        dashing = 0.3f;
        dashCD = 1;
        jumpTime--;
        jumpDelay = 0.3f;
        onAir = true;
        isfloating = true;
        GameManager.instance.dashSound.Play();
        if (mode == 1)
        {
            if (transform.localScale.x > 0)
            {
                dashType = 1;
            }
            else
            {
                dashType = 2;
            }
        }
        else if (mode == 2)
        {
            dashType = 3;
        }
        else 
        {
            dashType = 4; 
        }
    }

    void NotOnGround()
    {
        anim.SetTrigger("StartJump");
        anim.SetBool("Jump", true);
        isfloating = true;
        jumpTime--;
        jumpDelay = 0.3f;
        onAir = true;
    }

    void Attack()
    {
        if (Input.GetKey(KeyCode.J) && !attacking)
        {
            attacking = true;
            attackCD = 5;

            if (Input.GetKey(KeyCode.W))
            {
                anim.SetTrigger("UpAttack");
                return;
            }

            if (Input.GetKey(KeyCode.S))
            {
                anim.SetTrigger("DownAttack");
                return;
            }

            anim.SetTrigger("Attack");
        }
    }

    public void AttackEvent(int mode)
    {
        GameManager.instance.fireBallSound.Play();
        switch (mode)
        {
            case 0:
                if (transform.localScale.x > 0)
                {
                    attackRot[0] = Vector3.right;
                }
                else
                {
                    attackRot[0] = Vector3.left;
                    attackRot[1] += Vector3.up * 180;
                }
                break;
            case 1:
                attackRot[0] = Vector3.up;
                attackRot[1] += Vector3.forward * 90;
                break;
            case 2:
                attackRot[0] = Vector3.down;
                attackRot[1] += Vector3.back * 90;
                break;
        }
        attackCD = 0.2f;

        if (transform.localScale.x < 1)
        {
            attackRot[1] += Vector3.up * 180;
        }

        if (transform.localScale.x < 1)
        {
            attackRot[1] += Vector3.up * 180;
        }

        GameObject oFireBall = Instantiate(fireBall, attackPos[mode].position, Quaternion.Euler(attackRot[1]));
        oFireBall.GetComponent<FireBall>().lookPos = attackRot[0];
        attackRot[1] = Vector3.zero;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            inWater = true;
        }
        if ((collision.tag == "Enemy" || collision.tag == "EnemyBullet") && iframe <= 0)
        {
            hp.TakeDamage(1);
            NotOnGround();
            anim.SetTrigger("GetHit");
            CamController.instance.ShakeCam(0.3f, 0.1f);

            Vector2 lookPos = collision.transform.position - transform.position;
            rb2.velocity = new Vector2(-lookPos.x, 1f).normalized * speed * 1.5f;
            lostControl = 1;
            iframe = 1;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Water")
        {
            rb2.velocity = new Vector2(rb2.velocity.x, rb2.velocity.y / 1.5f);
            inWater = false;
            isfloating = true;
            jumpTime--;
            jumpDelay = 0.3f;
            onAir = true;
        }
    }
}
