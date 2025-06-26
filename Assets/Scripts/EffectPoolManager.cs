using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform effectParent;

    public static EffectPoolManager Instance;

    private IObjectPool<GameObject> effectPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        effectPool = new ObjectPool<GameObject>(
            CreateFunc,
            OnGetEffect,
            OnReleaseEffect,
            OnDestroyEffect,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 50
        );
    }

    private GameObject CreateFunc()
    {
        var effect = Instantiate(effectPrefab, effectParent);
        return effect;
    }

    private void OnGetEffect(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReleaseEffect(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyEffect(GameObject obj)
    {
        Destroy(obj);
    }

    public GameObject GetEffect(Vector3 position, Quaternion rotation)
    {
        GameObject effect = effectPool.Get();
        effect.transform.SetPositionAndRotation(position, rotation);
        StartCoroutine(AutoRelease(effect, .5f));
        return effect;
    }

    private IEnumerator AutoRelease(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        effectPool.Release(obj);
    }
}
