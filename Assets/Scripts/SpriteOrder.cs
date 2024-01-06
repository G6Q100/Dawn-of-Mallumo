using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrder : MonoBehaviour
{
    SpriteRenderer[] spriteRenderer;
    int order;

    void Start()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        foreach (var renderer in spriteRenderer)
        {
            order++;
            renderer.sortingOrder = order;
        }
    }

}
