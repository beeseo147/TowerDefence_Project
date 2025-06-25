using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    public static EffectPoolManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetEffect(Vector3 position, Quaternion rotation)
    {
        GameObject effect;
        if(pool.Count > 0)
        {
            effect = pool.Dequeue();
            effect.SetActive(true);
        }
        else
        {
            effect = Instantiate(effectPrefab);
        }
        effect.transform.SetPositionAndRotation(position, rotation);

        StartCoroutine(ReleaseAfter(effect, 2f));
        return effect;
    }

    private IEnumerator ReleaseAfter(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
