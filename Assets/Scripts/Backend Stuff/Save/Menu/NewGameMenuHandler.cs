using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewGameMenuHandler : MonoBehaviour
{
    [Header("UI References")]
    public GameObject FadeOtherElementsObj;

    private MainMenuButtonsHandler _mainMenuHandler;

    [HideInInspector] public MenuNewGameSlot[] NewGameSlots;
    [HideInInspector] public List<GameObject> NewGameSlotsObjs = new List<GameObject>();

    private void Awake()
    {
        _mainMenuHandler = GetComponent<MainMenuButtonsHandler>();

        NewGameSlots = GetComponentsInChildren<MenuNewGameSlot>(true);
        for (int i = 0; i < NewGameSlots.Length; i++)
        {
            NewGameSlotsObjs.Add(NewGameSlots[i].gameObject);
        }
    }

    private void Start()
    {
        _mainMenuHandler.NewGameParent.SetActive(false);
        FadeOtherElementsObj.SetActive(false);
    }

    public void OnNewGameButton()
    {
        _mainMenuHandler.MainMenuParent.SetActive(false);
        _mainMenuHandler.NewGameParent.SetActive(true);

        for (int i = 0; i < NewGameSlots.Length; i++)
        {
            NewGameSlots[i].LoadAnySaveData();
        }
    }

    public void OnNewGameBackButton()
    {
        _mainMenuHandler.NewGameParent.SetActive(false);
        _mainMenuHandler.MainMenuParent.SetActive(true);
    }

}
