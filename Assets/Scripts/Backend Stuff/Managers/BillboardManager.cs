using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class BillboardManager : MonoBehaviour
{

    [SerializeField] private TMP_Text billboardText;
    [SerializeField] private string textToDisplay;
    [SerializeField] private float _fontSize;

    // Start is called before the first frame update
    void Start()
    {
            billboardText.SetText(textToDisplay);
            billboardText.fontSize = _fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
