using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            // switch to scene
            SceneController.instance.NextLevel();

            //resets scene - WORKING
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
    }
}
