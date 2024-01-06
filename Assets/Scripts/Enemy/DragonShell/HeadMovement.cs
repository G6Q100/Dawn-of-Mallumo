using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HeadMovement : MonoBehaviour
{
    [SerializeField] Vector3 targetPos;
    Vector3 goPos;
    Vector2 lookPos, targetVelocity;
    float speed = 6, angle;
    [SerializeField] float offset;
    Rigidbody2D rb2;
    [SerializeField] Transform body;
    [SerializeField] bool middleHead;

    GameObject player;
    Vector3 playerDir;

    string mode = "Rest";
    [SerializeField] float modeCD;
    Animator anim;

    Vector3 attackRot;
    [SerializeField] Transform attackPos;
    [SerializeField] GameObject attackBallAttack;
    public bool loading;

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

    }

    private void Update()
    {
        if (!loading) { return; }
        MovePattern();
    }

    void FixedUpdate()
    {
        goPos = body.position + (targetPos / 2);
        lookPos = goPos - transform.position;
        targetVelocity = new Vector2(lookPos.x, lookPos.y) * speed;
        rb2.velocity = Vector2.SmoothDamp(rb2.velocity, targetVelocity, ref lookPos, 0.1f);

        if (!middleHead)
        {
            FacingPlayer();
            return;
        }

        playerDir = player.transform.position - transform.position;
        angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
    }

    void MovePattern()
    {
        FacingPlayer();

        if (modeCD > -1)
        {
            modeCD -= Time.deltaTime;
        }

        switch (mode)
        {
            case "Attack":
                mode = "RangeAttack";
                anim.ResetTrigger("Attack");
                break;
            case "RangeAttack":
                anim.SetTrigger("Attack");
                modeCD = 9f;
                mode = "Rest";
                break;
            case "Rest":
                if (modeCD <= 0)
                {
                    mode = "RangeAttack";
                }
                break;
        }
    }
    public void Attack()
    {
        attackRot = player.transform.position - transform.position;
        GameObject oWaterBall = Instantiate(attackBallAttack, attackPos.position, Quaternion.Euler(attackRot));
        oWaterBall.GetComponent<WaterBallAttack>().lookPos = attackRot;
        oWaterBall.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        oWaterBall.GetComponent<WaterBallAttack>().ingoreWall = true;
    }

    void FacingPlayer()
    {
        if (player.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(3, 3, 1);
        else
            transform.localScale = new Vector3(-3, 3, 1);
    }

}
