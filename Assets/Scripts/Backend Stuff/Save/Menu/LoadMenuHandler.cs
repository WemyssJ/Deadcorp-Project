using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMenuHandler : MonoBehaviour
{
    private MainMenuButtonsHandler _mainMenuHandler;

    [HideInInspector] public MenuSaveSlot[] _saveSlots;
    [HideInInspector] public List<GameObject> SaveSlots = new List<GameObject>();

    private void Awake()
    {
        _mainMenuHandler = GetComponent<MainMenuButtonsHandler>();

        _saveSlots = GetComponentsInChildren<MenuSaveSlot>(true);
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            SaveSlots.Add(_saveSlots[i].gameObject);
        }
    }

    private void Start()
    {
        _mainMenuHandler.MainMenuParent.SetActive(true);
        _mainMenuHandler.LoadParent.SetActive(false);
    }

    public void OnLoadButton()
    {
        _mainMenuHandler.MainMenuParent.SetActive(false);
        _mainMenuHandler.LoadParent.SetActive(true);

        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].LoadAnySaveData();
        }
    }

    public void OnLoadGameBackButton()
    {
        _mainMenuHandler.LoadParent.SetActive(false);
        _mainMenuHandler.MainMenuParent.SetActive(true);
    }
}
