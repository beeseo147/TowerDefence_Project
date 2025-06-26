using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -------------------- Dynamic Data -------------------- */
public class PlayerRuntimeStat : MonoBehaviour
{
    public string Name       { get; private set; }
    public int    Attack     { get; private set; }
    public float  CritChance { get; private set; }
    public float  CritMult   { get; private set; }

    public event System.Action OnChanged;
    
    [SerializeField] Transform startPoint; // 임시. 이동예정..

    private void Awake()
    {
        // 임시
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

    public void Init(PlayerStatBaseSO so)
    {
        if (null == so)
        {
            Debug.LogWarning("PlayerStatBaseSO is Null");
            return;
        }

        Name = so.baseName; // TODO : Get input values Later..

                Attack = so.baseAttackDamage;
        CritChance = so.baseCritChance;
        CritMult = so.baseCritMultiplier;
        OnChanged?.Invoke();
    }
    public void AddAttack(int value) { Attack += value; OnChanged?.Invoke(); }
    public void AddCrit(float value) { CritChance += value; OnChanged?.Invoke(); }
}
