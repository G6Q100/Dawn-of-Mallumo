using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] public int level;
    Animator anim;

    public static NextLevel instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Loading()
    {
        anim.SetTrigger("Transition");
    }

    void LoadingLevel()
    {
        SceneManager.LoadScene(level);
        anim.SetTrigger("Transition");
    }
}
