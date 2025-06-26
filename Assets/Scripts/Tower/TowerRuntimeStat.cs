using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowerRuntimeStat : MonoBehaviour
{
    public float  CurHp             { get; private set; }
    public bool   IsGodMode         { get; private set; }
    public int    ShieldCharge      { get; private set; }
    public float  CurShieldDuration { get; private set; }

    public UnityEvent<float, float> OnHpChanged;
    //public UnityEvent OnStartGodMode;
    //public UnityEvent OnEndGodMode;

    void Start()
    {
        
    }

    public void Init(TowerStatBaseSO so)
    {
        if (null == so)
        {
            Debug.LogWarning("PlayerStatBaseSO is Null");
            return;
        }

        CurHp = so.baseMaxHP;
        IsGodMode = so.baseGodMode;
        ShieldCharge = so.baseShieldCharges;
        CurShieldDuration = so.baseShieldDuration;
        OnHpChanged?.Invoke(CurShieldDuration, so.baseShieldDuration);
    }

    public void TakeDamage(float damage, float maxHp)
    {
        CurHp -= damage;
        OnHpChanged.Invoke(CurHp, maxHp);
    }
}
