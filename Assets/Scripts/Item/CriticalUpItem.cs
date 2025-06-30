using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 김동균
// 크리티컬 확률 증가 아이템 클래스
// 기능 : 크리티컬 확률 증가 아이템 효과 적용
public class CriticalUpItem : MonoBehaviour, IPassiveItem
{
    [Header("크리티컬 확률 증가량")]
    public float critIncrease = 0.05f; // 5% 증가
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 패시브 아이템 효과 적용
    public void ApplyPassiveEffect(GameObject collector)
    {
        print($"CriticalUpItem 패시브 효과 적용됨 - 크리티컬 확률 {critIncrease * 100}% 증가");
        
        // 플레이어 크리티컬 확률 증가
        var playerStats = collector.GetComponent<PlayerStatController>();
        if (playerStats != null && playerStats.Runtime != null)
        {
            playerStats.Runtime.AddCrit(critIncrease);
            Debug.Log($"플레이어 크리티컬 확률이 {critIncrease * 100}% 증가했습니다. 현재 크리티컬 확률: {playerStats.Runtime.CritChance * 100}%");
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