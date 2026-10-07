using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUpGenerator : MonoBehaviour
{
    public static DamagePopUpGenerator current;
    public GameObject PreFab;
    private void Awake()
    {
        current = this;
    }

    public void CreatePopUp(Vector3 position, string text, Color Colour)
    {

        

        var popup = Instantiate(PreFab, position, Quaternion.identity);
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;
        temp.faceColor = Colour;
        Destroy(popup, 1f);
    }
}
