using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 작성자 : 박세영
// 보스 드론 클래스
// 기능 : 보스 드론 생성, 보스 드론 처치 시 보스 드론 생성, 보스 드론 처치 시 보스 드론 처치 횟수 증가, 보스 드론 처치 시 보스 드론 처치 횟수 저장
public class DroneBoss : DroneAI
{
    [Header("보스 드론 생성")]
    [SerializeField] private GameObject[] splitDronePrefabs;
    [SerializeField] private GameObject spawnEffectPrefab;

    [Header("보스 드론 회전")]
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float rotateRadius = 10f;

    [Header("보스 드론 생성 증가 간격")]
    [SerializeField] private float spawnIncreaseInterval = 15f; // n초마다 생성 증가
    [SerializeField] private int maxSpawnCount = 5;

    private int spawnCount = 1;
    private float spawnTimer = 0f;
    private float startTime;
    private float angle;
    private bool isRotating = false;

    // 보스 드론 시작
    new void Start()
    {
        base.Start();

        startTime = Time.time;
        HpUI.SetActive(true);
    }

    // 보스 드론 이동
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

    // 보스 드론 회전
    private void RotateAroundTower()
    {
        angle += rotateSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        float rad = angle * Mathf.Deg2Rad;

        // 보스 드론 회전 위치
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * rotateRadius;
        Vector3 orbitPos = tower.position + offset;
        transform.position = orbitPos;

        // 보스 드론 회전 방향
        Vector3 lookDir = (tower.position - transform.position).normalized;
        lookDir.y = 0f;
        if(lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }
    // 보스 드론 공격
    protected override void Attack(int attackPower)
    {
        currentTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // 보스 드론 생성 증가 간격
        float elapsed = Time.time - startTime;
        spawnCount = Mathf.Min(1 + Mathf.FloorToInt(elapsed / spawnIncreaseInterval), maxSpawnCount);

        if (currentTime > attackDelayTime)
        {
            currentTime = 0;
            SpawnRandomSplitDrones(spawnCount);
        }
    }

    // 보스 드론 생성
    private void SpawnRandomSplitDrones(int count)
    {
        if (splitDronePrefabs == null || splitDronePrefabs.Length == 0) return;

        float radius = 5f;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
            Vector3 spawnPos = transform.position + offset;

            // 보스 드론 생성 효과
            if (spawnEffectPrefab != null)
            {
                GameObject fx = Instantiate(spawnEffectPrefab, spawnPos, Quaternion.identity);
                Destroy(fx, 1f); 
            }

            int index = Random.Range(0, splitDronePrefabs.Length);
            GameObject selectedPrefab = splitDronePrefabs[index];

            // 보스 드론 생성
            Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        }
    }

    // 보스 드론 데미지 처리
    public override void OnDamageProcess(int damage)
    {
        currentHp -= damage;
        if (currentHp > 0)
        {
            //state = DroneState.Damage;
            HpUI.GetComponentInChildren<Image>().fillAmount = (float)currentHp / maxHp;
            StopAllCoroutines(); //데미지 처리 코루틴 중지
            StartCoroutine(Damage());

        }
        else //보스 드론 체력이 0이면 사망
        {
            Die();
        }
    }

    // 보스 드론 데미지 처리
    protected override IEnumerator Damage()
    {
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = mat.color; //보스 드론 기본 색상
        mat.color = Color.black;  //보스 드론 데미지 효과
        //     GetComponentInChildren<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        //     GetComponentInChildren<MeshRenderer>().enabled = true;
        mat.color = originalColor;
    }//보스 드론 데미지 효과

    // 보스 드론 사망
    protected override void Die()
    {
        agent.enabled = false;

        //보스 드론 폭발 위치
        explosion.position = transform.position;
        //보스 드론 폭발 효과
        expEffect.Play();
        expAudio.Play(); //이펙트 사운드 재생
        
        // 보스 드론이 죽으면 게임 클리어 (승리)
        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.GameOver();
        }
        
        Destroy(gameObject); //보스 드론 없애기
    }
}
