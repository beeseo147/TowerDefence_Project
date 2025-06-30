using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 김동균
// 체력 회복 아이템 클래스
// 기능 : 체력 회복 아이템 효과 적용
public class HpUpItem : MonoBehaviour, IPassiveItem
{
    [Header("체력 회복량")]
    public int healAmount = 3;
    
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
        print($"HpUpItem 패시브 효과 적용됨 - 체력 {healAmount} 회복");
        
        // 타워 체력 회복 (음수 데미지로 힐링)
        if (Tower.Instance != null)
        {
            Tower.Instance.Runtime.CurHp += 3;
        }
        else
        {
            Debug.LogWarning("Tower.Instance가 null입니다!");
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
