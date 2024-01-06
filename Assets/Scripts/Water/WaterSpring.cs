using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterSpring : MonoBehaviour
{
    public float velocity = 0;
    public float force = 0;
    [SerializeField] public float height = 0;
    [SerializeField] public float targetHeight = 0;
    int waterIndex;
    SpriteShapeController spriteShapeController;
    float resistance = 120;

    public void Init(SpriteShapeController ssc)
    {
        var index = transform.GetSiblingIndex();
        waterIndex = index + 1;

        spriteShapeController = ssc;
        velocity = 0;
        height = transform.localPosition.y;
        targetHeight = transform.localPosition.y;
    }

    public void WaterPointUpdate()
    {
        if (spriteShapeController != null)
        {
            Spline waterSpline = spriteShapeController.spline;
            Vector3 waterPositon = waterSpline.GetPosition(waterIndex);
            waterSpline.SetPosition(waterIndex, new Vector3(waterPositon.x,
                transform.localPosition.y, waterPositon.z));
        }
    }

    public void WaterSpringUpdate(float springForce, float dampening)
    {
        height = transform.localPosition.y;

        var x = height - targetHeight;
        var loss = -dampening * velocity;

        force = -springForce * x + loss;
        velocity += force;
        var y = transform.localPosition.y;

        transform.localPosition = new Vector3(transform.localPosition.x,
            y + velocity, transform.localPosition.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        var speed = collision.gameObject.GetComponent<Rigidbody2D>().velocity;

        velocity += speed.y / resistance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "FireBall")
            return;

        if (collision.GetComponent<Rigidbody2D>().gravityScale != 0)
            collision.GetComponent<Rigidbody2D>().gravityScale /= 1.25f;

        var speed = collision.gameObject.GetComponent<Rigidbody2D>().velocity;

        collision.gameObject.GetComponent<Rigidbody2D>().velocity /= 1.25f;

        velocity += speed.y / resistance;
    }
}
