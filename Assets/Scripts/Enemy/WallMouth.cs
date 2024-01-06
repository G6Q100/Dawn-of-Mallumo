using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WallMouth : MonoBehaviour
{
    float modeCD;
    string mode = "Rest";
    
    [SerializeField] int dir;

    [SerializeField] AttackRange attackRange;
    Animator anim;

    [SerializeField] GameObject rock;
    [SerializeField] bool right;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!attackRange.triggered)
        {
            return;
        }

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
            case "Attack":
                anim.SetTrigger("Attack");
                modeCD = 3.5f;
                mode = "Rest";
                break;
            case "Rest":
                if (modeCD <= 0)
                {
                    mode = "Attack";
                }
                break;
        }
    }

    public void RockAttack()
    {
        GameObject oFireBall = Instantiate(rock, transform.position + Vector3.left * dir, Quaternion.identity);
        oFireBall.GetComponent<RockAttack>().right = right;
    }
}
