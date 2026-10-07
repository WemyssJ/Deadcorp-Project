using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return null;
            }

            if (instance == null)
            {
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
#endif
            return instance;
        }
    }
    public Player Player { get; set; }
    public PlayerUnlocks PlayerUnlocks { get; set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        /*if (Keyboard.current.ctrlKey.wasPressedThisFrame)
        {
            SaveSystem.Save();
            //SaveSystemEncrypted.Save();
        }

        if (Keyboard.current.altKey.wasPressedThisFrame)
        {
            SaveSystem.Load();
            //SaveSystemEncrypted.Load();
        }*/
    }

    public void Save()
    {
        SaveSystem.Save();
    }
}
