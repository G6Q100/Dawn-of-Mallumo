using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RockWorm : ThrowPos
{
    float modeCD, cutsceneTime = 4;
    string mode = "Rest";
    [SerializeField] GameObject rock, dust;

    bool triggered, cutsceneEnd;

    [SerializeField] AttackRange attackRange;

    Animator anim;
    GameObject player;

    int rand;
    float speed = 30;
    Rigidbody2D rb2;

    [SerializeField] Transform leftAttackPos, rightAttackPos, leftPos, rightPos, middlePos;

    int repeatCount = 2;

    [SerializeField] Animator bossHPAnim;
    HP hp;

    int phase = 0;
    [SerializeField] PolygonCollider2D polyCollider;
    [SerializeField] BoxCollider2D jumpCollider;

    bool dead;

    void Start()
    {
        if (GameManager.instance.boss1Down)
            gameObject.SetActive(false);

        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb2 = GetComponent<Rigidbody2D>();
        hp = GetComponent<HP>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
        if (dead)
        {
            DustOff();
            GameManager.instance.rumblingSound.Stop();
            return; 
        }

        if (dust.activeInHierarchy)
        {
            if (!GameManager.instance.rumblingSound.isPlaying)
                GameManager.instance.rumblingSound.Play();
        }
        else 
            GameManager.instance.rumblingSound.Stop();

        if (!attackRange.triggered && !triggered && attackRange.gameObject.activeInHierarchy)
        {
            return;
        }
        attackRange.enabled = false;

        if (cutsceneTime > 3)
        {
            CamController.instance.playCutscene = true;
        }
        else if (cutsceneTime > 1)
        {
            dust.SetActive(true);
            CamController.instance.ShakeCam(0.1f, 0.1f);
        }
        else
        {
            if (!triggered)
            {
                polyCollider.enabled = true;
                modeCD = 2;
                anim.enabled = true;
                triggered = true;
                anim.SetTrigger("StartFight");
                CamController.instance.ShakeCam(0.1f, 0.1f);
            }
        }
        if (cutsceneTime > 0)
        {
            player.GetComponent<Player>().lostControl = 0.1f;
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);
            cutsceneTime -= Time.deltaTime;
            return;
        }

        if (!cutsceneEnd)
        {
            cutsceneEnd = true;
            GameManager.instance.boss1Fighting = true;
            GameManager.instance.boss1BGM.Play();
            GameManager.instance.boss1BGM.volume = 0.6f;
            bossHPAnim.SetBool("BossUp", true);
        }

        CamController.instance.playCutscene = false;

        AttackPattern();
    }

    void FacingPlayer()
    {
        if (player.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-0.9f, 1f, 1);
        else
            transform.localScale = new Vector3(0.9f, 1f, 1);
    }

    void AttackPattern()
    {
        if (hp.hp >= 18)
        {
            phase = 0;
            speed = 30;
        }
        else if (hp.hp >= 12)
        {
            phase = 1;
            speed = 35;
        }
        else
        {
            phase = 2;
            speed = 40;
        }

        if (modeCD > -1)
        {
            modeCD -= Time.deltaTime;
        }

        switch (mode)
        {
            case "Attack":
                FacingPlayer();
                anim.SetTrigger("RockAttack");
                modeCD = 3.2f;
                mode = "Rest";
                break;
            case "SwimDown":
                FacingPlayer();
                anim.SetTrigger("SwimDown");
                break;
            case "DashToRight":
                dust.transform.position = transform.position + new Vector3(0, 0.5f);
                if(transform.position.x < 70)
                {
                    CamController.instance.ShakeCam(0.1f, 0.1f);
                    rb2.velocity = Vector2.right * speed;
                }
                else
                {
                    rb2.velocity = Vector2.zero;
                    DustOff();
                    if (phase != 2)
                    {
                        StartCoroutine(SwimUp());
                        modeCD = 3.5f;
                        mode = "Rest";
                    }
                    else
                    {
                        StartCoroutine(JumpAttack());
                        modeCD = 6;
                        mode = "Rest";
                    }
                }
                break;
            case "DashToLeft":
                dust.transform.position = transform.position + new Vector3(0, 0.5f);
                if (transform.position.x > 8)
                {
                    CamController.instance.ShakeCam(0.1f, 0.1f);
                    rb2.velocity = Vector2.left * speed;
                }
                else
                {
                    rb2.velocity = Vector2.zero;
                    DustOff();
                    if (phase != 2)
                    {
                        StartCoroutine(SwimUp());
                        modeCD = 3.5f;
                        mode = "Rest";
                    }
                    else
                    {
                        StartCoroutine(JumpAttack());
                        modeCD = 6;
                        mode = "Rest";
                    }
                }
                break;
            case "Rest":
                anim.ResetTrigger("SwimDown");
                FacingPlayer();
                if (modeCD > 0)
                    break;

                if (repeatCount > 0)
                {
                    repeatCount--;
                    mode = "Attack";
                }
                else
                {
                    mode = "SwimDown";
                    rand = Random.Range(1, 4);
                    repeatCount = rand;
                }
                break;
        }
    }

    public void RockAttack()
    {
        for (int i = phase + 1; i > 0; i--)
        {
            rand = Random.Range(0, stats.Length);

            while (stats[rand].used)
            {
                if (rand == 0)
                {
                    rand = 4;
                }
                else
                {
                    rand--;
                }
            }

            if (player.transform.position.x > transform.position.x)
            {
                RockAttack oRock = Instantiate(rock, transform.position + Vector3.left * -4.5f, Quaternion.identity).GetComponent<RockAttack>();
                oRock.right = true;
                oRock.rb2d.gravityScale = stats[rand].gravity;
                oRock.rb2d.velocity = new Vector2(oRock.rb2d.velocity.x, stats[rand].Yvelocity);
                transform.localScale = Vector3.one * Random.Range(0.25f, 0.45f);
                oRock.speed = stats[rand].speed;

            }
            else
            {
                RockAttack oRock = Instantiate(rock, transform.position + Vector3.left * 4.5f, Quaternion.identity).GetComponent<RockAttack>();
                oRock.right = false;
                oRock.rb2d.gravityScale = stats[rand].gravity;
                oRock.rb2d.velocity = new Vector2(oRock.rb2d.velocity.x, stats[rand].Yvelocity);
                transform.localScale = Vector3.one * Random.Range(0.25f, 0.45f);
                oRock.speed = stats[rand].speed;
            }
            stats[rand].used = true;
        }

        foreach(ThrowStats stat in stats)
            stat.used = false;
    }

    public IEnumerator DashAttack()
    {
        yield return new WaitForSeconds(1);
        rand = Random.Range(0, 101);
        if (rand > 50)
        {
            CamController.instance.ShakeCam(1, 0.1f);
            transform.position = leftAttackPos.position;
            yield return new WaitForSeconds(1);
            mode = "DashToRight";
            anim.SetTrigger("DashAttack");
            DustOn();
        }
        else
        {
            CamController.instance.ShakeCam(1, 0.1f);
            transform.position = rightAttackPos.position;
            yield return new WaitForSeconds(1);
            mode = "DashToLeft";
            anim.SetTrigger("DashAttack");
            DustOn();
        }
    }

    IEnumerator JumpAttack()
    {
        polyCollider.enabled = false;
        transform.position = new Vector2(player.transform.position.x, -16.5f);
        dust.transform.position = transform.position + new Vector3(0, 5f);
        DustOn();
        yield return new WaitForSeconds(1.2f);
        jumpCollider.enabled = true;
        anim.SetTrigger("Jump");
        yield return new WaitForSeconds(1.6f);
        StartCoroutine(SwimUp());
        modeCD = 3.5f;
        mode = "Rest";
    }

    IEnumerator SwimUp()
    {
        polyCollider.enabled = false;
        anim.SetTrigger("Hiding");
        yield return new WaitForSeconds(1);
        rand = Random.Range(0, 101); 
        if (rand > 50)
        {
            transform.position = leftPos.position;
            dust.transform.position = transform.position + new Vector3(0, -1.5f);
            DustOn();
        }
        else
        {
            transform.position = rightPos.position;
            dust.transform.position = transform.position + new Vector3(0, -1.5f);
            DustOn();
        }
        CamController.instance.ShakeCam(1, 0.1f);
        yield return new WaitForSeconds(0.5f);
        polyCollider.enabled = true;
        anim.SetTrigger("StartFight");
    }

    public void JumpEnd()
    {
        jumpCollider.enabled = false;
    }

    public void DustOn()
    {
        dust.SetActive(true);
    }

    public void DustOff()
    {
        dust.SetActive(false);
    }
    public void DeathStart()
    {
        DustOff();
        dead = true;
        GameManager.instance.boss1Fighting = false;
        rb2.velocity = Vector2.zero;
        polyCollider.enabled = false;
        jumpCollider.enabled = false;
        CamController.instance.playCutscene = true;

    }

    public void Death()
    {
        CamController.instance.playCutscene = false;
        GameManager.instance.boss1Down = true;
        gameObject.SetActive(false);
    }
}
