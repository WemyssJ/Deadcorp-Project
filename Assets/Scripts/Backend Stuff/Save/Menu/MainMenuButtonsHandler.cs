using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class MainMenuButtonsHandler : MonoBehaviour
{
    [Header("Rotation Parents")]
    public GameObject MainMenuParent;
    public GameObject LoadParent;
    public GameObject NewGameParent;

    public float FadeTime = 0.5f;
    [SerializeField] private Image _fadeImage;

    [Header("Load Button Stuff")]
    [SerializeField] private Button _loadButton;
    [SerializeField] private Image _loadButtonImage;
    [SerializeField] private TextMeshProUGUI _loadText;
    [SerializeField] private Color ButtonGreyedOutColor;
    [SerializeField] private Color ButtonTextGreyedOutColor;
    [SerializeField] private Color ButtonNormalColor;
    [SerializeField] private Color ButtonTextNormalColor;

    private Color _fadeColor;

    private void Awake()
    {
        _fadeColor.a = 0f;
        _fadeImage.color = _fadeColor;
        _fadeImage.enabled = false;

        GreyOutLoadButtonIfNoSaveData();
    }

    public void GreyOutLoadButtonIfNoSaveData()
    {
        int saveNumber = 0;
        string path = "";
        bool saveDataFound = false;

        MenuSaveSlot[] saveSlots = GetComponentsInChildren<MenuSaveSlot>(true);
        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveNumber = i;
            path = Application.persistentDataPath + "/save" + saveNumber + ".save";

            if (File.Exists(path))
            {
                saveDataFound = true;
                break;
            }
        }

        if (!saveDataFound)
        {
            _loadButtonImage.color = ButtonGreyedOutColor;
            _loadText.color = ButtonTextGreyedOutColor;
            _loadButton.interactable = false;
        }
        else
        {
            _loadButtonImage.color = ButtonNormalColor;
            _loadText.color = ButtonTextNormalColor;
            _loadButton.interactable = true;
        }
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void FadeToBlack(float fadeTime, System.Action OnComplete = null)
    {
        StartCoroutine(FadeScreenToBlackAction(fadeTime, OnComplete));
    }

    //private IEnumerator FadeScreenToBlack()
    //{
    //    _fadeImage.enabled = true;

    //    float elapsedTime = 0f;
    //    while (elapsedTime < FadeTime)
    //    {
    //        elapsedTime += Time.deltaTime;

    //        float alpha = Mathf.Lerp(0f, 1f, (elapsedTime / FadeTime));
    //        _fadeColor.a = alpha;
    //        _fadeImage.color = _fadeColor;

    //        yield return null;
    //    }

    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}

    private IEnumerator FadeScreenToBlackAction(float fadeTime, System.Action OnComplete = null)
    {
        _fadeImage.enabled = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, (elapsedTime / fadeTime));
            _fadeColor.a = alpha;
            _fadeImage.color = _fadeColor;

            yield return null;
        }

        _fadeColor.a = 1f;
        _fadeImage.color = _fadeColor;

        OnComplete?.Invoke();
    }
}
