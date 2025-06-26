using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템 데이터에 대한 설명을 담고 있는 클래스
[CreateAssetMenu(fileName = "New Item", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int id; // 아이템의 고유 ID
    public GameObject itemPrefab;
}
