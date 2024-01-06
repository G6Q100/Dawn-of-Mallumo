using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public int hp;
    [SerializeField] public int startHp;

    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject[] parentObjects;
    [SerializeField] HP core;
    [SerializeField] bool parts, boss, boss2;
    [SerializeField] Slider HPSlider;

    private void Start()
    {
        hp = startHp;
        if (HPSlider == null)
            return;
        HPSlider.maxValue = hp;
        HPSlider.value = hp;
    }

    public void TakeDamage(int damage, [Optional] Vector3 target, [Optional] float speed)
    {
        if (parts)
        {
            core.TakeDamage(damage);
            return;
        }

        GameManager.instance.gethitSound.Play();
        hp -= damage;
        if (HPSlider != null)
            HPSlider.value = hp;

        if (hp <= 0)
        {
            GameManager.instance.gethitSound.Play();
            Instantiate(hitEffect, transform.position + Vector3.back, Quaternion.identity);

            if (boss)
            {
                HPSlider.GetComponent<Animator>().SetBool("BossUp", false);
                GetComponent<Animator>().SetTrigger("BossDeath");
                enabled = false;
                return;
            }
            if (boss2)
            {
                GameManager.instance.boss2Down = true;
                GameManager.instance.boss2Fighting = false;
                HPSlider.GetComponent<Animator>().SetBool("BossUp", false);
                foreach (GameObject parentObject in parentObjects)
                    parentObject.SetActive(false);
            }

            if (GameManager.instance.rumblingSound.isPlaying)
                GameManager.instance.rumblingSound.Stop();

            gameObject.SetActive(false);
        }
    }
}
