using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlocks : MonoBehaviour
{
    [Header("Mobility Unlocks")]
    public bool DashUnlocked;
    public bool PogoAttackUnlocked;
    public bool DoubleJumpUnlocked;
    public bool WallJumpSlideUnlocked;
    public bool AirDashUnlocked;
    public bool AirDashResetOnWallUnlocked;

    [Header("Damage Unlocks")]
    public bool ParryUnlocked;
    public bool EnableCritDamage;
    public bool EnableDeadlyDamage;
    public bool IdleUpAttackUnlocked;

    public void Awake()
    {
        GameManager.Instance.PlayerUnlocks = this;
    }
    #region Save Data

    public void Save(ref PlayerUnlocksData data)
    {
        data.DashUnlocked = DashUnlocked;
        data.PogoAttackUnlocked = PogoAttackUnlocked;
        data.DoubleJumpUnlocked = DoubleJumpUnlocked;
        data.WallJumpSlideUnlocked = WallJumpSlideUnlocked;
        data.AirDashUnlocked = AirDashUnlocked;
        data.ParryUnlocked = ParryUnlocked;
        data.EnableCritDamage = EnableCritDamage;
        data.EnableDeadlyDamage = EnableDeadlyDamage;
        data.IdleUpAttackUnlocked = IdleUpAttackUnlocked;
        data.AirDashResetOnWallUnlocked = AirDashResetOnWallUnlocked;
    }

    public void Load(PlayerUnlocksData data)
    {
        DashUnlocked = data.DashUnlocked;
        PogoAttackUnlocked = data.PogoAttackUnlocked;
        DoubleJumpUnlocked = data.DoubleJumpUnlocked;
        WallJumpSlideUnlocked = data.WallJumpSlideUnlocked;
        AirDashUnlocked = data.AirDashUnlocked;
        ParryUnlocked = data.ParryUnlocked;
        EnableCritDamage = data.EnableCritDamage;
        EnableDeadlyDamage = data.EnableDeadlyDamage;
        IdleUpAttackUnlocked = data.IdleUpAttackUnlocked;
        AirDashResetOnWallUnlocked = data.AirDashResetOnWallUnlocked;
    }


    #endregion
}

[System.Serializable]
public struct PlayerUnlocksData
{
    //always unlocked
    public bool ParryUnlocked;

    //Core unlocks
    public bool DashUnlocked;
    public bool PogoAttackUnlocked;
    public bool DoubleJumpUnlocked;
    public bool WallJumpSlideUnlocked;
    public bool AirDashUnlocked;

    //trinket unlock
    public bool AirDashResetOnWallUnlocked;

    //not used
    public bool IdleUpAttackUnlocked;
    public bool EnableCritDamage;
    public bool EnableDeadlyDamage;
    


}
