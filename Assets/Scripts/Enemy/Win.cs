using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    [SerializeField] GameObject boss;
    float loading;
    [SerializeField] public int level;

    private void Update()
    {
        if (boss.activeInHierarchy)
            return;
        if (GameManager.instance.playerDown)
            return;

        loading += Time.deltaTime;

        if (loading > 1f)
        {
            loading = -1421;
            NextLevel.instance.level = level;
            NextLevel.instance.Loading();
        }
    }
}
