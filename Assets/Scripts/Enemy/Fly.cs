using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Fly : MonoBehaviour
{
    float speed, modeCD;
    string mode = "Rest";
    Rigidbody2D rb2;

    [SerializeField] AttackRange attackRange;
    Vector3 spawnPos, targetPos, lookPos;

    GameObject target;

    RaycastHit2D[][] hit;

    bool hitGround;

    public float damagedCD;

    void Start()
    {
        speed = 3.5f;

        rb2 = GetComponent<Rigidbody2D>();
        spawnPos = transform.position;
        target = GameObject.FindGameObjectWithTag("Player");
        hit = new RaycastHit2D[4][];
    }

    private void Update()
    {
        MovePattern();
    }

    void FixedUpdate()
    {
        lookPos = targetPos - transform.position;

        if (damagedCD <= 0)
            rb2.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;

        DetectObject();
    }

    void MovePattern()
    {
        if (damagedCD > 0)
            damagedCD -= Time.deltaTime;

        if (!attackRange.triggered)
        {
            targetPos = spawnPos;
            if (Mathf.Approximately(Vector2.Distance(targetPos, transform.position), 0))
            {
                modeCD = 1.5f;
                mode = "Rest";
            }
            return;
        }

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
                    modeCD = 1.5f;
                    mode = "Rest";
                }
                if (Mathf.Approximately(Vector2.Distance(targetPos, transform.position), 0))
                {
                    modeCD = 1.5f;
                    mode = "Rest";
                }
                break;
            case "Rest":
                targetPos = transform.position;
                if (modeCD <= 0)
                {
                    modeCD = 2;
                    mode = "Attack";
                }
                break;
        }
    }

    public void DetectObject()
    {
        hit[0] = Physics2D.LinecastAll(transform.position + Vector3.up * 0.55f + Vector3.left * 0.75f, transform.position + Vector3.up * 0.55f - Vector3.left * 0.75f);
        hit[1] = Physics2D.LinecastAll(transform.position - Vector3.up * 1.1f + Vector3.left * 0.75f, transform.position - Vector3.up * 1.1f - Vector3.left * 0.75f);
        hit[2] = Physics2D.LinecastAll(transform.position + Vector3.left * 0.9f + Vector3.up * 0.5f, transform.position + Vector3.left * 0.9f - Vector3.up * 0.5f);
        hit[3] = Physics2D.LinecastAll(transform.position - Vector3.left * 0.9f + Vector3.up * 0.5f, transform.position - Vector3.left * 0.9f - Vector3.up * 0.5f);

        for (int i = 0; i < hit[i].Length; i++)
        {
            hitGround = false;

            foreach (RaycastHit2D ohit in hit[i])
            {
                if (ohit.transform == null)
                {
                    continue;
                }
                if (ohit.transform.tag == "Ground")
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

