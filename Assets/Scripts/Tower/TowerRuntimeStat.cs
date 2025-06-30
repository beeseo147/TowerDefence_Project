using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// 작성자 : 윤여진
// 타워 런타임 클래스
// 기능 : 타워 체력, 타워 쉴드 처리, 타워 쉴드 시간 표시, 타워 쉴드 개수 표시
public class TowerRuntimeStat : MonoBehaviour
{
    public float  CurHp; // 타워 체력
    public bool   IsGodMode; // 타워 쉴드 처리
    public int    ShieldCharge { get; private set; } // 타워 쉴드 개수
    public float  maxShieldTime; // 타워 쉴드 시간
    public float remainShieldTime { get; private set; } // 타워 쉴드 시간

    [SerializeField] Material shieldMat; // 타워 쉴드 매터리얼
    Material originMat; // 타워 쉴드 매터리얼
    Renderer rend; // 타워 렌더러

    public UnityEvent<float, float> OnHpChanged; // 타워 체력 변경 이벤트
    public UnityEvent<float, float> OnRunningShield; // 타워 쉴드 시간 변경 이벤트
    public UnityEvent<int>          OnShieldAdded; // 타워 쉴드 개수 변경 이벤트

    Coroutine shieldRoutine; // 타워

    // 타워 런타임 초기화
    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        originMat = rend.sharedMaterial;    // 타워 쉴드 매터리얼 초기화
    }

    // 타워 런타임 초기화
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

    // 타워 데미지 처리
    public void TakeDamage(float damage, float maxHp)
    {
        CurHp -= damage;
        OnHpChanged.Invoke(CurHp, maxHp);
    }

    // 타워 쉴드 추가
    public void AddShield(int amount)
    {
        ShieldCharge += amount;
        OnShieldAdded?.Invoke(ShieldCharge);
        Debug.Log($"TowerRuntimeStat AddShield() : ShieldCharge {ShieldCharge}");
    }

    // 타워 쉴드 시작
    public bool StartShield()
    {
        if (shieldRoutine != null || ShieldCharge <= 0)
            return false;

        ShieldCharge--;

        shieldRoutine = StartCoroutine(CoUseShield());
        return true;
    }

    // 타워 쉴드 사용
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
            yield return wait; // 0.1��
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
