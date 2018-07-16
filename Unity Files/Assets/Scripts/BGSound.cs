using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGSound : MonoBehaviour {

    static bool AudioBegin = false;
    void Awake()
    {
        if (!AudioBegin)
        {
            GetComponent<AudioSource>().Play();
            DontDestroyOnLoad(gameObject);
            AudioBegin = true;
        }
    }
    void Update()
    {
        /*if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
        {
            GetComponent<AudioSource>().Stop();
            AudioBegin = false;
        }*/
    }
}
