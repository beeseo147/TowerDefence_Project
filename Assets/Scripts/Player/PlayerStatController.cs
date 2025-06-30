using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 윤여진
// 플레이어 스탯 컨트롤러 클래스
// 기능 : 플레이어 스탯 초기화, 플레이어 스탯 업그레이드, 플레이어 스탯 적용, 플레이어 스탯 초기화
public class PlayerStatController : MonoBehaviour
{
    [SerializeField] PlayerStatBaseSO baseSO;
    public PlayerRuntimeStat Runtime { get; private set; }

    // 플레이어 스탯 초기화
    void Awake()
    {
        Runtime = GetComponent<PlayerRuntimeStat>();
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
        Debug.Log(PlayerPrefs.GetString("PlayerName", "NoName"));
        Runtime.Init(baseSO);
        Debug.Log(Runtime.Name);
    }

    void Start()
    {
        StageManager.Instance.onStageChange += Upgrade;
    }
    // 플레이어 스탯 업그레이드
    public void ApplyStageBonus(int atk, float crit)
    {
        Runtime.AddAttack(atk);
        Runtime.AddCrit(crit);
    }

    // 플레이어 스탯 적용
    public int GetCurrentAttack()
    {
        int damage = Runtime.Attack;

        // ũ��Ƽ�� ����
        bool isCrit = UnityEngine.Random.value < Runtime.CritChance;
        if (isCrit)
        {
            damage = Mathf.RoundToInt(damage * Runtime.CritMult);
        }

        return damage;
    }

    // 플레이어 스탯 적용
    public void OnEnemyKilled()
    {
        ScoreManager.Instance.AddKill();
    }

    // 플레이어 스탯 업그레이드
    public void Upgrade(int stagenum)
    {
        switch (stagenum)
        {
            case 2:
            case 4:
            case 6:
                {
                    ApplyStageBonus(1, 0.05f);
                    break;
                }
        }
    }
}
