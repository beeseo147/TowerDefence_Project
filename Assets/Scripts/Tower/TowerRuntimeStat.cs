using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class TowerRuntimeStat : MonoBehaviour
{
    public float  CurHp;
    public bool   IsGodMode;
    public int    ShieldCharge { get; private set; }
    public float  maxShieldTime;
    public float remainShieldTime { get; private set; }

    [SerializeField] Material shieldMat;
    Material originMat;
    Renderer rend;

    public UnityEvent<float, float> OnHpChanged;
    public UnityEvent<float, float> OnRunningShield;
    public UnityEvent<int>          OnShieldAdded;

    Coroutine shieldRoutine;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        originMat = rend.sharedMaterial;    // 원본 저장
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
        maxShieldTime = so.baseShieldDuration;
        OnHpChanged?.Invoke(CurHp, so.baseMaxHP);
    }

    public void TakeDamage(float damage, float maxHp)
    {
        CurHp -= damage;
        OnHpChanged.Invoke(CurHp, maxHp);
    }
    public void AddShield(int amount)
    {
        ShieldCharge += amount;
        OnShieldAdded?.Invoke(ShieldCharge);
        Debug.Log($"TowerRuntimeStat AddShield() : ShieldCharge {ShieldCharge}");
    }

    public bool StartShield()
    {
        if (shieldRoutine != null || ShieldCharge <= 0)
            return false;

        ShieldCharge--;

        shieldRoutine = StartCoroutine(CoUseShield());
        return true;
    }

    IEnumerator CoUseShield()
    {
        remainShieldTime = maxShieldTime;
        OnRunningShield?.Invoke(remainShieldTime, maxShieldTime);
        IsGodMode = true;
        rend.sharedMaterial = shieldMat;

        // Start Shield
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (remainShieldTime > 0f)
        {
            yield return wait; // 0.1초
            remainShieldTime = Mathf.Max(0, remainShieldTime - 0.1f);
            OnRunningShield.Invoke(remainShieldTime, maxShieldTime);
        }
        // FInish Shield
        shieldRoutine = null;
        OnRunningShield.Invoke(0f, maxShieldTime);
        rend.sharedMaterial = originMat;
        IsGodMode = false;
    }
}
