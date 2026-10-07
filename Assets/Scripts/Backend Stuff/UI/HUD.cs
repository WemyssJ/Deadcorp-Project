using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    //Health UI
    [SerializeField] private Player _player;
    [SerializeField] private Image HealthBar4;
    [SerializeField] private Image HealthBar3;
    [SerializeField] private Image HealthBar2;
    [SerializeField] private Image HealthBar1;
    [SerializeField] private Image HealthBar0;
    //Unlocks
    [SerializeField] private PlayerUnlocks _playerUnlocks;
    [SerializeField] private Image Dash;
    [SerializeField] private Image Pogo;
    [SerializeField] private Image DoubleJump;
    [SerializeField] private Image WallJump;
    [SerializeField] private Image AirDash;
    //Collectibles
    [SerializeField] private TMP_Text NumberOfCoins;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (_player.CurrentHealth == 4)
        {
            HealthBar4.fillAmount = 1;
            HealthBar3.fillAmount = 0;
            HealthBar2.fillAmount = 0;
            HealthBar1.fillAmount = 0;
            HealthBar0.fillAmount = 0;
        }
        //Player Health
        else if (_player.CurrentHealth == 3)
        {
            HealthBar4.fillAmount = 0;
            HealthBar3.fillAmount = 1;
            HealthBar2.fillAmount = 0;
            HealthBar1.fillAmount = 0;
            HealthBar0.fillAmount = 0;
        }
        else if (_player.CurrentHealth == 2)
        {
            HealthBar4.fillAmount = 0;
            HealthBar3.fillAmount = 0;
            HealthBar2.fillAmount = 1;
            HealthBar1.fillAmount = 0;
            HealthBar0.fillAmount = 0;
        }
        else if (_player.CurrentHealth == 1)
        {
            HealthBar4.fillAmount = 0;
            HealthBar3.fillAmount = 0;
            HealthBar2.fillAmount = 0;
            HealthBar1.fillAmount = 1;
            HealthBar0.fillAmount = 0;
        }
        else if (_player.CurrentHealth == 0)
        {
            HealthBar4.fillAmount = 0;
            HealthBar3.fillAmount = 0;
            HealthBar2.fillAmount = 0;
            HealthBar1.fillAmount = 0;
            HealthBar0.fillAmount = 1;
        }

        //Player Unlocks
        if (_playerUnlocks.DashUnlocked == true)
        {
            Dash.fillAmount = 1;
        }
        else
        {
            Dash.fillAmount = 0;
        }
        if (_playerUnlocks.PogoAttackUnlocked == true)
        {
            Pogo.fillAmount = 1;
        }
        else
        {
            Pogo.fillAmount = 0;
        }

        if (_playerUnlocks.DoubleJumpUnlocked == true)
        {
            DoubleJump.fillAmount = 1;
        }
        else
        {
            DoubleJump.fillAmount = 0;
        }

        if (_playerUnlocks.WallJumpSlideUnlocked == true)
        {
            WallJump.fillAmount = 1;
        }
        else
        {
            WallJump.fillAmount = 0;
        }

        if (_playerUnlocks.AirDashUnlocked == true)
        {
            AirDash.fillAmount = 1;
        }
        else
        {
            AirDash.fillAmount = 0;
        }


        UpdateNumberOfCoins();


    }
    public void UpdateNumberOfCoins()
    {
        NumberOfCoins.SetText(_player.CurrentCoins.ToString() + "x");
    }


}
