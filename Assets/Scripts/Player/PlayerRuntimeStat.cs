using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -------------------- Dynamic Data -------------------- */
public class PlayerRuntimeStat : MonoBehaviour
{
    [SerializeField] private PlayerStatBaseSO statBase;

    public string Name { get; private set; }
    public int Attack { get; private set; }
    public float CritChance { get; private set; }

    public event System.Action OnChanged;
    [SerializeField] Transform startPoint; // 임시. 이동예정..

    private void Awake()
    {
        if (null == statBase)
        {
            Debug.LogWarning("PlayerStatBaseSO is Null");
            return;
        }

        Name = statBase.baseName;
        Attack = statBase.baseAttackDamage;
        CritChance = statBase.baseCritChance;

        // Test
        Debug.Log($"[TEST] PlayerName : {Name}");
        Debug.Log($"[TEST] PlayerAttack : {Attack}");
        Debug.Log($"[TEST] PlayerCritChance : {CritChance}");

        if (startPoint == null)
        {
            var go = GameObject.FindWithTag("TeleportPoint");
            if (go == null)
            {
                Debug.LogError("TeleportPoint not found!");
                return;
            }
            startPoint = go.transform;
        }

        transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
