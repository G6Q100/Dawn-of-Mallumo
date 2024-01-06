using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameObject player;
    private Vector3 movePoint, refVector3;
    [SerializeField] private float threshold;
    [SerializeField] private Vector2 maxPoint, minPoint;

    private float shakeTime;
    private float shakeForce;
    private float shakeReduce;

    public static CamController instance;

    public bool boss, startedFight, stopFollow;
    [SerializeField] AttackRange RangeCheck;
    [SerializeField] Transform targetPos;
    public bool playCutscene;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            if (shakeTime <= shakeForce)
            {
                shakeForce -= shakeReduce;
            }
            if (shakeForce <= 0)
                shakeForce = 0;

            transform.position += new Vector3(Random.Range(-shakeForce, shakeForce),
                Random.Range(-shakeForce, shakeForce), 0);
        }

        if (player == null)
            return;

        if (!player.activeInHierarchy)
            return;

        if (stopFollow)
            return;

        if (RangeCheck != null)
        {
            if (RangeCheck.triggered && Camera.main.fieldOfView < 90)
            {
                Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, 90, ref shakeTime, 1);
            }
            if (!RangeCheck.triggered && Camera.main.fieldOfView > 60)
            {
                Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, 60, ref shakeTime, 0.5f);
            }
        }

        if (playCutscene)
        {
            movePoint = targetPos.transform.position;
        }
        else
        {
            movePoint = player.transform.position;
        }

        movePoint = new Vector2(Mathf.Clamp(movePoint.x, minPoint.x, maxPoint.x),
            Mathf.Clamp(movePoint.y, minPoint.y, maxPoint.y));

        movePoint.z = -10;

        transform.position = Vector3.SmoothDamp(transform.position, movePoint, ref refVector3, Mathf.Sqrt(0.05f));
    }

    public void ShakeCam(float theShakeTime, float theShakeForce)
    {
        shakeTime = theShakeTime;
        shakeForce = theShakeForce;

        shakeReduce = shakeForce * shakeTime;
    }
}
