using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

// 작성자 : 김동균
// 인벤토리 클래스
// 기능 : 인벤토리 추가, 인벤토리 사용, 인벤토리 아이템 제거
public class Inventory : MonoBehaviour
{
    [SerializeField]
    public Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();
    public Text[] itemTexts;

    // 인벤토리 아이템 추가
    public void AddItem(ItemData data)
    {
        if (items.ContainsKey(data))
            items[data]++;
        else
            items.Add(data, 1);

        //사용 아이템인 Bomb 과 FrozenGun의 경우 아이템의 수량 텍스트 표시
        if (data.itemName == "Bomb" || data.itemName == "FrozenGun")
            itemTexts[data.id].text = items[data].ToString();
    }

    // 인벤토리 아이템 사용
    public void InventoryUseItem(ItemData data)
    {
        if (items.ContainsKey(data))
        {
            print("items.Count: " + items.Count);            
            var itemPrefab = data.itemPrefab;
            // 인스턴스 생성 후 UseItem 호출
            //GameObject itemObj = Instantiate(itemPrefab);
            GameObject itemObj = ItemObjectPool.Instance.GetItem(data, transform.position, transform.rotation);
            print("itemObj: " + itemObj);
            IUseItem useItem = itemObj.GetComponent<IUseItem>();
            if (useItem != null)
                useItem.UseItem(gameObject);
            else
                print(data.itemName + "이 없습니다.");
            InventoryRemoveItem(data);
        }
        else
        {
            print(data.itemName + "이 없습니다.");
        }
    }

    // 인벤토리 아이템 제거
    void InventoryRemoveItem(ItemData data)
    {
        if (items.ContainsKey(data))
        {
            items[data]--;
            if (items[data] == 0)
            {
                items.Remove(data);
                itemTexts[data.id].text = "0";
            }
            else
            {
                //사용 아이템인 Bomb 과 FrozenGun의 경우 아이템의 수량 텍스트 표시
                if (data.itemName == "Bomb" || data.itemName == "FrozenGun" )
                    itemTexts[data.id].text = items[data].ToString();
            }
        }
    }
}
