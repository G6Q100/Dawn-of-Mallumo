using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBall : MonoBehaviour
{
    [SerializeField] Transform[] attackPos;
    [SerializeField] GameObject attackBallAttack;
    Vector3 attackRot;

    private void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * 60);
    }

    public void Attack()
    {
        foreach (Transform attack in attackPos)
        {
            attackRot = attack.position - transform.position;

            GameObject oWaterBall = Instantiate(attackBallAttack, attack.position, Quaternion.Euler(attackRot));
            oWaterBall.GetComponent<WaterBallAttack>().lookPos = attackRot;
        }
    }
}
