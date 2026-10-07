using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private void Start()
    {

    }

    // Update is called once per frame
    

    public void Play()
    {
        SceneController.instance.FirstLevel();

    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
