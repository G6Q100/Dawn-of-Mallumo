using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public Vector3 lookPos;

    [HideInInspector] public Vector3 playerPos;

    Rigidbody2D rb2d;
    [SerializeField] float speed;

    float lifetime = 1.5f;
    [SerializeField] int damage, enemyType;
    [SerializeField] bool ignoreWall;

    [SerializeField] GameObject explosion;

    void OnEnable()
    {
        float rotZ = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

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
            lifetime = 1.5f;
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(lookPos.x, lookPos.y).normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player")
            return;

        if (ignoreWall && collision.tag == "Ground")
            return;

        if (collision.tag != "Enemy" && collision.tag != "Ground")
            return;

        if (collision.tag == "Enemy" && collision.GetComponent<HP>() != null)
        {
            collision.GetComponent<HP>().TakeDamage(1, gameObject.transform.position, 10);
            collision.GetComponent<Animator>().SetTrigger("GetHit");
            GameManager.instance.explosionSound.Play();

            if (collision.GetComponent<FlyBug>() != null)
            {
                collision.GetComponent<FlyBug>().damagedCD = 0.15f;
                collision.GetComponent<Rigidbody2D>().velocity = rb2d.velocity / 1.5f;
            }
            if (collision.GetComponent<Fly>() != null)
            {
                collision.GetComponent<Fly>().damagedCD = 0.15f;
                collision.GetComponent<Rigidbody2D>().velocity = rb2d.velocity / 1.5f;
            }
        }
        else
            GameManager.instance.fireBallSound.Play();

        Instantiate(explosion, transform.position + Vector3.back, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
