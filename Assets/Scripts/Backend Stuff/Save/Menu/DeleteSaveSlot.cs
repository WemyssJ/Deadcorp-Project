using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DeleteSaveSlot : MonoBehaviour
{
    [Header("Delete References")]
    [SerializeField] private GameObject _deleteButtonObj;
    [SerializeField] private GameObject _confirmButtonsObj;

    private int _uniqueSaveSlotNumber;
    private string _saveFile;

    private DeleteSaveSlotHandler _handler;
    private NewGameMenuHandler _newGameHandler;
    private MainMenuButtonsHandler _mainMenuHandler;

    private void Awake()
    {
        _handler = GetComponentInParent<DeleteSaveSlotHandler>();
        _newGameHandler = GetComponentInParent<NewGameMenuHandler>();
        _mainMenuHandler = GetComponentInParent<MainMenuButtonsHandler>();

        _deleteButtonObj.SetActive(false);
        _confirmButtonsObj.SetActive(false);
    }

    private void OnEnable()
    {
        if (_handler.DeleteSlotObjs.Count > 0)
        RefreshDeleteSaveButtonUI();
    }

    #region Delete Save Slot

    public void RefreshDeleteSaveButtonUI()
    {
        _confirmButtonsObj.SetActive(false);

        for (int i = 0; i < _handler.DeleteSlotObjs.Count; i++)
        {
            if (_handler.DeleteSlotObjs[i] == gameObject)
            {
                _uniqueSaveSlotNumber = i;
                break;
            }
        }

        _saveFile = Application.persistentDataPath + "/save" + _uniqueSaveSlotNumber + ".save";

        if (File.Exists(_saveFile))
        {
            _deleteButtonObj.SetActive(true);
        }

    }

    public void OnDeleteSlotButton()
    {
        for (int i = 0; i < _handler.DeleteSlotObjs.Count; i++)
        {
            if (_handler.DeleteSlotObjs[i] == gameObject)
            {
                _uniqueSaveSlotNumber = i;
                break;
            }
        }

        _saveFile = Application.persistentDataPath + "/save" + _uniqueSaveSlotNumber + ".save";

        _deleteButtonObj.SetActive(false);
        _confirmButtonsObj.SetActive(true);

    }

    public void OnConfirmYesButton()
    {
        File.Delete(_saveFile);

        //refresh the UI
        for (int i = 0; i < _newGameHandler.NewGameSlots.Length; i++)
        {
            _newGameHandler.NewGameSlots[i].LoadAnySaveData();
        }

        RefreshDeleteSaveButtonUI();

        //refresh the load button and whether it should be greyed out or not.
        _mainMenuHandler.GreyOutLoadButtonIfNoSaveData();

    }

    public void OnConfirmNoButton()
    {
        _confirmButtonsObj.SetActive(false);
        _deleteButtonObj.SetActive(true);
    }


    #endregion

}
