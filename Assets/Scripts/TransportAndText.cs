using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransportAndText : MonoBehaviour
{
    bool dropDown = false, triggered = false, activted = false;
    [SerializeField] bool goDown, cutscene, textOnly, bossTrigger;
    [SerializeField] GameObject player, floor, boss;
    [SerializeField] int sceneNum;
    [SerializeField] Vector3 spawnPoint;

    [SerializeField] TextMeshPro useText;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (bossTrigger && boss.activeInHierarchy)
            useText.color = new Color(1, 1, 1, 0);
    }

    private void FixedUpdate()
    {
        if (bossTrigger && boss.activeInHierarchy)
            return;

        if (triggered && !activted)
        {
            useText.color = new Color(useText.color.r, useText.color.g, useText.color.b, 
                Mathf.Clamp(useText.color.a + Time.deltaTime * 2, 0, 1));

            if (Input.GetKey(KeyCode.E) && !textOnly)
            {
                activted = true;
                StartCoroutine(Dropping());
            }
        }
        else
        {
            useText.color = new Color(useText.color.r, useText.color.g, useText.color.b,
                Mathf.Clamp(useText.color.a - Time.deltaTime * 2, 0, 1));
        }

        if (textOnly)
            return;

        if (cutscene && GameManager.instance.startArea)
        {
            if (floor.transform.position.y > -45.3f)
            {
                floor.transform.position = floor.transform.position + Vector3.down * Time.deltaTime * 5;
                player.transform.position = player.transform.position + Vector3.down * Time.deltaTime * 5;
                player.GetComponent<Player>().lostControl = 0.1f;
            }
            else
            {
                cutscene = false;
                GetComponent<BoxCollider2D>().enabled = true;
            }
        }
        else if (cutscene)
        {
            floor.transform.position = new Vector3(floor.transform.position.x, -45.3f, floor.transform.position.z);
            cutscene = false;
            GetComponent<BoxCollider2D>().enabled = true;
        }

        if (!dropDown)
            return;

        if (goDown)
        {
            floor.transform.position = floor.transform.position + Vector3.down * Time.deltaTime * 3;
            player.transform.position = player.transform.position + Vector3.down * Time.deltaTime * 3;
        }
        else
        {
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            floor.transform.position = floor.transform.position + Vector3.up * Time.deltaTime * 3;
            player.transform.position = player.transform.position + Vector3.up * Time.deltaTime * 3;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        triggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        triggered = false;
    }

    IEnumerator Dropping()
    {
        player.GetComponent<Player>().lostControl = 8;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);
        yield return new WaitForSeconds(1);
        CamController.instance.stopFollow = true;
        dropDown = true;
        yield return new WaitForSeconds(1);
        GameManager.instance.spawnPoint = spawnPoint;
        if (SceneManager.GetActiveScene().buildIndex == 1)
            GameManager.instance.startArea = true;
        else
            GameManager.instance.startArea = false;
        NextLevel.instance.level = sceneNum;
        NextLevel.instance.Loading();
    }
}
