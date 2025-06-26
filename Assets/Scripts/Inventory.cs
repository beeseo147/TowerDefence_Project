using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Inventory : MonoBehaviour
{
    [SerializeField]
    public Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();
    
    public void AddItem(ItemData data)
    {
        if (items.ContainsKey(data))
            items[data]++;
        else
            items.Add(data, 1);
    }

    public void InventoryUseItem(ItemData data)
    {
        print("UseItem 호출됨");
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

    void InventoryRemoveItem(ItemData data)
    {
        if (items.ContainsKey(data))
        {
            items[data]--;
            if (items[data] == 0)
                items.Remove(data);
        }
    }
}
