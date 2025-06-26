using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUpItem : MonoBehaviour, IUseItem
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseItem(GameObject player)
    {
        print("HpUpItem UseItem 호출됨");
        Tower.Instance.HP += 3; // 플레이어의 HP를 3 증가시킴
    }
}
