using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawMovement : MonoBehaviour
{
    [SerializeField] public Vector3 targetPos, startPos;
    Vector2 lookPos;
    public float speed = 20, angle;
    [SerializeField] float offset;
    Rigidbody2D rb2;
    [SerializeField] bool attack;
    public bool notFacingDir;
    GameObject player;
    public string mode = "WithDraw";
    float modeCD = 0;

    [SerializeField] GameObject anchor, dust;
    [SerializeField] GameObject[] neckparts;
    Vector2 targetDir;

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (!attack)
            return;
        AttackPattern();
    }
    void AttackPattern()
    {
        if (modeCD > -1)
        {
            modeCD -= Time.deltaTime;
        }

        switch (mode)
        {
            case "StabAttack":
                if (!dust.activeInHierarchy)
                {
                    dust.SetActive(true);
                    dust.transform.position = new Vector3(50, player.transform.position.y);
                }

                transform.position = dust.transform.position + Vector3.right * 5;
                foreach (GameObject neck in neckparts)
                    neck.transform.position = dust.transform.position + Vector3.right * 5;
                anchor.transform.position = dust.transform.position + Vector3.right * 5;

                notFacingDir = false;
                startPos = transform.position;
                targetPos = startPos;
                CamController.instance.ShakeCam(1.5f, 0.2f);
                modeCD = 1.5f;
                mode = "LoadingAttack";
                break;
            case "LoadingAttack":
                if (modeCD <= 0)
                {
                    mode = "AttackStart";
                }
                break;
            case "AttackStart":
                speed = 15;
                targetPos = new Vector3(-2.7f, transform.position.y);
                dust.SetActive(false);
                if (transform.position.x <= -2.7f)
                {
                    rb2.velocity = Vector2.zero;
                    modeCD = 1;
                    mode = "Delay";
                    CamController.instance.ShakeCam(0.5f, 0.15f);
                    return;
                }
                break;
            case "Delay":
                if (modeCD <= 0)
                {
                    mode = "WithDraw";
                }
                break;
            case "WithDraw":
                notFacingDir = true;
                speed = 12;
                targetPos = new Vector3(55f, transform.position.y);
                if (transform.position.x >= 55f)
                {
                    modeCD = Random.Range(3f, 5f);
                    mode = "StabAttack";
                    return;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (Vector2.Distance(targetPos, transform.position) <= 0.2f && !attack)
        {
            rb2.velocity = Vector2.zero;
            return;
        }

        lookPos = targetPos - transform.position;
        rb2.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;

        if (notFacingDir)
            return;

        angle = Mathf.Atan2(rb2.velocity.y, rb2.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);

        if (!attack)
            return;

        if (dust.activeInHierarchy)
        {
            if (!GameManager.instance.rumblingSound.isPlaying)
                GameManager.instance.rumblingSound.Play();
        }
        else
            GameManager.instance.rumblingSound.Stop();
    }
}
