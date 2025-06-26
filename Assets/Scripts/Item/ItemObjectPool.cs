using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

//아이템 풀 관리
public class ItemObjectPool : MonoBehaviour
{
    public static ItemObjectPool Instance { get; private set; }
    
    [System.Serializable]
    public class PooledItem
    {
        public ItemData itemData;
        public int defaultCapacity = 10;
        public int maxSize = 100;
    }
    
    [Header("오브젝트 풀 설정")]
    public PooledItem[] pooledItems;
    
    private Dictionary<ItemData, ObjectPool<GameObject>> pools;
    private Dictionary<ItemData, GameObject> prefabLookup;
    
    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializePools()
    {
        pools = new Dictionary<ItemData, ObjectPool<GameObject>>();
        prefabLookup = new Dictionary<ItemData, GameObject>();
        
        foreach (var pooledItem in pooledItems)
        {
            if (pooledItem.itemData != null)
            {
                // 프리팹 룩업 테이블에 추가
                prefabLookup[pooledItem.itemData] = pooledItem.itemData.itemPrefab;
                
                // Unity Object Pool 생성
                var pool = new ObjectPool<GameObject>(
                    createFunc: () => CreateNewObject(pooledItem.itemData),
                    actionOnGet: (obj) => OnGetFromPool(obj, pooledItem.itemData),
                    actionOnRelease: (obj) => OnReleaseToPool(obj),
                    actionOnDestroy: (obj) => OnDestroyPoolObject(obj),
                    collectionCheck: true, // 중복 반환 체크
                    defaultCapacity: pooledItem.defaultCapacity,
                    maxSize: pooledItem.maxSize
                );
                
                pools[pooledItem.itemData] = pool;
                Debug.Log($"Unity Object Pool 초기화 완료: {pooledItem.itemData.itemName} (기본: {pooledItem.defaultCapacity}, 최대: {pooledItem.maxSize})");
            }
        }
    }
    
    private GameObject CreateNewObject(ItemData itemData)
    {
        if (!prefabLookup.ContainsKey(itemData))
        {
            Debug.LogError($"프리팹을 찾을 수 없습니다: {itemData.itemName}");
            return null;
        }
        
        GameObject obj = Instantiate(prefabLookup[itemData], transform);
        ItemWorldObject itemWorld = obj.GetComponent<ItemWorldObject>();
        
        if (itemWorld != null)
        {
            itemWorld.SetItemData(itemData);
        }
        
        return obj;
    }
    
    private void OnGetFromPool(GameObject obj, ItemData itemData)
    {
        // 풀에서 가져올 때 호출
        obj.SetActive(true);
        
        ItemWorldObject itemWorld = obj.GetComponent<ItemWorldObject>();
        if (itemWorld != null)
        {
            itemWorld.OnSpawnFromPool();
        }
    }
    
    private void OnReleaseToPool(GameObject obj)
    {
        // 풀로 반환할 때 호출
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
    
    private void OnDestroyPoolObject(GameObject obj)
    {
        // 풀에서 오브젝트를 제거할 때 호출
        if (obj != null)
        {
            Destroy(obj);
        }
    }
    
    // 풀에서 아이템 가져오기
    public GameObject GetItem(ItemData itemData, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(itemData))
        {
            Debug.LogWarning($"아이템 풀이 없습니다: {itemData.itemName}");
            return null;
        }
        
        GameObject obj = pools[itemData].Get();
        
        // 위치와 회전 설정
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        
        return obj;
    }
    
    // 아이템을 풀로 반환
    public void ReturnItem(GameObject item)
    {
        if (item == null) return;
        
        ItemWorldObject itemWorld = item.GetComponent<ItemWorldObject>();
        if (itemWorld == null) return;
        
        ItemData itemData = itemWorld.GetItemData();
        if (itemData == null || !pools.ContainsKey(itemData)) return;
        
        // Unity Object Pool로 반환
        pools[itemData].Release(item);
    }
    
    // 풀 정보 출력 (디버그용)
    public void PrintPoolInfo()
    {
        foreach (var kvp in pools)
        {
            var pool = kvp.Value;
            Debug.Log($"아이템: {kvp.Key.itemName}, 활성: {pool.CountActive}, 비활성: {pool.CountInactive}, 총: {pool.CountAll}");
        }
    }
    
    // 특정 풀의 정보 출력
    public void PrintPoolInfo(ItemData itemData)
    {
        if (pools.ContainsKey(itemData))
        {
            var pool = pools[itemData];
            Debug.Log($"아이템: {itemData.itemName}, 활성: {pool.CountActive}, 비활성: {pool.CountInactive}, 총: {pool.CountAll}");
        }
        else
        {
            Debug.LogWarning($"아이템 풀이 없습니다: {itemData.itemName}");
        }
    }
    
    // 풀 정리 (메모리 최적화)
    public void ClearPool(ItemData itemData)
    {
        if (pools.ContainsKey(itemData))
        {
            pools[itemData].Clear();
            Debug.Log($"풀 정리 완료: {itemData.itemName}");
        }
    }
    
    // 모든 풀 정리
    public void ClearAllPools()
    {
        foreach (var kvp in pools)
        {
            kvp.Value.Clear();
        }
        Debug.Log("모든 풀 정리 완료");
    }
    
    void OnDestroy()
    {
        // 모든 풀 정리
        if (pools != null)
        {
            foreach (var pool in pools.Values)
            {
                pool?.Dispose();
            }
        }
    }
} 