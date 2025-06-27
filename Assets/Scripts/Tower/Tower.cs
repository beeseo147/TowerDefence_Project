using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/* ----- Tower Controller ----- */
public class Tower : MonoBehaviour
{
    public static Tower Instance; //Tower의 싱글톤 객체
    //데미지 표현할 UI (빨간화면)
    public Transform damageUI;
    public Image damageImage;

    [SerializeField] TowerStatBaseSO baseSO;
    public TowerRuntimeStat Runtime { get; private set; }
    public Action onTowerDestroy;
    public float damageTime = 0.1f;
    int[]  hpThresholds = { 70, 50, 20 };
    bool[] usedShield   = { false, false, false };

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; //싱글톤 객체 값 할당
        }

        Runtime = GetComponent<TowerRuntimeStat>();
        if (!Runtime)
        {
            Debug.LogError("PlayerStatController Awake() : PlayerRuntimeStat component missing");
            enabled = false;
            return;
        } 
        if (!baseSO)
        {
            Debug.LogError("PlayerStatController Awake() : PlayerStatBaseSO is Null");
            enabled = false;
            return;
        }
    }
    void Start()
    {
        Runtime.Init(baseSO);
        Runtime.OnHpChanged.AddListener(CheckAutoShield);
        StageManager.Instance.onStageChange += Upgrade;
    
        //카메라의 nearClipPlane값을 저장
        float z = Camera.main.nearClipPlane + 0.1f;
        //데미지 UI를 카메라의 자식으로 등록
        damageUI.parent = Camera.main.transform;
        //UI 위치를 카메라의 near 값으로 설정
        damageUI.localPosition = new Vector3(0, 0, z);
        damageImage.enabled = false; //처음에는 비활성화
    }

    public void TakeDamage(float dmg)
    {
        if (Runtime.IsGodMode)
        {
            Debug.Log($"<color=green>TowerRuntimeStat TakeDamage() : God Mode!!</color>");
            return;
        }

        Runtime.TakeDamage(dmg, baseSO.baseMaxHP);
        FlashHit();
        if (Runtime.CurHp < 0)
        {
            onTowerDestroy?.Invoke();
            Destroy(gameObject); // 타워의 체력이 0이되면 타워, 플레이어, 카메라가 모두 제거
        }
    }

    IEnumerator DamageEvent()
    {
        //0.1초 동안 빨간색 이미지를 활성화/비활성화하여 피격효과를 재생함
        damageImage.enabled = true;
        yield return new WaitForSeconds(damageTime);
        damageImage.enabled = false;
    }

    void FlashHit()
    {
        StopAllCoroutines();
        StartCoroutine(DamageEvent());
    }

    void Upgrade(int stagenum)
    {
        switch (stagenum)
        {
            case 3:
            case 6:
                {
                    Runtime.AddShield(1);
                    break;
                }
            case 9:
                {
                    Runtime.AddShield(2);
                    break;
                }
        }
    }

    void CheckAutoShield(float curhp, float maxhp)
    {
        if (Runtime.ShieldCharge <= 0)
            return;

        for (int i = 0; i < hpThresholds.Length; i++)
        {
            if (usedShield[i] || curhp > hpThresholds[i]) continue;

            Runtime.StartShield();

            usedShield[i] = true;                 
            break;                              
        }
    }
}
