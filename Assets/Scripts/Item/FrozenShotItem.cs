using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 김동균
// 냉동 사격 아이템 클래스
// 기능 : 냉동 사격 아이템 효과 적용
public class FrozenShotItem : MonoBehaviour, IUseItem
{
    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("UseItem")]
    public void UseItem(GameObject player)
    {
        Debug.Log("FrozenShotItem Use");
        //Gun무기에 효과 부여
        var gun = player.GetComponent<Gun>();
        if(gun != null)
        {
            print("Gun found, applying FrozenShot effect.");
            gun.FreezeShot();
        }
        ItemObjectPool.Instance.ReturnItem(gameObject);
    }
}
