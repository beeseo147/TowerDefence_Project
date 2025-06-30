using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성자 : 김동균
// 아이템 데이터에 대한 설명을 담고 있는 클래스
// 기능 : 아이템 이름, 아이템 설명, 아이템 고유 ID, 아이템 프리팹 설정
[CreateAssetMenu(fileName = "New Item", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int id; // 아이템의 고유 ID
    public GameObject itemPrefab;
}
