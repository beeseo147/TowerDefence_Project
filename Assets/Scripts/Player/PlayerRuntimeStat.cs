using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* -------------------- Dynamic Data -------------------- */
// 작성자 : 윤여진
// 플레이어 런타임 클래스
// 기능 : 플레이어 이름, 플레이어 공격력, 플레이어 크리티컬 확률, 플레이어 크리티컬 데미지, 플레이어 적 처치 횟수, 플레이어 변경 이벤트
public class PlayerRuntimeStat : MonoBehaviour
{
    public string Name       { get; private set; } // 플레이어 이름
    public int    Attack     { get; private set; } // 플레이어 공격력
    public float  CritChance { get; private set; } // 플레이어 크리티컬 확률
    public float  CritMult   { get; private set; } // 플레이어 크리티컬 데미지
    public int    EnemyKillCount { get; private set; } // 플레이어 적 처치 횟수
    public UnityEvent OnChanged; // 플레이어 변경 이벤트
    
    [SerializeField] Transform startPoint; // 플레이어 시작 위치

    // 플레이어 초기화
    private void Awake()
    {
        // 플레이어 시작 위치 초기화
        if (startPoint == null)
        {
            var go = GameObject.FindWithTag("TeleportPoint");
            if (go == null)
            {
                Debug.LogError("TeleportPoint not found!");
                return;
            }
            startPoint = go.transform;
        }

        transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
    }

    // 플레이어 초기화
    public void Init(PlayerStatBaseSO so)
    {
        if (null == so)
        {
            Debug.LogWarning("PlayerStatBaseSO is Null");
            return;
        }

        Name = PlayerPrefs.GetString("PlayerName", so.baseName);
        print(Name);
        Attack = so.baseAttackDamage;
        CritChance = so.baseCritChance;
        CritMult = so.baseCritMultiplier;
        EnemyKillCount = so.baseEnemyKillCount;
        OnChanged?.Invoke();
    }
    public void AddAttack(int value) { Attack += value; OnChanged?.Invoke(); }
    public void AddCrit(float value) { CritChance += value; OnChanged?.Invoke(); }
    public void AddEnemyKillCount(int value) { EnemyKillCount += value; OnChanged?.Invoke(); }
}
