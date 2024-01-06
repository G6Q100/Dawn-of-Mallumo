using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ThrowStats
{
    public float speed;
    public float Yvelocity;
    public float gravity;
    public bool used;
}

public class ThrowPos : MonoBehaviour
{
    public ThrowStats[] stats;
}
