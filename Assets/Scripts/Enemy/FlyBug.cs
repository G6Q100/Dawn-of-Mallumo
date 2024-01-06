using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FlyBug : MonoBehaviour
{
    float speed, modeCD;
    string mode = "Rest";
    Rigidbody2D rb2;

    [SerializeField] AttackRange attackRange;
    Vector3 targetPos, lookPos, attackRot;
    [SerializeField] Transform attackPos;

    GameObject target;
    [SerializeField] GameObject attackBallAttack;

    RaycastHit2D[][] hit;

    bool hitGround, spotted;
    [SerializeField] bool bigFlyBug;

    Animator anim;

    float flySpeed, flyPos;
    [Range(1, 15)][SerializeField] float sinSpeed;
    [Range(1, 8)][SerializeField] float sinRange;

    public float damagedCD;

    void Start()
    {
        speed = 2.5f;

        rb2 = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        hit = new RaycastHit2D[10][];
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        MovePattern();
        if (bigFlyBug)
        {
            flyPos = Mathf.Sin(flySpeed) * sinRange;
            flySpeed += Time.deltaTime * sinSpeed;
            if (flySpeed > 360)
                flySpeed = 0;
        }
        else
            flyPos = 0;
    }

    void FixedUpdate()
    {
        if (bigFlyBug)
        {
            rb2.velocity = new Vector2(flyPos, 0);
            return;
        }

        if (!spotted)
            return;

        lookPos = targetPos - transform.position;

        if (damagedCD > 0)
        {
            DetectObject();
            return;
        }

        if (mode != "Rest")
            rb2.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;
        else
            rb2.velocity = Vector2.zero;

        DetectObject();
    }

    void FacingPlayer()
    {
        if (bigFlyBug)
        {
            if (target.transform.position.x > transform.position.x)
                transform.localScale = new Vector3(-0.6f, 0.6f, 1);
            else
                transform.localScale = new Vector3(0.6f, 0.6f, 1);
            return;
        }

        if (target.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-0.2f, 0.2f, 1);
        else
            transform.localScale = new Vector3(0.2f, 0.2f, 1);
    }

    void MovePattern()
    {
        FacingPlayer();

        if (damagedCD > 0)
            damagedCD -= Time.deltaTime;

        if (attackRange.triggered)
            spotted = true;

        if (!spotted)
            return;

        if (modeCD > -1)
        {
            modeCD -= Time.deltaTime;
        }

        switch (mode)
        {
            case "Attack":
                targetPos = target.transform.position;
                if (modeCD <= 0)
                {
                    mode = "RangeAttack";
                }
                anim.ResetTrigger("Attack");
                break;
            case "RangeAttack":
                anim.SetTrigger("Attack");
                modeCD = 2f;
                mode = "Rest";
                break;
            case "Rest":
                if (modeCD <= 0)
                {
                    if (!bigFlyBug)
                        modeCD = 3f;
                    else
                        modeCD = 0.5f;
                    mode = "Attack";
                }
                break;
        }
    }

    public void Attack()
    {
        attackRot = target.transform.position - transform.position;
        GameObject oWaterBall = Instantiate(attackBallAttack, attackPos.position, Quaternion.Euler(attackRot));
        oWaterBall.GetComponent<WaterBallAttack>().lookPos = attackRot;
        if (!bigFlyBug)
            return;
        oWaterBall.transform.localScale = new Vector3(1.5f, 1.5f, 1);

        var direction = Quaternion.Euler(0, 0, 30) * (target.transform.position - transform.position);
        attackRot = attackRot + direction;
        oWaterBall = Instantiate(attackBallAttack, attackPos.position, Quaternion.Euler(attackRot));
        oWaterBall.GetComponent<WaterBallAttack>().lookPos = attackRot;
        oWaterBall.transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }

    public void DetectObject()
    {
        hit[0] = Physics2D.LinecastAll(transform.position + Vector3.up * 0.55f + Vector3.left * 0.75f, transform.position + Vector3.up * 0.55f - Vector3.left * 0.75f);
        hit[1] = Physics2D.LinecastAll(transform.position - Vector3.up * 1.1f + Vector3.left * 0.75f, transform.position - Vector3.up * 1.1f - Vector3.left * 0.75f);
        hit[2] = Physics2D.LinecastAll(transform.position + Vector3.left * 0.9f + Vector3.up * 0.5f, transform.position + Vector3.left * 0.9f - Vector3.up * 0.5f);
        hit[3] = Physics2D.LinecastAll(transform.position - Vector3.left * 0.9f + Vector3.up * 0.5f, transform.position - Vector3.left * 0.9f - Vector3.up * 0.5f);

        for (int i = 0; i < 4; i++)
        {
            hitGround = false;

            foreach (RaycastHit2D ohit in hit[i])
            {
                if (ohit.transform == null)
                {
                    continue;
                }
                if (ohit.transform.tag == "Ground" || ohit.transform.tag == "Water")
                {
                    hitGround = true;
                    break;
                }
            }
            if (!hitGround)
                continue;

            switch (i)
            {
                case 0:
                    rb2.velocity = new Vector2(rb2.velocity.x, Mathf.Clamp(rb2.velocity.y, float.MinValue, 0)).normalized * speed;
                    break;
                case 1:
                    rb2.velocity = new Vector2(rb2.velocity.x, Mathf.Clamp(rb2.velocity.y, 0, float.MaxValue)).normalized * speed;
                    break;
                case 2:
                    rb2.velocity = new Vector2(Mathf.Clamp(rb2.velocity.x, 0, float.MaxValue), rb2.velocity.y).normalized * speed;
                    break;
                case 3:
                    rb2.velocity = new Vector2(Mathf.Clamp(rb2.velocity.x, float.MinValue, 0), rb2.velocity.y).normalized * speed;
                    break;
            }
        }
    }
}
