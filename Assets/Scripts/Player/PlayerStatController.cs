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
        Debug.Log(PlayerPrefs.GetString("PlayerName", "NoName"));
        Runtime.Init(baseSO);
        Debug.Log(Runtime.Name);
    }

    void Start()
    {
        StageManager.Instance.onStageChange += Upgrade;
    }
    public void ApplyStageBonus(int atk, float crit)
    {
        Runtime.AddAttack(atk);
        Runtime.AddCrit(crit);
    }

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

    public void OnEnemyKilled()
    {
        ScoreManager.Instance.AddKill();
    }

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
