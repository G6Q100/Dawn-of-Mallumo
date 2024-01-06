using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class TestThrow : ThrowPos
{
    [SerializeField] GameObject rock;
    float time;

    void Update()
    {
        time -= Time.deltaTime;

        if (time >= 0)
            return;

        time = 0.5f;

        foreach (ThrowStats stat in stats)
        {
            RockAttack oRock = Instantiate(rock, transform.position, Quaternion.identity).GetComponent<RockAttack>();
            oRock.right = false;
            oRock.rb2d.gravityScale = stat.gravity;
            oRock.rb2d.velocity = new Vector2(oRock.rb2d.velocity.x, stat.Yvelocity);
            oRock.speed = stat.speed;
        }
    }
}
