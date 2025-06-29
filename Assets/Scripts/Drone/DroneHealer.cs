using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class DroneHealer : DroneAI
{
    [SerializeField] private float allySearchRadius = 10f;
    [SerializeField] private LayerMask allyLayer;
    [SerializeField] GameObject healEffectPrefab;

    public float healRange = 3f;
    public int healAmount = 5;
    public float healInterval = 3f;

    private DroneAI healTarget;

    protected override void Move()
    {
        healTarget = FindLowestHpAlly();

        if (healTarget != null)
        {
            float distance = Vector3.Distance(transform.position, healTarget.transform.position);
            agent.SetDestination(healTarget.transform.position);

            if (distance < healRange)
            {
                state = DroneState.Attack;
                agent.isStopped = true;
            }
            else
            {
                state = DroneState.Move;
                agent.isStopped = false;
            }
        }

        else
        {
            // Tower가 파괴되었으면 Die 상태로 전환
            if (tower == null)
            {
                state = DroneState.Die;
                return;
            }
            
            // 힐 대상이 없으면 타워 근처 대기
            if (Vector3.Distance(transform.position, tower.position) > 5f)
            {
                agent.isStopped = false;
                agent.SetDestination(tower.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }

    protected override void Attack(int dummy)
    {
        // ���ݴ�� ��
        if (healTarget == null) return;

        float distance = Vector3.Distance(transform.position, healTarget.transform.position);
        if (distance > healRange || healTarget.CurrentHp >= healTarget.MaxHp)
        {
            state = DroneState.Move;
            return;
        }

        currentTime += Time.deltaTime;
        if (currentTime >= healInterval)
        {
            currentTime = 0;
            //Debug.Log("�� ����!");
            healTarget.Heal(healAmount);

            EffectPoolManager.Instance.GetHealEffect(transform.position, Quaternion.identity);
        }
    }

    private DroneAI FindLowestHpAlly()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, allySearchRadius, allyLayer);
        DroneAI lowest = null;
        float lowestRatio = 1f;

        foreach (var hit in hits)
        {
            DroneAI ally = hit.GetComponent<DroneAI>();
            if (ally == null || ally == this || ally.CurrentHp <= 0 || ally.CurrentHp >= ally.MaxHp) continue;
            if (ally.CompareTag("Healer")) continue;

            float ratio = (float)ally.CurrentHp / ally.MaxHp;
            if (ratio < lowestRatio)
            {
                lowestRatio = ratio;
                lowest = ally;
            }
        }
        return lowest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, allySearchRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
