using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBallAttack : MonoBehaviour
{
    public Vector3 lookPos;

    [HideInInspector] public Vector3 playerPos;

    Rigidbody2D rb2d;
    [SerializeField] float speed;

    float lifetime = 5f;

    [SerializeField] GameObject explosion;
    public bool ingoreWall;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0.5f)
        {
            transform.localScale = Vector3.one * (lifetime * 2 + 0.1f);
        }
        if (lifetime <= 0)
        {
            lifetime = 5f;
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Ground")
            return;

        if (ingoreWall)
            return;

        if (lifetime > 0.2f)
        {
            lifetime = 0.2f;
        }
    }
}
