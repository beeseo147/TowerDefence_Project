using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class DroneShooter : DroneAI
{
    public Transform firePoint;
    public float rayDistance = 20f;
    public LayerMask targetMask;

    protected override void Move()
    {
        agent.SetDestination(tower.position);

        if (Vector3.Distance(transform.position, tower.position) < attackRange)
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

    protected override void Attack(int attackPower)
    {
        
        currentTime += Time.deltaTime; 
        if (currentTime > attackDelayTime)
        {
            Vector3 direction = (tower.position - firePoint.position).normalized;
            Ray ray = new Ray(firePoint.position, direction);
            RaycastHit hit;
            //Debug.DrawRay(firePoint.position, direction * rayDistance, Color.red,0.5f);

            if (Physics.Raycast(ray, out hit, rayDistance, targetMask))
            {
                if (hit.transform.name.Contains("Tower"))
                {
                    //print("TowerHit");
                    Quaternion rot = Quaternion.LookRotation(direction);
                    EffectPoolManager.Instance.GetBulletEffect(firePoint.position, rot);

                    Tower.Instance.TakeDamage(attackPower);
                    //Tower.Instance.HP -= attackPower;
                }
            }
            currentTime = 0;
        }
    }

}
