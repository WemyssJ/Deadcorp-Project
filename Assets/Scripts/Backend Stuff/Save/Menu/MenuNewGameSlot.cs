using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuNewGameSlot : MonoBehaviour
{
    [Header("GameObject References")]
    [SerializeField] private GameObject _saveDataObj;
    [SerializeField] private GameObject _noSaveDataObj;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _currencyText;

    private int _uniqueSaveSlotNumber;

    private NewGameMenuHandler _newGameHandler;
    private MainMenuButtonsHandler _mainMenuHandler;

    //private static SaveSystem.SaveData _saveData = new SaveSystem.SaveData();
    private static SaveSystem.SaveData _saveData = new SaveSystem.SaveData();

    private string _saveFile;

    private bool _hasSaveData;

    private void OnEnable()
    {
        _newGameHandler = GetComponentInParent<NewGameMenuHandler>();
        _mainMenuHandler = GetComponentInParent<MainMenuButtonsHandler>();

        _noSaveDataObj.SetActive(true);
        _saveDataObj.SetActive(false);
    }

    public void LoadAnySaveData()
    {
        for (int i = 0; i < _newGameHandler.NewGameSlotsObjs.Count; i++)
        {
            if (_newGameHandler.NewGameSlotsObjs[i] == gameObject)
            {
                _uniqueSaveSlotNumber = i;
                break;
            }
        }

        _saveFile = Application.persistentDataPath + "/save" + _uniqueSaveSlotNumber + ".save";

        if (File.Exists(_saveFile))
        {
            _hasSaveData = true;

            //for unencrypted saves
            string saveContent = File.ReadAllText(_saveFile);
            _saveData = JsonUtility.FromJson<SaveSystem.SaveData>(saveContent);


            UpdateUIFromLoad();
        }

        //this is redundant at first, but not when we call it after a delete slot
        else
        {
            _hasSaveData = false;
            _noSaveDataObj.SetActive(true);
            _saveDataObj.SetActive(false);
        }
    }

    private void UpdateUIFromLoad()
    {
        _currencyText.text = _saveData.PlayerData.CurrentCoins.ToString("N0");

        _saveDataObj.SetActive(true);
        _noSaveDataObj.SetActive(false);
    }

    //when we press the save slot button
    public void OnStartNewGameInThisSlotButton()
    {
        if (_hasSaveData)
        {
            return;
        }

        OnStartGame();
    }

    public void OnStartGame()
    {
        SaveSystem.SaveFileNumber = _uniqueSaveSlotNumber;
        SaveSystem.IsNewGame = true;
        SaveSystem.SaveFileNumber = _uniqueSaveSlotNumber;
        SaveSystem.IsNewGame = true;

        //fade screen to black
        _mainMenuHandler.FadeToBlack(0.5f, OnScreenFadedToBlackComplete);
    }

    private void OnScreenFadedToBlackComplete()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

}
