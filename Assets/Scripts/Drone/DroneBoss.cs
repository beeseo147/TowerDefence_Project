using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneBoss : DroneAI
{
    [Header("분열 드론 프리팹")]
    [SerializeField] private GameObject[] splitDronePrefabs;
    [SerializeField] private GameObject spawnEffectPrefab;

    [Header("보스 이동")]
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float rotateRadius = 10f;

    [Header("시간 경과에 따라 소환 증가")]
    [SerializeField] private float spawnIncreaseInterval = 15f; // n초마다 소환 수 +1
    [SerializeField] private int maxSpawnCount = 5;

    private int spawnCount = 1;
    private float spawnTimer = 0f;
    private float startTime;
    private float angle;
    private bool isRotating = false;

    void Start()
    {
        base.Start();

        startTime = Time.time;
        HpUI.SetActive(true);
    }

    protected override void Move()
    {
        float targetRadius = rotateRadius;
        Vector3 directionToTower = (tower.position - transform.position).normalized;
        float currentDistance = Vector3.Distance(transform.position, tower.position);
        
        if(!isRotating)
        {
            if (currentDistance > targetRadius)
            {
                agent.SetDestination(tower.position);
            }
            else
            {
                isRotating = true;
                agent.isStopped = true;
                agent.updatePosition = false;
                agent.updateRotation = false;

                angle = Mathf.Atan2(transform.position.z - tower.position.z, transform.position.x - tower.position.x) * Mathf.Rad2Deg;
            }
        }

        else
        {
            RotateAroundTower();
            Attack(attackPower);
        }
    }

    private void RotateAroundTower()
    {
        angle += rotateSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        float rad = angle * Mathf.Deg2Rad;

        // 새로운 위치 계산 (타워 중심으로 원 궤도)
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * rotateRadius;
        Vector3 orbitPos = tower.position + offset;
        transform.position = orbitPos;

        // 항상 타워를 바라보도록 설정
        Vector3 lookDir = (tower.position - transform.position).normalized;
        lookDir.y = 0f;
        if(lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }
    protected override void Attack(int attackPower)
    {
        currentTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // 시간 경과에 따라 소환 수 증가
        float elapsed = Time.time - startTime;
        spawnCount = Mathf.Min(1 + Mathf.FloorToInt(elapsed / spawnIncreaseInterval), maxSpawnCount);

        if (currentTime > attackDelayTime)
        {
            currentTime = 0;
            SpawnRandomSplitDrones(spawnCount);
        }
    }

    private void SpawnRandomSplitDrones(int count)
    {
        if (splitDronePrefabs == null || splitDronePrefabs.Length == 0) return;

        float radius = 5f;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
            Vector3 spawnPos = transform.position + offset;

            // 이펙트 먼저 생성
            if (spawnEffectPrefab != null)
            {
                GameObject fx = Instantiate(spawnEffectPrefab, spawnPos, Quaternion.identity);
                Destroy(fx, 1f); 
            }

            int index = Random.Range(0, splitDronePrefabs.Length);
            GameObject selectedPrefab = splitDronePrefabs[index];

            // 드론 생성
            Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        }
    }

    public override void OnDamageProcess(int damage)
    {
        currentHp -= damage;
        if (currentHp > 0)
        {
            //state = DroneState.Damage;
            HpUI.GetComponentInChildren<Image>().fillAmount = (float)currentHp / maxHp;
            StopAllCoroutines(); //실행되고 있는 코루틴 함수가 있다면 중지시킴
            StartCoroutine(Damage());

        }
        else //죽었다면 폭발 이펙트 재생, 드론 파괴
        {
            Die();
        }
    }

    protected override IEnumerator Damage()
    {
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = mat.color; //원래 색 저장
        mat.color = Color.black;  //재질의 색을 검은색으로 변경
        //     GetComponentInChildren<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        //     GetComponentInChildren<MeshRenderer>().enabled = true;
        mat.color = originalColor;
    }//원래 색으로 변경

    protected override void Die()
    {
        agent.enabled = false;

        //폭발효과 위치 지정
        explosion.position = transform.position;
        //이펙트 재생
        expEffect.Play();
        expAudio.Play(); //이펙트 사운드 재생
        Destroy(gameObject); //드론 없애기
    }
}
