using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
#endif
using UnityEngine;

// 작성자 : 박세영
// 총 드론 클래스
// 기능 : 총 드론 이동, 총 드론 공격, 총 드론 체력 회복, 총 드론 얼리기, 총 드론 얼림 해제
public class DroneShooter : DroneAI
{
    public Transform firePoint;
    public float rayDistance = 20f;
    public LayerMask targetMask;

    // 총 드론 이동
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

    // 총 드론 공격
    protected override void Attack(int attackPower)
    {
        // 공격 딜레이 시간 증가
        currentTime += Time.deltaTime; 
        if (currentTime > attackDelayTime)
        {
            // 타워 방향 계산
            Vector3 direction = (tower.position - firePoint.position).normalized;
            Ray ray = new Ray(firePoint.position, direction);
            RaycastHit hit;
            //Debug.DrawRay(firePoint.position, direction * rayDistance, Color.red,0.5f);

            // 타워 충돌 체크
            if (Physics.Raycast(ray, out hit, rayDistance, targetMask))
            {
                // 타워 충돌 체크
                if (hit.transform.name.Contains("Tower"))
                {
                    // 타워 효과 표시
                    Quaternion rot = Quaternion.LookRotation(direction);
                    EffectPoolManager.Instance.GetBulletEffect(firePoint.position, rot);

                    // 타워 데미지 적용
                    Tower.Instance.TakeDamage(attackPower);
                }
            }
            // 공격 딜레이 시간 초기화
            currentTime = 0;
        }
    }

}
