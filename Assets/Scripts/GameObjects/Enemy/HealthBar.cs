using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Image HealthBarImage;


    // Update is called once per frame

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        HealthBarImage.fillAmount = currentHealth / maxHealth;
    }
}
