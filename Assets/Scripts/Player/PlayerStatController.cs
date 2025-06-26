using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [SerializeField] PlayerStatBaseSO baseSO;
    public PlayerRuntimeStat Runtime { get; private set; }

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

        Runtime.Init(baseSO);
    }

    // 임시
    public void ApplyStageBonus(int atk, float crit)
    {
        Runtime.AddAttack(atk);
        Runtime.AddCrit(crit);
    }

    public int GetCurrentAttack()
    {
        int damage = Runtime.Attack;

        // 크리티컬 판정
        bool isCrit = UnityEngine.Random.value < Runtime.CritChance;
        if (isCrit)
        {
            damage = Mathf.RoundToInt(damage * Runtime.CritMult);
        }

        return damage;
    }

    public void OnEnemyKilled()
    {
        ScoreManager.Instance.AddKill();
    }
}
