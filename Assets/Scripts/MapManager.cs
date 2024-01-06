using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [SerializeField] GameObject mapImage;
    [SerializeField] SpriteRenderer[] maps;
    bool[] mapActivate;

    int currentScene;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            mapActivate = new bool[maps.Length];
            CheckMap();
            instance = this;
        }
        else
        {
            instance.CheckMap();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!mapImage.activeInHierarchy)
                mapImage.SetActive(true);
            else
                mapImage.SetActive(false);
        }
    }

    void CheckMap()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        mapActivate[currentScene - 1] = true;

        for (int i = 0; i < maps.Length; i++)
        {
            if (i == currentScene - 1)
            {
                maps[i].color = Color.green;
                maps[i].sortingOrder = 10;
            }
            else if (mapActivate[i])
            {
                maps[i].color = Color.white;
                maps[i].sortingOrder = 1;
            }
            else
            {
                maps[i].color = Color.clear;
                maps[i].sortingOrder = 1;
            }
        }
    }
}
