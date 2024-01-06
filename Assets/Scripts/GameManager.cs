using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject player;

    public static GameManager instance;

    [SerializeField] GameObject objectPoolPos;

    bool restart;

    public Vector3 spawnPoint;

    public bool startArea = true;

    public bool boss1Down, boss2Down, playerDown, boss1Fighting, boss2Fighting;

    public bool healthItem, healthItem2, healthItem3, healthItem4;

    [SerializeField] public AudioSource gethitSound, fireBallSound, explosionSound,
        jumpSound, onGroundSound, dashSound, rumblingSound, menuBGM, area1Sound, boss1BGM, boss2BGM;

    public bool[] HpItem;

    private void Awake()
    {
        HpItem = new bool[5];

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player");

        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1 && spawnPoint == Vector3.zero)
        {
            spawnPoint = player.transform.position;
        }
        else
        {
            instance.player = player;
            instance.restart = false;

            player.transform.position = instance.spawnPoint;
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            menuBGM.volume = 0;
            if (!menuBGM.isPlaying)
            {
                menuBGM.Play();
                menuBGM.volume = 0;
            }
            return;
        }
        restart = false;
        player.transform.position = spawnPoint;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        CheckBGM();

        if (player == null)
            return;

        if (player.activeInHierarchy)
            return;

        if (!restart)
        {
            restart = true;
            playerDown = true;
            StartCoroutine(Restart());
        }
    }

    IEnumerator Restart()
    {

        yield return new WaitForSeconds(1);
        boss1Fighting = false;
        boss2Fighting = false;
        playerDown = false;
        NextLevel.instance.Loading();
    }
    
    void CheckBGM()
    {
        if (boss1BGM.volume <= 0)
            boss1BGM.Stop();

        if (boss2BGM.volume <= 0)
            boss2BGM.Stop();

        if (area1Sound.volume <= 0)
            area1Sound.Stop();

        if ((!boss1Fighting && boss1BGM.isPlaying) || (SceneManager.GetActiveScene().buildIndex != 7 && boss1BGM.isPlaying))
        {
            boss1BGM.volume -= Time.deltaTime * 2;
        }

        if (boss1BGM.volume <= 0)
            boss1BGM.Stop();

        if (boss2BGM.volume <= 0)
            boss2BGM.Stop();

        if (area1Sound.volume <= 0)
            area1Sound.Stop();

        if ((!boss2Fighting && boss2BGM.isPlaying) || (SceneManager.GetActiveScene().buildIndex != 13 && boss2BGM.isPlaying))
        {
            boss2BGM.volume -= Time.deltaTime * 2;
        }

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (menuBGM.volume > 0)
                menuBGM.volume -= Time.deltaTime;
        }
        else if (menuBGM.volume < 0.6f)
        {
            menuBGM.volume += Time.deltaTime * 0.3f;
        }

        if (SceneManager.GetActiveScene().buildIndex > 1 && SceneManager.GetActiveScene().buildIndex != 7
            && SceneManager.GetActiveScene().buildIndex != 13)
        {
            if (!area1Sound.isPlaying)
            {
                area1Sound.volume = 0.1f;
                area1Sound.Play();
            }
            else if (area1Sound.volume < 1)
            {
                area1Sound.volume += Time.deltaTime * 0.3f;
            }
        }
        else
        {
            if (area1Sound.volume > 0 || area1Sound.isPlaying)
                area1Sound.volume -= Time.deltaTime;
        }
    }
}
