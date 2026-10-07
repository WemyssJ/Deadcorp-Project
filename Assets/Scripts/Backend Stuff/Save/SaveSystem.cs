using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    public static int SaveFileNumber { get; set; }
    public static bool IsNewGame { get; set; }

    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerData;
        public PlayerUnlocksData PlayerUnlocksData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + SaveFileNumber + ".save";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }


    private static void HandleSaveData()
    {
        GameManager.Instance.Player.Save(ref _saveData.PlayerData);
        GameManager.Instance.PlayerUnlocks.Save(ref _saveData.PlayerUnlocksData);

    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }



    private static void HandleLoadData()
    {
        GameManager.Instance.Player.Load(_saveData.PlayerData);
        GameManager.Instance.PlayerUnlocks.Load(_saveData.PlayerUnlocksData);
        ////GameManager.Instance.SceneData.Load(_saveData.SceneData);
    }
}
