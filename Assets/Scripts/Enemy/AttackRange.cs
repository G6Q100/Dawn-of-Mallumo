using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public bool triggered, stayAcitve;
    float triggering;

    private void Update()
    {

        if (triggering > 0)
        {
            triggered = true;
            if (stayAcitve)
                return;
            triggering -= Time.deltaTime;
        }
        else
            triggered = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        triggering = float.MaxValue;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.tag != "Player")
            return;

        triggering = 0;
    }
}
