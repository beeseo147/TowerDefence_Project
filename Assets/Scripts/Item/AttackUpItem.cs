using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUpItem : MonoBehaviour, IPassiveItem
{
    [Header("공격력 증가량")]
    public int attackIncrease = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyPassiveEffect(GameObject collector)
    {
        print($"AttackUpItem 패시브 효과 적용됨 - 공격력 {attackIncrease} 증가");
        
        // 플레이어 공격력 증가
        var playerStats = collector.GetComponent<PlayerStatController>();
        if (playerStats != null && playerStats.Runtime != null)
        {
            playerStats.Runtime.AddAttack(attackIncrease);
            Debug.Log($"플레이어 공격력이 {attackIncrease} 증가했습니다. 현재 공격력: {playerStats.Runtime.Attack}");
        }
        else
        {
            Debug.LogWarning("PlayerStatController 또는 Runtime이 null입니다!");
        }
        
        // 아이템 사용 후 오브젝트 풀로 반환
        if (ItemObjectPool.Instance != null)
        {
            ItemObjectPool.Instance.ReturnItem(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
} 