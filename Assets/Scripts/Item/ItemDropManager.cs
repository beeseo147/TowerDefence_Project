using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Drone,
    DroneBoss,
    DroneShooter,
    DroneHealer
}

public class ItemDropManager : MonoBehaviour
{
    public static ItemDropManager Instance { get; private set; }
    
    [System.Serializable]
    public class DropTable
    {
        public EnemyType enemyType; // Enum 사용으로 타입 안전성 확보
        public ItemData[] possibleItems; // 드롭 가능한 아이템들
        public float dropChance = 10f; // 드롭 확률 (0-100)
        public int minDropCount = 1; // 최소 드롭 개수
        public int maxDropCount = 1; // 최대 드롭 개수
    }
    
    [Header("아이템 드롭 설정")]
    public DropTable[] dropTables; // 적 타입별 드롭 테이블
    
    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 적이 죽었을 때 호출되는 메서드 (문자열 버전 - 기존 호환성)
    public void OnEnemyDeath(string enemyType, Vector3 deathPosition)
    {
        
        DropTable dropTable = GetDropTable(enemyType);
        if (dropTable == null) 
        {
            return;
        }
        
        
        // 드롭 확률 체크
        float randomValue = Random.Range(0f, 100f);
        
        if (randomValue < dropTable.dropChance)
        {
            // 드롭할 아이템 개수 결정
            int dropCount = Random.Range(dropTable.minDropCount, dropTable.maxDropCount + 1);
            Debug.Log($"드롭 개수: {dropCount} (min: {dropTable.minDropCount}, max: {dropTable.maxDropCount})");
            
            for (int i = 0; i < dropCount; i++)
            {
                DropRandomItem(dropTable, deathPosition);
            }
        }
        else
        {
            Debug.Log("드롭 확률에 실패하여 아이템이 드롭되지 않음");
        }
    }
    
    // 적이 죽었을 때 호출되는 메서드 (Enum 버전 - 새로운 방식)
    public void OnEnemyDeath(EnemyType enemyType, Vector3 deathPosition)
    {
        OnEnemyDeath(enemyType.ToString(), deathPosition);
    }
    
    private DropTable GetDropTable(string enemyType)
    {
        foreach (DropTable table in dropTables)
        {
            if (table.enemyType.ToString() == enemyType)
                return table;
        }
        return null;
    }
    
    private void DropRandomItem(DropTable dropTable, Vector3 position)
    {
        Debug.Log($"DropRandomItem 호출됨: {position}");
        
        if (dropTable.possibleItems.Length == 0) 
        {
            Debug.LogWarning("드롭 가능한 아이템이 없습니다!");
            return;
        }
        
        // 랜덤 아이템 선택
        ItemData randomItem = dropTable.possibleItems[Random.Range(0, dropTable.possibleItems.Length)];
        Debug.Log($"선택된 아이템: {randomItem.itemName}");
        
        // Object Pool을 사용하여 아이템 생성
        if (ItemObjectPool.Instance != null)
        {
            // 약간 위로 올려서 바닥에 묻히지 않도록
            Vector3 dropPosition = position;
            dropPosition.y += 0.5f;
            
            Debug.Log($"Object Pool에서 아이템 생성: {dropPosition}");
            GameObject droppedItem = ItemObjectPool.Instance.GetItem(randomItem, dropPosition, Quaternion.identity);
            
            if (droppedItem == null)
            {
                Debug.LogWarning($"Object Pool에서 아이템을 가져올 수 없습니다: {randomItem.itemName}");
                // Object Pool이 실패하면 기존 방식으로 생성
                CreateItemWithInstantiate(randomItem, dropPosition);
            }
        }
        else
        {
            // Object Pool이 없으면 기존 방식으로 생성
            Vector3 dropPosition = position;
            dropPosition.y += 0.5f;
            CreateItemWithInstantiate(randomItem, dropPosition);
        }
        
        Debug.Log($"아이템 드롭 완료: {randomItem.itemName} at {position}");
    }
    
    // 기존 Instantiate 방식 (Object Pool이 없을 때 사용)
    private void CreateItemWithInstantiate(ItemData randomItem, Vector3 position)
    {
        if (randomItem.itemPrefab != null)
        {
            Debug.Log($"Instantiate로 아이템 생성: {position}");
            GameObject droppedItem = Instantiate(randomItem.itemPrefab, position, Quaternion.identity);
            
            // 아이템 데이터 설정
            ItemWorldObject itemWorld = droppedItem.GetComponent<ItemWorldObject>();
            if (itemWorld != null)
            {
                itemWorld.SetItemData(randomItem);
            }
            else
            {
                Debug.LogWarning("ItemWorldObject 컴포넌트를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogWarning($"아이템 프리팹이 null입니다: {randomItem.itemName}");
        }
    }
} 