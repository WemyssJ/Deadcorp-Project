using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuSaveSlot : MonoBehaviour
{
    [Header("GameObject References")]
    [SerializeField] private GameObject _saveDataObj;
    [SerializeField] private GameObject _noSaveDataObj;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _currencyText;

    private int _uniqueSaveSlotNumber;

    private LoadMenuHandler _loadHandler;
    private MainMenuButtonsHandler _mainMenuHandler;

    //private static SaveSystem.SaveData _saveData = new SaveSystem.SaveData();
    private static SaveSystem.SaveData _saveData = new SaveSystem.SaveData();

    private string _saveFile;

    private bool _hasSaveData;


    private void OnEnable()
    {
        _loadHandler = GetComponentInParent<LoadMenuHandler>();
        _mainMenuHandler = GetComponentInParent<MainMenuButtonsHandler>();

        _noSaveDataObj.SetActive(true);
        _saveDataObj.SetActive(false);

        for (int i = 0; i < _loadHandler.SaveSlots.Count; i++)
        {
            if (_loadHandler.SaveSlots[i] == gameObject)
            {
                _uniqueSaveSlotNumber = i;
                break;
            }
        }

        _saveFile = Application.persistentDataPath + "/save" + _uniqueSaveSlotNumber + ".save";
    }

    public void LoadAnySaveData()
    {
        if (File.Exists(_saveFile))
        {
            _hasSaveData = true;
            string saveContent = File.ReadAllText(_saveFile);
            _saveData = JsonUtility.FromJson<SaveSystem.SaveData>(saveContent);

            //for encrypted saves
            //string encryptedContent = File.ReadAllText(_saveFile);
            //string decryptedContent = EncryptionUtility.DecryptString(encryptedContent);
            //_saveData = JsonUtility.FromJson<SaveSystem.SaveData>(decryptedContent);

            UpdateUIFromLoad();
        }
    }

    private void UpdateUIFromLoad()
    {
        //TODO: farm name
        _currencyText.text = _saveData.PlayerData.CurrentCoins.ToString("N0");

        _saveDataObj.SetActive(true);
        _noSaveDataObj?.SetActive(false);
    }

    //when we press the save slot button
    public void OnLoadThisSaveSlotButton()
    {
        if (_hasSaveData)
        {
            SaveSystem.SaveFileNumber = _uniqueSaveSlotNumber;
            SaveSystem.IsNewGame = false;
            SaveSystem.SaveFileNumber = _uniqueSaveSlotNumber;
            SaveSystem.IsNewGame = false;

            //fade screen to black
            _mainMenuHandler.FadeToBlack(0.5f, OnScreenFadedToBlack);
        }
    }

    private void OnScreenFadedToBlack()
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
