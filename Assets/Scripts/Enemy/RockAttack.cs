using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RockAttack : MonoBehaviour
{
    public Rigidbody2D rb2d;
    [SerializeField] public float speed = 5, startSpeed;

    float lifetime = 10;

    [HideInInspector] public bool right;
    [SerializeField] bool gravity;

    bool inWater;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        startSpeed = speed;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            gameObject.SetActive(false);
        }
        transform.Rotate(0, 0, Time.deltaTime * 360);
    }

    void FixedUpdate()
    {
        if (right)
        {
            rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
        }
        else
        {
            rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
        }
    }
}
