using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* -------------------- Dynamic Data -------------------- */
public class PlayerRuntimeStat : MonoBehaviour
{
    public string Name       { get; private set; }
    public int    Attack     { get; private set; }
    public float  CritChance { get; private set; }
    public float  CritMult   { get; private set; }
    public int    EnemyKillCount { get; private set; }
    public UnityEvent OnChanged;
    
    [SerializeField] Transform startPoint; // �ӽ�. �̵�����..

    private void Awake()
    {
        // �ӽ�
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

        Name = PlayerPrefs.GetString("PlayerName", so.baseName);
        print(Name);
        Attack = so.baseAttackDamage;
        CritChance = so.baseCritChance;
        CritMult = so.baseCritMultiplier;
        EnemyKillCount = so.baseEnemyKillCount;
        OnChanged?.Invoke();
    }
    public void AddAttack(int value) { Attack += value; OnChanged?.Invoke(); }
    public void AddCrit(float value) { CritChance += value; OnChanged?.Invoke(); }
    public void AddEnemyKillCount(int value) { EnemyKillCount += value; OnChanged?.Invoke(); }
}
