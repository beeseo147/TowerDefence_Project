using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletEffectPrefab;
    [SerializeField] private GameObject healEffectPrefab;
    [SerializeField] private Transform effectParent;

    public static EffectPoolManager Instance;

    private IObjectPool<GameObject> bulletPool;
    private IObjectPool<GameObject> healPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        bulletPool = CreatePool(bulletEffectPrefab);
        healPool = CreatePool(healEffectPrefab);
    }

    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            () => Instantiate(prefab, effectParent),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false),
            obj => Destroy(obj),
            false, 10, 50
            );
    }

    public GameObject GetBulletEffect(Vector3 position, Quaternion rotation)
    {
        return GetAndRelease(bulletPool, position, rotation, 0.5f);
    }

    public GameObject GetHealEffect(Vector3 position, Quaternion rotation)
    {
        return GetAndRelease(healPool, position, rotation, 0.5f);
    }

    private GameObject GetAndRelease(IObjectPool<GameObject> pool, Vector3 pos, Quaternion rot, float delay)
    {
        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(pos, rot);
        StartCoroutine(AutoRelease(pool, obj, delay));
        return obj;
    }

    private IEnumerator AutoRelease(IObjectPool<GameObject> pool, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Release(obj);
    }

    // 게임 재시작 시 이펙트 풀 초기화
    public void ResetGame()
    {
        Debug.Log("EffectPoolManager: 게임 데이터 초기화");
        
        // 모든 풀 정리
        bulletPool?.Clear();
        healPool?.Clear();
        
        Debug.Log("EffectPoolManager: 게임 데이터 초기화 완료");
    }
}
