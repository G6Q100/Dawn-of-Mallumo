using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelWall : MonoBehaviour
{
    GameObject player;
    [SerializeField] int sceneNum;
    [SerializeField] Vector3 spawnPoint;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;

        player.GetComponent<Player>().lostControl = 2;
        GameManager.instance.spawnPoint = spawnPoint;
        NextLevel.instance.level = sceneNum;
        NextLevel.instance.Loading();
    }
}
