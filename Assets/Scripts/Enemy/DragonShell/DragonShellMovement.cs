using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonShellMovement : MonoBehaviour
{
    float speed = 2, cutsceneTime = 10;
    Rigidbody2D rb2;
    float waterPosY, waterSpeed, sinRange = 1, sinSpeed = 2.5f;
    int moveDir;
    [SerializeField] Transform[] MovePath;
    [SerializeField] bool MoveLeft, phase2;
    [SerializeField] Animator bossHPAnim;

    [SerializeField] GameObject claw, dust;
    [SerializeField] GameObject[] clawArms;
    GameObject player, parentObject;

    ClawMovement clawmovement;
    HP hp;
    Vector2 refVector2;

    [SerializeField] AttackRange startBossRange;
    [SerializeField] HeadMovement leftHead, middleHead, rightHead;
    [SerializeField] ClawMovement[] claws;
    bool bossStarted, cutsceneEnd;

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        clawmovement = claw.GetComponent<ClawMovement>();
        hp = GetComponent<HP>();
        parentObject = transform.parent.parent.gameObject;
    }   
    private void Update()
    {
        if (!startBossRange.triggered)
            return;

        if (!bossStarted)
        {
            bossStarted = true;
            cutsceneTime = 8;
        }
        if (cutsceneTime > 0)
            cutsceneTime -= Time.deltaTime;

        if (!startBossRange.triggered && !bossStarted && startBossRange.gameObject.activeInHierarchy)
        {
            return;
        }

        startBossRange.enabled = false;

        if (Mathf.Abs(-6.5f - parentObject.transform.position.y) > 0.2f)
        {
            foreach (Transform child in parentObject.transform)
                child.transform.position = parentObject.transform.position;

            CamController.instance.playCutscene = true;
            parentObject.transform.position += Vector3.up * 8 * Time.deltaTime;
            player.GetComponent<Player>().lostControl = float.MaxValue;
            player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            cutsceneTime = 2;
            return;
        }

        if (cutsceneTime > 2)
        {
            CamController.instance.playCutscene = true;
        }
        else if (!leftHead.enabled)
        {
            leftHead.enabled = true;
            middleHead.enabled = true;
            rightHead.enabled = true;
            foreach (ClawMovement oclaw in claws)
                oclaw.enabled = true;
        }

        if (cutsceneTime > 0)
        {
            player.GetComponent<Player>().lostControl = float.MaxValue;
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);
            cutsceneTime -= Time.deltaTime;
            return;
        }

        if (!cutsceneEnd)
        {
            cutsceneEnd = true;
            leftHead.loading = true;
            middleHead.loading = true;
            rightHead.loading = true;

            player.GetComponent<Player>().lostControl = 0;
            leftHead.GetComponent<CircleCollider2D>().enabled = true;
            middleHead.GetComponent<CircleCollider2D>().enabled = true;
            rightHead.GetComponent<CircleCollider2D>().enabled = true;

            CamController.instance.playCutscene = false;
            bossHPAnim.SetBool("BossUp", true);
            GameManager.instance.boss2Fighting = true;
            GameManager.instance.boss2BGM.Play();
            GameManager.instance.boss1BGM.volume = 0.6f;
        }

        CamController.instance.playCutscene = false;

        if (!cutsceneEnd)
            return;

        waterPosY = Mathf.Sin(waterSpeed) * sinRange;
        waterSpeed += Time.deltaTime * sinSpeed;
        if (waterSpeed > 360)
            waterSpeed = 0;
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

        MovePattern();
    }

    void MovePattern()
    {
        if (hp.hp > 30)
            return;
        clawmovement.enabled = true;
    }

    void FixedUpdate()
    {
        if (hp.hp <= 15)
        {
            if (transform.position.y > -21)
            {
                waterSpeed = 0;
                rb2.velocity = Vector2.SmoothDamp(rb2.velocity, new Vector2(rb2.velocity.x, -20), ref refVector2, 0.3f);
                return;
            }
        }    

        rb2.velocity = new Vector2(moveDir * speed, waterPosY);
    }
}
