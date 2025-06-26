using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemWorldObject : MonoBehaviour, ICollectible
{
    [Header("아이템 설정")]
    public float rotationSpeed = 10f; // 회전 속도
    public float bobSpeed = 0.1f; // 위아래 움직임 속도
    public float bobHeight = 0.05f; // 위아래 움직임 높이
    private Vector3 startPosition;
    private float bobTime;
    private ItemData itemData;
    
    // 아이템 수집 이벤트
    public static event Action<ItemData, GameObject> OnItemCollected;
    
    void Start()
    {
        startPosition = transform.position;
        bobTime = UnityEngine.Random.Range(0f, 2f * Mathf.PI); // 랜덤 시작 시간
        print(startPosition);
    }
    
    // Object Pool에서 스폰될 때 호출
    public void OnSpawnFromPool()
    {
        startPosition = transform.position;
        bobTime = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        gameObject.SetActive(true);
    }
    
    void Enable()
    {
        gameObject.SetActive(true);
        startPosition = transform.position;
    }
    void Update()
    {
        // 회전 애니메이션
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // 위아래 움직임 애니메이션
        bobTime += bobSpeed * Time.deltaTime;
        float newY = startPosition.y + Mathf.Sin(bobTime) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    
    //아이템 데이터를 결정
    //ItemData에 itemPrefab 오브젝트가 있고 그 오브젝트에 IUseItem 인터페이스가 있다.
    //IUseItem 인터페이스를 구현한 클래스가 있고 그 클래스에 UseItem() 함수가 있다.
    //UseItem() 함수를 호출하면 아이템 효과를 적용한다.
    public void SetItemData(ItemData data)
    {
        itemData = data;
        print("SetItemData 호출됨: " + data.itemName);
    }
    
    // ItemData 반환 (Object Pool에서 사용)
    public ItemData GetItemData()
    {
        return itemData;
    }
    
    // ICollectible 인터페이스 구현
    public bool CanBeCollected(GameObject collector)
    {
        // 수집 가능한지 확인하는 로직
        return itemData != null && collector != null;
    }
    
    public void Collect(GameObject collector)
    {
        if (!CanBeCollected(collector))
            return;
            
        // 이벤트 발생
        OnItemCollected?.Invoke(itemData, collector);
        
        // 인벤토리에 추가
        var inventory = collector.GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem(this.itemData);
        }
        
        // Object Pool로 반환 (Destroy 대신)
        ReturnToPool();
    }
    
    // Object Pool로 반환
    private void ReturnToPool()
    {
        if (ItemObjectPool.Instance != null)
        {
            ItemObjectPool.Instance.ReturnItem(gameObject);
        }
        else
        {
            // Object Pool이 없으면 기존 방식으로 Destroy
            Destroy(gameObject);
        }
    }
    
    // 플레이어가 아이템을 획득했을 때 호출 (하위 호환성)
    // 플레이어가 Gun으로 총을 쏘면 아이템을 획득할 수 있다.
    public void CollectItem(GameObject player)
    {
        Collect(player);
    }
} 