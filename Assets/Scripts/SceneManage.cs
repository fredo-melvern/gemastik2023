using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    static SceneManage instance;

    static int level = 1;

    void Start()
    {
        // handle duplicates
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
       
    }

    public static SceneManage GetInstance()
    {
        return instance;
    }
    void Update()
    {
        
    }
    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public static void SetLevel(int lvl)
    {
        level = lvl;
        Debug.Log("now " + SceneManage.level);
    }
    public static int GetLevel()
    {
        return level;
    }
    public static void NextLevelSceneManage()
    {
        FindObjectOfType<GameManager>().NextLevel();
    }

}
