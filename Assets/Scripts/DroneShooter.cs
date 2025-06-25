using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class DroneShooter : DroneAI
{
    public ParticleSystem bulletEffect;
    public Transform firePoint;
    public float rayDistance = 20f;
    public LayerMask targetMask;

    protected override void Attack(int attackPower)
    {
        
        currentTime += Time.deltaTime; 
        if (currentTime > attackDelayTime)
        {
            Vector3 direction = (tower.position - firePoint.position).normalized;
            Ray ray = new Ray(firePoint.position, direction);
            RaycastHit hit;
            Debug.DrawRay(firePoint.position, direction * rayDistance, Color.red,0.5f);

            if (Physics.Raycast(ray, out hit, rayDistance, targetMask))
            {
                if (hit.transform.name.Contains("Tower"))
                {
                    //print("TowerHit");
                    Tower.Instance.HP -= attackPower;

                    if (bulletEffect != null)
                    {
                        bulletEffect.Play();
                    }
                    bulletEffect.transform.position = hit.point;
                    bulletEffect.transform.forward = hit.normal;
                }
            }
            currentTime = 0;
        }
    }

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
}
