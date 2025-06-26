using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ----- Tower Controller ----- */
public class Tower : MonoBehaviour
{
    public static Tower Instance; //Tower의 싱글톤 객체
    //데미지 표현할 UI
    public Transform damageUI; 
    public Image damageImage;
    //public Image hpBarIamage;
    //public Transform HpUI;

    [SerializeField] TowerStatBaseSO baseSO;
    public TowerRuntimeStat Runtime { get; private set; }

    public Action onTowerDestroy;
    float _hp = 100;
    public float HP //프로퍼티 
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            Debug.Log($"Tower CurHp : {_hp}");
            StopAllCoroutines(); //기존 진행 중인 코루틴 해제
            StartCoroutine(DamageEvent()); //깜박거림을 처리할 코루틴 함수 호출
            TakeDamage(1);
            if (_hp <= 0)
            {
                onTowerDestroy?.Invoke();
                Destroy(gameObject); // 타워의 체력이 0이되면 타워, 플레이어, 카메라가 모두 제거
            }
        }
    }
    public float damageTime = 0.1f;


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

        Runtime.Init(baseSO);
    }
    // Start is called before the first frame update
    void Start()
    {
        //카메라의 nearClipPlane값을 저장
        float z = Camera.main.nearClipPlane + 0.1f;
        //데미지 UI를 카메라의 자식으로 등록
        damageUI.parent = Camera.main.transform;
        //UI 위치를 카메라의 near 값으로 설정
        damageUI.localPosition = new Vector3(0, 0, z);
        damageImage.enabled = false; //처음에는 비활성화
        //HpUI.parent = Camera.main.transform;
        //HpUI.localPosition = new Vector3(0.38f, 0.18f, z);
        //HpUI.GetComponentInChildren<Image>().fillAmount = 1;
    }

    public void TakeDamage(float dmg)
    {
        _hp -= dmg;
        Runtime.TakeDamage(dmg, baseSO.baseMaxHP);
        //Runtime.OnHpChanged.Invoke(_hp, baseSO.baseMaxHP);
        FlashHit();
    }
    IEnumerator DamageEvent()
    {
        //0.1초 동안 빨간색 이미지를 활성화/비활성화하여 피격효과를 재생함
        damageImage.enabled = true;
        yield return new WaitForSeconds(damageTime);
        damageImage.enabled = false;
        //hpBarIamage.fillAmount = (float)HP / initialHP;
        //HpUI.GetComponentInChildren<Image>().fillAmount = (float)HP / initialHP;
    }

    void FlashHit()
    {
        StopAllCoroutines();
        StartCoroutine(DamageEvent());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
