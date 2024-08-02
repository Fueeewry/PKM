using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(string targetScene)
    {
        Debug.Log("changing scene to : " + targetScene);
        SceneManager.LoadScene(targetScene);
    }

    public void QuitApplication()
    {
        Debug.Log("Application quitted");
        Application.Quit();
    }
}
